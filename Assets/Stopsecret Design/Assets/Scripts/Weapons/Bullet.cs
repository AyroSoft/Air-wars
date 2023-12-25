using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
    public float speed, gravity, gravityadd, deviance;
    private Vector3 dir;
    public float damageAmount;
    public GameObject remnant;
    public LayerMask hitMatrix;
    // Use this for initialization
    void Start()
    {
        Vector3 deviant = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * deviance;
        //initial velocity
        AddVelocity(transform.forward*speed + deviant);
    }

    // Update is called once per frame
    public void AddVelocity(Vector3 amt)
    {
        dir += amt;
    }
    void Update()
    {
        //Apply gravity
        gravity += gravityadd * Time.deltaTime;
        AddVelocity(new Vector3(0, -gravity * Time.deltaTime, 0));

        //point into the wind
        transform.forward = dir;

        //Check if we hit anything this frame
        RaycastHit hit = new RaycastHit();
        if (!Physics.Raycast(transform.position, dir, out hit, dir.magnitude * Time.deltaTime, hitMatrix.value))
        {
            transform.position += dir * Time.deltaTime;
        }
        else
        {
            hit.transform.SendMessageUpwards("Damage", damageAmount, SendMessageOptions.DontRequireReceiver);
            if (remnant != null)
            {
                Instantiate(remnant, hit.point, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }
    void InheritVelocity(Vector3 velo)
    {
        dir += velo;
    }
}
