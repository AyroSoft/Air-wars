//----------------------------------------------
//      Realistic Hovercraft Controller
//
// Copyright © 2015  - 2022 BoneCracker Games
// http://www.bonecrackergames.com
//
//----------------------------------------------

using UnityEngine;
using System.Collections;

[AddComponentMenu("BoneCracker Games/Realistic Hovercraft Controller/Main/RHC Thruster")]
public class RHC_Thruster : MonoBehaviour {

    private RHC_HovercraftController hovercraftController;
    private RHC_HovercraftController HovercraftController {

        get {

            if (hovercraftController == null)
                hovercraftController = GetComponentInParent<RHC_HovercraftController>();

            return hovercraftController;

        }

    }

    private float fuelInput = 1f;

    private float springForce;
    private float damperForce;
    private float springConstant;
    private float damperConstant;
    private float restLenght;

    private float previouseLenght;
    private float currentLenght;
    private float springVelocity;

    public float springLimit = 5000f;

    private void FixedUpdate() {

        if (!HovercraftController) {

            enabled = false;
            return;

        }

        fuelInput = Mathf.Lerp(fuelInput, HovercraftController.engineRunning ? 1f : 0f, Time.fixedDeltaTime);

        springConstant = HovercraftController.thrusterSpring;
        damperConstant = HovercraftController.thrusterDamp;
        restLenght = HovercraftController.stableHeight;

        RaycastHit hit;

        if (Physics.Raycast(transform.position, -HovercraftController.transform.up, out hit, restLenght + .5f)) {

            previouseLenght = currentLenght;
            currentLenght = restLenght - (hit.distance - .5f);
            springVelocity = (currentLenght - previouseLenght) / Time.fixedDeltaTime;
            springForce = springConstant * currentLenght;
            damperForce = damperConstant * springVelocity;

            springForce = Mathf.Clamp(springForce, -springLimit, springLimit);

            HovercraftController.Rigid.AddForceAtPosition((HovercraftController.transform.up * (springForce + damperForce)) * fuelInput, transform.position);

        }

    }

    private void Reset() {

        RHC_ThrusterParticle particle = GetComponentInChildren<RHC_ThrusterParticle>();

        if (particle)
            return;

        GameObject newParticles = GameObject.Instantiate((GameObject)Resources.Load("RHC_ThrusterParticle", typeof(GameObject)), transform.position, transform.rotation);
        newParticles.transform.SetParent(transform, true);

    }

}