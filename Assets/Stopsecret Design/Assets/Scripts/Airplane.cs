using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Airplane : MonoBehaviour {

    private Rigidbody rb;
    private float currentThrottle;
    private float x, y;
    private float roll, pitch, yaw;

    [SerializeField] private FixedTouchField cameraJoystick;
    [SerializeField] private Joystick joystick;
    //[SerializeField] private ScreenButton upBtn;
    //[SerializeField] private ScreenButton downBtn;

    [SerializeField]
    private float power, acceleration, liftPower, turnPower, graphicsTurnAngle, graphicsBankAngle, graphicsDisplacement, uprightStrength;
    [SerializeField]
    private bool invertY;
    [SerializeField]
    private Transform graphics;
    [SerializeField]
    private RectTransform aimReticle;
    [SerializeField]
    private RectTransform cursor;
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private SuspensionWheel[] wheels;
    [SerializeField]
    private Text groundSpeed;
    [SerializeField]
    private Text heightAGL;
    [SerializeField]
    private Slider accelerationSlider;
    [SerializeField]
    private new AudioSource audio;



    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
        audio = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        
        
        float forwardForce = currentThrottle * power;
        float liftForce = currentThrottle * liftPower;
        pitch = y * (invertY ? -1f : 1f);
        yaw = x;
        roll = -Input.GetAxis("Horizontal");

        foreach (SuspensionWheel wheel in wheels)
        {
            if (wheel.IsGrounded())
            {
                rb.drag = 2f;
                yaw = x;
            }
        }

        rb.AddRelativeForce(0, liftForce, forwardForce);
        Vector3 torque = new Vector3(pitch * turnPower, yaw * turnPower, roll * turnPower * .1f);
        float dampener = Mathf.Clamp((rb.velocity.magnitude / 20f), 0f, 1f);
        rb.AddRelativeTorque(torque*dampener);
        rb.drag = Mathf.Clamp(currentThrottle*3f+.5f, .1f, 2f);

        Quaternion upright = Quaternion.LookRotation(transform.forward);
        transform.rotation = Quaternion.Lerp(transform.rotation, upright, uprightStrength * Time.deltaTime);

        
    }
    private void LateUpdate()
    {
        float maxCursorRange = .03f;

        if (cameraJoystick != null)
        {
            x += 0.1f * cameraJoystick.TouchDelta.x;
            y += 0.1f * cameraJoystick.TouchDelta.y;
        }
        else
        {
            x += Input.GetAxis("Mouse X") * Time.deltaTime;
            y += Input.GetAxis("Mouse Y") * Time.deltaTime;
        }

        if (joystick != null)
        {
            currentThrottle += joystick.Vertical * Time.deltaTime * acceleration;
            x += 0.1f * joystick.Horizontal * Time.deltaTime;
            /*if (upBtn.IsPressed)
                currentThrottle += Time.deltaTime * acceleration;

            if (downBtn.IsPressed)
                currentThrottle -= Time.deltaTime * acceleration;*/
        }
        else
            currentThrottle += Input.GetAxis("Vertical") * Time.deltaTime * acceleration;
        
        x = Mathf.Lerp(x, 0, 3f * Time.deltaTime);
        y = Mathf.Lerp(y, 0, 3f * Time.deltaTime);
        x = Mathf.Clamp(x, -maxCursorRange, maxCursorRange);
        y = Mathf.Clamp(y, -maxCursorRange, maxCursorRange);
        currentThrottle = Mathf.Clamp(currentThrottle, 0f, 1f);

        if (cursor != null)
            cursor.localPosition = Vector2.Lerp(cursor.localPosition /*aimReticle.localPosition + */
                                              , new Vector3(x * 3000f, y * 3000f, 0)
                                              , 20f * Time.deltaTime);

        graphics.Rotate(pitch * graphicsTurnAngle*currentThrottle*Time.deltaTime * 100f
                      , yaw * .5f * graphicsTurnAngle * currentThrottle * Time.deltaTime * 100f
                      , ((roll * graphicsTurnAngle*.1f) - (yaw * graphicsBankAngle)) * currentThrottle * Time.deltaTime * 100f);
        graphics.localRotation = Quaternion.Slerp(graphics.localRotation, Quaternion.identity, 5f * Time.deltaTime);
        graphics.localPosition = Vector3.Lerp(
            graphics.localPosition
          , new Vector3(-x* (graphicsDisplacement/maxCursorRange), 
                        -y* (graphicsDisplacement/maxCursorRange), 
                        -(currentThrottle* (graphicsDisplacement*.5f)))*currentThrottle
          , 1f * Time.deltaTime);

        
        UpdateUI();
        UpdateAudio();
    }
    void WorldToScreenPosition (RectTransform uiElement, Vector3 worldPos)
    {

        //first you need the RectTransform component of your canvas
        RectTransform CanvasRect = uiElement.GetComponentInParent<Canvas>().GetComponent<RectTransform>();

        //then you calculate the position of the UI element
        //0,0 for the canvas is at the center of the screen, whereas WorldToViewPortPoint treats the lower left corner as 0,0. Because of this, you need to subtract the height / width of the canvas * 0.5 to get the correct position.

        Vector2 ViewportPosition = cam.WorldToViewportPoint(worldPos);
        Vector2 WorldObject_ScreenPosition = new Vector2(
        ((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
        ((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)));

        //now you can set the position of the ui element
        uiElement.anchoredPosition = WorldObject_ScreenPosition;
    }
    private void UpdateUI()
    {
        //Raycast down to see how many meters above ground we are
        RaycastHit hit = new RaycastHit();
        int heightAGLVal = 9999;
        if (Physics.Raycast(transform.position, -Vector3.up, out hit))
        {
            heightAGLVal = (int)hit.distance;
        }
        //Ground speed is our rigidbody x and z velocity combined
        int groundSpeedVal = (int)new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude;
        //Update the UI
        heightAGL.text = string.Format("<size=50>{0}</size> <i>m AGL</i>", heightAGLVal);
        groundSpeed.text = string.Format("<size=50>{0}</size> <i>m/s</i>", groundSpeedVal);
        accelerationSlider.value = currentThrottle;
        //Set the cursor's true aim
        hit = new RaycastHit();
        if (Physics.Raycast(graphics.transform.position, graphics.transform.forward, out hit))
        {
            WorldToScreenPosition(aimReticle, hit.point);
        }
        else
        {
            WorldToScreenPosition(aimReticle, graphics.transform.position + (graphics.transform.forward * 1000f));
        }
    }
    private void UpdateAudio()
    {
        if (audio == null) return;
        audio.volume = Mathf.Lerp(0.9f, 1f, currentThrottle);
        audio.pitch = Mathf.Lerp(0.2f, 1.1f, currentThrottle);
    }
}
