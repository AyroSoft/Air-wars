using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class OrbitCamera : MonoBehaviour {

    [SerializeField] private FixedTouchField cameraJoystick;
    [SerializeField] private Joystick leftJoystick;

    [SerializeField]
    private Transform target, headTarget;
    [SerializeField]
    private float rotateSpeed = 180f;
    [SerializeField]
    private float minZoomDistance = 5f, maxZoomDistance = 20f;
    [SerializeField]
    private float zoomSpeed = 20f;
    [SerializeField]
    private float distanceAbove = 0f;

    private Camera thisCam;
    private float timeOut;
    private float xRot;
    private float yRot;
    private float yRotAimed;
    private float distance = 5f;
    private bool isHeadTarget;

    // Use this for initialization
    void Start () {
        thisCam = GetComponent<Camera>();
        distance = (minZoomDistance + maxZoomDistance) / 2f;
    }
	
	// Update is called once per frame
	void LateUpdate () {
        if (Input.GetKeyDown(KeyCode.C))
        {
            isHeadTarget = !isHeadTarget;
        }
        
        if (target == null || headTarget == null) return;
        //Keyboard rotation timeout
        timeOut -= Time.deltaTime;
        if (cameraJoystick != null)
        {
            if (cameraJoystick.TouchDelta.magnitude > 0.1f)
            {
                timeOut = 3000f;
            }
        }
        else if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            timeOut = 3000f;
        }

        //zooming
        distance -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * Time.deltaTime;
        distance = Mathf.Clamp(distance, minZoomDistance, maxZoomDistance);
        transform.position = isHeadTarget ? headTarget.position : target.position;
        
        transform.rotation = Quaternion.identity;

        if (cameraJoystick != null)
            xRot += -cameraJoystick.TouchDelta.y * rotateSpeed * Time.deltaTime;
        else
            xRot += -Input.GetAxis("Mouse Y") * rotateSpeed * Time.deltaTime;

        xRot = Mathf.Clamp(xRot, -50, 88);

        transform.rotation = Quaternion.identity;
        transform.Rotate(0, yRot, 0);
        transform.RotateAround(transform.position, transform.right, xRot);
        TranslateCamera(.3f, (transform.forward * -distance) + (Vector3.up * distanceAbove));

        if (cameraJoystick != null)
            yRot += cameraJoystick.TouchDelta.x * rotateSpeed * Time.deltaTime;
        else
            yRot += Input.GetAxis("Mouse X") * rotateSpeed * Time.deltaTime;

        if (timeOut < 0)
        {
            if (leftJoystick != null)
                yRot += leftJoystick.Horizontal * .5f * rotateSpeed * Time.deltaTime;
            else
                yRot += Input.GetAxis("Horizontal") * .5f * rotateSpeed * Time.deltaTime;
        }
        yRot %= 360f;
        
    }
    void TranslateCamera(float radius, Vector3 normal)
    {
        if (isHeadTarget) return;
        RaycastHit hit = new RaycastHit();
        if (Physics.SphereCast(transform.position, radius, normal.normalized, out hit, normal.magnitude, (1 << 0)))
        {
            transform.position += normal.normalized * hit.distance;
        }
        else
        {
            transform.position += normal;
        }
        transform.Translate(0, distanceAbove, 0);

    }
}
