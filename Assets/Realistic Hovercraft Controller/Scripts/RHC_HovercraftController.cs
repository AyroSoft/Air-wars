//----------------------------------------------
//      Realistic Hovercraft Controller
//
// Copyright © 2015  - 2022 BoneCracker Games
// http://www.bonecrackergames.com
//
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("BoneCracker Games/Realistic Hovercraft Controller/Main/RHC Hovercraft Controller")]
[RequireComponent(typeof(Rigidbody))]
public class RHC_HovercraftController : MonoBehaviour {

    private Rigidbody rigid;
    public Rigidbody Rigid {

        get {

            if (rigid == null)
                rigid = GetComponent<Rigidbody>();

            return rigid;

        }

    }

    public Transform COM;       // Center of mass

    // Receive Player Inputs and Engine Running Bool
    public bool canControl = true;
    public bool engineRunning = true;

    // Stabilizer Thrusters
    private RHC_Thruster[] stabilizerThrusters = new RHC_Thruster[0];

    public float thrusterSpring = 5000f;
    public float thrusterDamp = 1000f;

    // Torques
    public float motorTorque = 5000f;
    public float steerTorque = 3500f;
    public float brakeTorque = 4000f;

    // Engine Torque Curve Depends On Craft Speed
    public AnimationCurve motorTorqueCurve = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(150f, .1f));
    public bool autoCreateMotorTorqueCurve = true;

    // Speeds
    public float currentSpeed = 0f;
    public float maximumSpeed = 150f;

    // Inputs
    public RHC_Inputs inputs;
    [HideInInspector] public float throttleInput = 0f;
    [HideInInspector] public float brakeInput = 0f;
    [HideInInspector] public float steerInput = 0f;
    [HideInInspector] public float elevationInput = 0f;

    //	Input sensitivity.
    [Range(.01f, 1f)] public float throttleSensitivity = .25f;
    [Range(.01f, 1f)] public float steeringSensitivity = .25f;

    // Stabilizer Variables
    private readonly float stability = .5f;
    private readonly float reflection = 100f;
    public float stableHeight = 3f;
    public float maximumAngularVelocity = 2f;

    // Sounds
    public AudioClip engineSound;
    private AudioSource engineSoundSource;
    public AudioClip[] crashSounds;
    private AudioSource crashSoundSource;

    //Lights
    public Light[] headLights;
    public bool headlightsOn = false;

    //Contach Particles
    public GameObject contactSparkle;
    private readonly int maximumContactSparkle = 5;
    private List<ParticleSystem> contactSparkeList = new List<ParticleSystem>();

    public bool useDamage = true;
    public RHC_Damage damage;

    //  Particles
    public RHC_BoosterParticle[] boosterParticles;
    public RHC_ThrusterParticle[] thrusterParticles;

    // ---EVENTS---
    public delegate void RHCSpawned();
    public static event RHCSpawned OnRHCSpawned;

    public delegate void RHCDestroyed();
    public static event RHCDestroyed OnRHCDestroyed;

    private void Awake() {

        Rigid.centerOfMass = COM.localPosition;
        Rigid.maxAngularVelocity = maximumAngularVelocity;

        stabilizerThrusters = GetComponentsInChildren<RHC_Thruster>();

        SoundsInit();
        ParticlesInit();

        damage.Initialize(this, GetComponentsInChildren<MeshFilter>(true));

    }

    private void OnEnable() {

        if (OnRHCSpawned != null)
            OnRHCSpawned();

        RHC_InputManager.OnHeadlights += RHC_InputManager_OnHeadlights;

    }

    private void RHC_InputManager_OnHeadlights() {

        headlightsOn = !headlightsOn;

    }

    private void SoundsInit() {

        engineSoundSource = RHC_CreateAudioSource.NewAudioSource(gameObject, "Engine Sound", 5f, 1f, engineSound, true, true, false);

    }

    private void ParticlesInit() {

        if (contactSparkle) {

            for (int i = 0; i < maximumContactSparkle; i++) {

                GameObject sparks = (GameObject)Instantiate(contactSparkle, transform.position, Quaternion.identity) as GameObject;
                sparks.transform.SetParent(transform);
                contactSparkeList.Add(sparks.GetComponent<ParticleSystem>());
                ParticleSystem.EmissionModule em = sparks.GetComponent<ParticleSystem>().emission;
                em.enabled = false;

            }

        }

    }

    private void Update() {

        Inputs();
        Audio();
        Lights();
        Damage();

    }

    private void FixedUpdate() {

        if (!engineRunning)
            return;

        Engine();
        Stabilizer();

    }

    private void Inputs() {

        if (!canControl)
            return;

        inputs = RHC_InputManager.Instance.inputs;

        throttleInput = Mathf.MoveTowards(throttleInput, inputs.throttleInput, throttleSensitivity * Time.deltaTime * 10f);
        brakeInput = Mathf.MoveTowards(brakeInput, inputs.brakeInput, throttleSensitivity * Time.deltaTime * 10f);
        steerInput = Mathf.MoveTowards(steerInput, inputs.steerInput, steeringSensitivity * Time.deltaTime * 10f);
        elevationInput = Mathf.MoveTowards(elevationInput, inputs.elevationInput, .5f * Time.deltaTime * 10f);

    }

    private void Engine() {

        currentSpeed = transform.InverseTransformDirection(Rigid.velocity).z * 3.6f;

        if (currentSpeed < maximumSpeed)
            Rigid.AddForceAtPosition(transform.forward * ((motorTorque * Mathf.Clamp(throttleInput, -.25f, 1f)) * motorTorqueCurve.Evaluate(currentSpeed)), COM.position, ForceMode.Force);

        Rigid.AddForceAtPosition(transform.right * (steerTorque * steerInput * motorTorqueCurve.Evaluate(currentSpeed)), COM.position, ForceMode.Force);
        Rigid.AddRelativeTorque(Vector3.up * (steerTorque * steerInput * motorTorqueCurve.Evaluate(currentSpeed)), ForceMode.Force);
        Rigid.AddRelativeTorque(Vector3.forward * ((-steerTorque) * steerInput), ForceMode.Force);

        if (currentSpeed > 0f)
            Rigid.AddForceAtPosition(-transform.forward * brakeTorque * Mathf.Clamp(brakeInput, 0f, 1f), COM.position, ForceMode.Force);

        Vector3 locVel = transform.InverseTransformDirection(Rigid.velocity);
        locVel = new Vector3(Mathf.Lerp(locVel.x, 0f, Time.fixedDeltaTime * 3f), locVel.y + elevationInput / 7f, locVel.z);

        Rigid.velocity = transform.TransformDirection(locVel);
        Rigid.angularVelocity = Vector3.Lerp(Rigid.angularVelocity, Vector3.zero, Time.fixedDeltaTime * 1f);

    }

    private void Stabilizer() {

        Vector3 predictedUp = Quaternion.AngleAxis(Rigid.velocity.magnitude * Mathf.Rad2Deg * stability / reflection, Rigid.angularVelocity) * transform.up;
        Vector3 torqueVector = Vector3.Cross(predictedUp, Vector3.up);

        Rigid.AddTorque(torqueVector * reflection * reflection);

    }

    private void Audio() {

        if (engineRunning) {

            engineSoundSource.volume = Mathf.Lerp(engineSoundSource.volume, Mathf.Clamp(Mathf.Abs(throttleInput), .3f, 1f), Time.deltaTime * 2f);
            engineSoundSource.pitch = Mathf.Lerp(engineSoundSource.pitch, Mathf.Clamp(Mathf.Abs(throttleInput) + Mathf.Lerp(0f, .25f, currentSpeed / maximumSpeed), .45f, 1.5f), Time.deltaTime * 2f);

        } else {

            engineSoundSource.volume = Mathf.Lerp(engineSoundSource.volume, 0f, Time.deltaTime * 2f);
            engineSoundSource.pitch = Mathf.Lerp(engineSoundSource.pitch, 0f, Time.deltaTime * 2f);

        }

    }

    private void Lights() {

        foreach (Light l in headLights)
            l.enabled = headlightsOn;

    }

    private void Damage() {

        if (useDamage && damage != null) {

            damage.Damage();
            damage.Repair();

        }

    }

    private void CollisionParticles(Vector3 contactPoint) {

        for (int i = 0; i < contactSparkeList.Count; i++) {

            if (contactSparkeList[i].isPlaying)
                return;

            contactSparkeList[i].transform.position = contactPoint;
            ParticleSystem.EmissionModule em = contactSparkeList[i].emission;
            em.enabled = true;
            contactSparkeList[i].Play();

        }

    }

    private void OnCollisionEnter(Collision col) {

        if (col.relativeVelocity.magnitude < 2.5f)
            return;

        if (crashSounds.Length > 0) {

            crashSoundSource = RHC_CreateAudioSource.NewAudioSource(gameObject, "Crash Sound", 5f, 1f, crashSounds[Random.Range(0, crashSounds.Length)], false, false, true);
            crashSoundSource.pitch = Random.Range(.85f, 1f);
            crashSoundSource.Play();

        }

        CollisionParticles(col.contacts[0].point);

        if (useDamage)
            damage.OnCollision(col);

    }

    private void OnDrawGizmos() {

        RaycastHit hit;
        stabilizerThrusters = GetComponentsInChildren<RHC_Thruster>();

        for (int i = 0; i < stabilizerThrusters.Length; i++) {

            if (stabilizerThrusters[i] != null) {

                if (Physics.Raycast(stabilizerThrusters[i].transform.position, stabilizerThrusters[i].transform.forward, out hit, stableHeight * 2f) && hit.transform.root != transform) {

                    Debug.DrawRay(stabilizerThrusters[i].transform.position, stabilizerThrusters[i].transform.forward * hit.distance, new Color(Mathf.Lerp(1f, 0f, hit.distance / (stableHeight * 1.5f)), Mathf.Lerp(0f, 1f, hit.distance / (stableHeight * 1.5f)), 0f, 1f));
                    Gizmos.color = new Color(Mathf.Lerp(1f, 0f, hit.distance / (stableHeight * 1.5f)), Mathf.Lerp(0f, 1f, hit.distance / (stableHeight * 1.5f)), 0f, 1f);
                    Gizmos.DrawSphere(hit.point, .5f);

                }

                Matrix4x4 temp = Gizmos.matrix;
                Gizmos.matrix = Matrix4x4.TRS(stabilizerThrusters[i].transform.position, stabilizerThrusters[i].transform.rotation, Vector3.one);
                Gizmos.DrawFrustum(Vector3.zero, 30f, 5f, .1f, 1f);
                Gizmos.matrix = temp;

            }

        }

    }

    private void OnDisable() {

        if (OnRHCDestroyed != null)
            OnRHCDestroyed();

        RHC_InputManager.OnHeadlights -= RHC_InputManager_OnHeadlights;

    }

    private void Reset() {

        if (Rigid == null)
            gameObject.AddComponent<Rigidbody>();

        Rigid.mass = 500f;
        Rigid.drag = .01f;
        Rigid.angularDrag = .5f;
        Rigid.interpolation = RigidbodyInterpolation.Interpolate;

        COM = transform.Find("COM");

        if (COM == null) {

            GameObject c = new GameObject("COM");
            c.transform.SetParent(transform, false);
            c.transform.localPosition = new Vector3(0f, -.5f, 0f);
            c.transform.localRotation = Quaternion.identity;
            COM = c.transform;

        }

        if (stabilizerThrusters.Length < 1) {

            stabilizerThrusters = new RHC_Thruster[2];

            for (int i = 0; i < stabilizerThrusters.Length; i++) {

                GameObject t = new GameObject("Thruster_" + i.ToString());
                t.transform.SetParent(transform, false);
                t.transform.localPosition = new Vector3(0f, -1f, i == 0 ? 1.5f : -1.5f);
                t.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
                t.AddComponent<RHC_Thruster>();
                stabilizerThrusters[i] = t.GetComponent<RHC_Thruster>();

            }

        }

    }

}
