using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour {

    [SerializeField]
    private float timer = 1.5f;
    [SerializeField]
    private float trackingAccuracy = 5f;
    [SerializeField]
    private float speed = 200f;
    [SerializeField]
    private float drag = 2f;
    [SerializeField]
    private GameObject explosion;

    private new Rigidbody rigidbody;
    private bool lit = false;
    private Transform target;
    
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }
	public void Launch (Transform target) {
        rigidbody.isKinematic = false;
        rigidbody.velocity = GetComponentInParent<Rigidbody>().velocity;
        transform.SetParent(null);
        this.target = target;
        Invoke("Light", timer);
        Invoke("Hit", 30f);
        if(GetComponent<AudioSource>() != null)
        {
            GetComponent<AudioSource>().Play();
        }
    }
    private void Light()
    {
        lit = true;
        ParticleSystem[] systems = GetComponentsInChildren<ParticleSystem>();
        foreach(ParticleSystem system in systems)
        {
            system.Play();
        }
        rigidbody.useGravity = false;
        rigidbody.drag = drag;
    }
    void FixedUpdate()
    {
        if(lit)
        {
            if (target == null)
            {
                Hit();
            }
            else
            {
                Quaternion look = Quaternion.LookRotation(target.position - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, look, trackingAccuracy * Time.deltaTime);
                rigidbody.AddForce(transform.forward * speed);
            }
            
            
        }
        
    }
    void OnTriggerEnter(Collider col)
    {
        Hit();
    }
    void Hit()
    {
        Instantiate(explosion, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
