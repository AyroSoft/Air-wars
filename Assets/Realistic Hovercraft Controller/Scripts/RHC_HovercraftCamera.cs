//----------------------------------------------
//      Realistic Hovercraft Controller
//
// Copyright © 2015  - 2022 BoneCracker Games
// http://www.bonecrackergames.com
//
//----------------------------------------------

using UnityEngine;
using System.Collections;

[AddComponentMenu("BoneCracker Games/Realistic Hovercraft Controller/Main/RHC Hovercraft Camera")]
public class RHC_HovercraftCamera : MonoBehaviour {

    private Camera cam;
    public Camera Cam {

        get {

            if (cam == null)
                cam = GetComponent<Camera>();

            return cam;

        }

    }

    // The target we are following
    public RHC_HovercraftController target;

    // The distance in the x-z plane to the target
    public float distance = 6f;

    // the height we want the camera to be above the target
    public float height = 2.4f;

    public float heightOffset = 1f;
    public float heightDamping = 5f;
    public float rotationDamping = 5f;
    public bool useSmoothRotation = true;

    public bool orbit = false;
    public float orbitXSpeed = .2f;
    public float orbitYSpeed = .2f;
    private Vector2 orbitVector = new Vector2(0f, 0f);

    private float m_rotationDamping = 5f;

    public float minimumFOV = 40f;
    public float maximumFOV = 60f;

    public float maximumTilt = 25f;
    private float tiltAngle = 0f;

    private void OnEnable() {

        if (!target)
            target = FindObjectOfType<RHC_HovercraftController>();

    }

    private void Update() {

        // Early out if we don't have a target
        if (!target)
            return;

        Vector3 localVelocity = target.transform.InverseTransformDirection(target.Rigid.velocity);

        //Tilt Angle Calculation.
        tiltAngle = Mathf.Lerp(tiltAngle, (Mathf.Clamp(localVelocity.x, -35, 35)), Time.deltaTime * 5f);

        Cam.fieldOfView = Mathf.Lerp(Cam.fieldOfView, Mathf.Lerp(minimumFOV, maximumFOV, (localVelocity.magnitude * 3.6f) / 150f), Time.deltaTime * 5f);

        if (orbit) {

            RHC_Inputs inputs = RHC_InputManager.Instance.inputs;
            orbitVector += new Vector2(inputs.orbitX * orbitXSpeed, inputs.orbitY * orbitYSpeed);

        }

    }

    private void LateUpdate() {

        // Early out if we don't have a target
        if (!target)
            return;

        float speed = target.currentSpeed;

        // Calculate the current rotation angles.
        float wantedRotationAngle = target.transform.eulerAngles.y;
        float wantedHeight = target.transform.position.y + height;
        float currentRotationAngle = transform.eulerAngles.y;
        float currentHeight = transform.position.y;

        if (useSmoothRotation)
            m_rotationDamping = Mathf.Lerp(0f, 1f, Mathf.Abs(speed) / 50f);
        else
            m_rotationDamping = 1f;

        if (speed < -10)
            wantedRotationAngle += 180;

        // Damp the rotation around the y-axis
        currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, m_rotationDamping * rotationDamping * Time.deltaTime);

        // Damp the height
        currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

        // Convert the angle into a rotation
        Quaternion currentRotation;

        if (!orbit)
            currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);
        else
            currentRotation = Quaternion.Euler(0f, orbitVector.x, 0f);

        // Set the position of the camera on the x-z plane to:
        // distance meters behind the target
        transform.position = target.transform.position;
        transform.position -= currentRotation * Vector3.forward * distance;

        // Set the height of the camera
        transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);

        // Always look at the target
        transform.LookAt(new Vector3(target.transform.position.x, target.transform.position.y + heightOffset, target.transform.position.z));
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, Mathf.Clamp(tiltAngle, -10f, 10f));

    }

}