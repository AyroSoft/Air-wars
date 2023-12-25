using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuspensionWheel : MonoBehaviour {

    [SerializeField]
    private float suspensionDistance;
    [SerializeField]
    private float suspensionPower;

    private new Rigidbody rigidbody;
    private bool grounded;

    private void Start()
    {
        rigidbody = GetComponentInParent<Rigidbody>();
    }
    void FixedUpdate () {
        grounded = false;
        RaycastHit hit = new RaycastHit();
        if(Physics.Raycast(transform.position, -transform.up, out hit))
        {
            if(hit.distance < suspensionDistance)
            {
                float normalizedPower = (suspensionDistance - hit.distance)/suspensionDistance;
                float applyPower = suspensionPower * normalizedPower * Time.deltaTime;
                rigidbody.AddForceAtPosition(transform.up * applyPower, transform.position);
                grounded = true;
            }
        }
	}

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position - transform.up * suspensionDistance);
    }
    public bool IsGrounded()
    {
        return grounded;
    }
}
