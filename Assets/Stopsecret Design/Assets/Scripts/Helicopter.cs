using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.UI;

public class Helicopter : MonoBehaviour {

    private bool grounded = false;
    private new Rigidbody rigidbody;

    [SerializeField] private ScreenButton upBtn;
    [SerializeField] private ScreenButton downBtn;
    [SerializeField] private Joystick joystic;

    [SerializeField]
    private float liftForce = 20f;
    [SerializeField]
    private float speed = 25f;
    [SerializeField]
    private float pitchAngle = 90f;
    [SerializeField]
    private float maxTurningAngle = 30f;
    [SerializeField]
    private new Transform camera;
    [SerializeField]
    private Text groundSpeed;
    [SerializeField]
    private Text heightAGL;


    private new AudioSource audio;
    private float audioIntensity;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        audio = GetComponent<AudioSource>();
    }

    void OnCollisionEnter()
    {
        grounded = true;
    }

    void OnCollisionExit()
    {
        grounded = false;
    }

    private void Update()
    {
        //Raycast down to see how many meters above ground we are
        RaycastHit hit = new RaycastHit();
        int heightAGLVal = 9999;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit))
        {
            heightAGLVal = (int)hit.distance;
        }
        //Ground speed is our rigidbody x and z velocity combined
        int groundSpeedVal = (int)new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z).magnitude;
        //Update the UI
        heightAGL.text = string.Format("<size=50>{0}</size> <i>m AGL</i>", heightAGLVal);
        groundSpeed.text = string.Format("<size=50>{0}</size> <i>m/s</i>", groundSpeedVal);
    }

    void FixedUpdate()
    {
        //The forward vector of the camera, disregarding the y axis
        Vector3 cameraForwardFlat = new Vector3(camera.forward.x, 0, camera.forward.z).normalized;
        //Normalized input
        Vector3 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (joystic != null)
            input = new Vector2(joystic.Horizontal, joystic.Vertical);
        if (input.magnitude > 1f)
        {
            input.Normalize();
        }
        //Lift

        float upwardsForce = -Physics.gravity.y*.98f;
        if (upBtn != null && upBtn.IsPressed)
            upwardsForce += liftForce;
        else
            upwardsForce += Input.GetAxis("Jump") * liftForce;

        if (downBtn != null && downBtn.IsPressed)
            upwardsForce -= liftForce;
        else
            upwardsForce -= Input.GetAxis("Fire3") * liftForce;

        upwardsForce *= rigidbody.mass;
        rigidbody.AddForce(transform.up * upwardsForce);
        //Steering
        transform.Rotate(camera.right, input.y * Time.deltaTime * pitchAngle, Space.World);
        transform.Rotate(camera.forward, -input.x * Time.deltaTime * pitchAngle, Space.World);
        rigidbody.AddForce(cameraForwardFlat * input.y * speed);
        rigidbody.AddForce(camera.right * input.x * speed);
        //Auto correct the helicopter's rotation so it doesn't flip over
        if (!grounded)
        {
            Quaternion trueRotationTotal = Quaternion.LookRotation(cameraForwardFlat, Vector3.up);
            Quaternion trueRotation = Quaternion.RotateTowards(transform.rotation, trueRotationTotal, maxTurningAngle);
            transform.rotation = Quaternion.Slerp(transform.rotation, trueRotation, 3f * Time.deltaTime);
        }
        UpdateAudio();

    }
    void UpdateAudio()
    {
        if (audio == null) return;
        float intensity = Mathf.Abs(Input.GetAxis("Horizontal")) + Mathf.Abs(Input.GetAxis("Vertical")) + Mathf.Abs(Input.GetAxis("Jump")) + Mathf.Abs(Input.GetAxis("Fire3"));
        if (joystic != null)
            intensity = Mathf.Abs(joystic.Horizontal + joystic.Vertical + Mathf.Abs(Input.GetAxis("Jump")) + Mathf.Abs(Input.GetAxis("Fire3")));

        audioIntensity = Mathf.Lerp(audioIntensity, Mathf.Clamp(intensity, 0f, 1f), 1f * Time.deltaTime);
        audio.volume = Mathf.Lerp(.5f, 1.1f, audioIntensity);
        audio.pitch = Mathf.Lerp(0.9f, 1.1f, audioIntensity);
    }
}
