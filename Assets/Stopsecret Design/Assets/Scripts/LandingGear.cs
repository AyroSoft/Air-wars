using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingGear : MonoBehaviour {
    private Quaternion originalAngle;
    public Vector3 gearUpAngle;
    public float gearUpHeight = 50f, transitionAngle = 90f;
	// Use this for initialization
	void Start () {
        originalAngle = transform.localRotation;
	}
	
	// Update is called once per frame
	void Update () {
        Quaternion moveTowards = (transform.position.y > gearUpHeight) ? Quaternion.Euler(gearUpAngle) : originalAngle;
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, moveTowards, transitionAngle*Time.deltaTime);
	}
}
