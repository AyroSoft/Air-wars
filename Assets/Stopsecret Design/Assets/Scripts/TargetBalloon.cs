using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TargetBalloon : MonoBehaviour {


    [SerializeField]
    private GameObject balloon, balloonString, explosion;

    private bool exploded = false;


	void OnTriggerEnter (Collider other) {
        other.SendMessage("Hit", SendMessageOptions.DontRequireReceiver);
        Explode();
	}
    void Damage(float amt)
    {
        Explode();
    }
	void Explode()
    {
        Instantiate(explosion, balloon.transform.position, balloon.transform.rotation);
        Destroy(balloon);
        exploded = true;
    }
	void Update () {
		if(exploded)
        {
            balloonString.transform.position -= Vector3.up * Time.deltaTime*9.81f;
        }
	}
}
