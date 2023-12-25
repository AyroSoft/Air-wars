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

[AddComponentMenu("BoneCracker Games/Realistic Hovercraft Controller/Misc/RHC Thruster Particle")]
public class RHC_ThrusterParticle : MonoBehaviour {

    private RHC_Thruster thruster;
    private RHC_HovercraftController hovercraftController;
    private ParticleSystem particle;
    private ParticleSystem.EmissionModule emission;

    [System.Serializable]
    public class ParticleLight {

        public Light particleLight;
        public float maxIntensity = 1f;

    }

    public ParticleLight particleLight = new ParticleLight();

    private void Awake() {

        thruster = GetComponentInParent<RHC_Thruster>();
        hovercraftController = GetComponentInParent<RHC_HovercraftController>();
        particle = GetComponent<ParticleSystem>();

    }

    private void Update() {

        if (!thruster || !hovercraftController || !particle)
            return;

        if (particleLight.particleLight != null)
            particleLight.particleLight.intensity = Mathf.Clamp(hovercraftController.throttleInput, 0f, 1f) * particleLight.maxIntensity;

    }

    private void FixedUpdate() {

        if (!thruster || !hovercraftController || !particle)
            return;

        emission = particle.emission;

        RaycastHit hit;

        if (Physics.Raycast(thruster.transform.position, thruster.transform.forward, out hit, hovercraftController.stableHeight * 2f) && hit.transform.root != hovercraftController.transform) {

            if (!emission.enabled)
                emission.enabled = true;

            transform.position = hit.point;

        } else {

            if (emission.enabled)
                emission.enabled = false;

        }

    }

}
