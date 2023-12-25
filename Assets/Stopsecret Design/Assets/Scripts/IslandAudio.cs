using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandAudio : MonoBehaviour
{
    public float altitude;
    public AudioSource lowAltitude, highAltitude;
    //The way this script works is is plays one audio clip at a 
    //lower altitude and fades it to another at a higher altitude.
    //This is based off wherever the Main Camera is
    void Update()
    {
        float camAlt = GameObject.FindGameObjectWithTag("MainCamera").transform.position.y;
        float currentAltNormalized = Mathf.Clamp(camAlt / altitude, 0f, 1f);
        lowAltitude.volume = 1f - currentAltNormalized;
        highAltitude.volume = currentAltNormalized;
    }
}
