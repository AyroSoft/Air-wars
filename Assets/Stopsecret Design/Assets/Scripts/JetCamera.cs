using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetCamera : MonoBehaviour
{
    public Transform target, head;
    public bool isHeadPosition;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            isHeadPosition = !isHeadPosition;
            
        }
        transform.position = isHeadPosition ? head.position : target.position;
    }
}
