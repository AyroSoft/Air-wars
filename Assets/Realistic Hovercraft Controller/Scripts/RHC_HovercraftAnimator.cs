//----------------------------------------------
//      Realistic Hovercraft Controller
//
// Copyright © 2015  - 2022 BoneCracker Games
// http://www.bonecrackergames.com
//
//----------------------------------------------

using UnityEngine;
using System.Collections;

[AddComponentMenu("BoneCracker Games/Realistic Hovercraft Controller/Misc/RHC Hovercraft Animator")]
public class RHC_HovercraftAnimator : MonoBehaviour {

    private RHC_HovercraftController hovercraftController;
    private RHC_HovercraftController HovercraftController {

        get {

            if (hovercraftController == null)
                hovercraftController = GetComponentInParent<RHC_HovercraftController>();

            return hovercraftController;

        }

    }

    private Animator driverAnimator;
    private Animator DriverAnimator {

        get {

            if (driverAnimator == null)
                driverAnimator = GetComponent<Animator>();

            return driverAnimator;

        }

    }

    private void Update() {

        if (!HovercraftController || !DriverAnimator)
            return;

        DriverAnimator.SetFloat("steer", HovercraftController.steerInput);

    }

}
