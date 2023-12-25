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

[AddComponentMenu("BoneCracker Games/Realistic Hovercraft Controller/Misc/RHC Booster Particle")]
public class RHC_BoosterParticle : MonoBehaviour {

    private RHC_HovercraftController hovercraftController;
    private RHC_HovercraftController HovercraftController {

        get {

            if (hovercraftController == null)
                hovercraftController = GetComponentInParent<RHC_HovercraftController>();

            return hovercraftController;

        }

    }

    private ParticleSystem particle;
    private ParticleSystem Particle {

        get {

            if (particle == null)
                particle = GetComponent<ParticleSystem>();

            return particle;

        }

    }


    private ParticleSystem.EmissionModule emission;

    public float minSpeed, maxSpeed;

    [System.Serializable]
    public class ParticleLight {

        public Light particleLight;
        public float maxIntensity = 1f;

    }

    public ParticleLight particleLight = new ParticleLight();

    private void Update() {

        if (!HovercraftController || !Particle)
            return;

        emission = Particle.emission;
        emission.enabled = HovercraftController.engineRunning;

        var main = Particle.main;

        main.startSpeed = Mathf.Clamp(HovercraftController.throttleInput, minSpeed, maxSpeed);

        if (particleLight.particleLight != null)
            particleLight.particleLight.intensity = Mathf.Clamp(HovercraftController.throttleInput, 0f, 1f) * particleLight.maxIntensity;

    }

}
