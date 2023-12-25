//----------------------------------------------
//      Realistic Hovercraft Controller
//
// Copyright © 2015  - 2022 BoneCracker Games
// http://www.bonecrackergames.com
//
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class RHC_InputManager : RHC_Singleton<RHC_InputManager> {

    public RHC_Inputs inputs = new RHC_Inputs();
    private static RHC_InputActions inputActions;

    public static bool gyroUsed = false;

    public delegate void onChangeCamera();
    public static event onChangeCamera OnChangeCamera;

    public delegate void onHeadlights();
    public static event onHeadlights OnHeadlights;

    private void Awake() {

        gameObject.hideFlags = HideFlags.HideInHierarchy;

        //  Creating inputs.
        inputs = new RHC_Inputs();

    }

    private void Update() {

        //  Creating inputs.
        if (inputs == null)
            inputs = new RHC_Inputs();

        //  Receive inputs from the controller.
        GetInputs();

    }

    private void GetInputs() {

#if RHC_OLDINPUT

        inputs.throttleInput = Input.GetAxis("Vertical");
        inputs.brakeInput = -Mathf.Clamp(Input.GetAxis("Vertical"), -1f, 0f);
        inputs.steerInput = Input.GetAxis("Horizontal");
        inputs.elevationInput = Input.GetAxis("Elevation");

        inputs.orbitX = Input.GetAxis("Mouse X");
        inputs.orbitY = Input.GetAxis("Mouse Y");

#else

        if (inputActions == null) {

            inputActions = new RHC_InputActions();
            inputActions.Enable();

            inputActions.Camera.ChangeCamera.performed += ChangeCamera_performed;
            inputActions.Vehicle.Headlights.performed += Headlights_performed; ;

        }

        inputs.throttleInput = inputActions.Vehicle.Throttle.ReadValue<float>();
        inputs.brakeInput = inputActions.Vehicle.Brake.ReadValue<float>();
        inputs.steerInput = inputActions.Vehicle.Steering.ReadValue<float>();
        inputs.elevationInput = inputActions.Vehicle.Elevation.ReadValue<float>();

        inputs.orbitX = inputActions.Camera.Orbit.ReadValue<Vector2>().x;
        inputs.orbitY = inputActions.Camera.Orbit.ReadValue<Vector2>().y;

#endif

    }


    private static void Headlights_performed(InputAction.CallbackContext obj) {

        if (OnHeadlights != null)
            OnHeadlights();

    }

    private static void ChangeCamera_performed(InputAction.CallbackContext obj) {

        if (OnChangeCamera != null)
            OnChangeCamera();

    }

}
