using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explode : MonoBehaviour {

    private Rigidbody rb;
    public float collideVelocity;
    public GameObject explosion;
    public float waterLevel = 13f;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(rb.velocity.magnitude > collideVelocity)
        {
            DestroyThis();
        }
    }
    void Update () {
        if (transform.position.y < waterLevel)
        {
            DestroyThis();
        }
	}
    void DestroyThis()
    {
        GetComponentInChildren<Camera>().transform.SetParent(null);
        Instantiate(explosion, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
