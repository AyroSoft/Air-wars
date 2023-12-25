//----------------------------------------------
//      Realistic Hovercraft Controller
//
// Copyright © 2015  - 2022 BoneCracker Games
// http://www.bonecrackergames.com
//
//----------------------------------------------

using UnityEngine;
using System.Collections;

public class RHC_CreateAudioSource : MonoBehaviour {

    public static AudioSource NewAudioSource(GameObject go, string audioName, float minDistance, float volume, AudioClip audioClip, bool loop, bool playNow, bool destroyAfterFinished) {

        GameObject audioSource = new GameObject(audioName);
        audioSource.transform.position = go.transform.position;
        audioSource.transform.rotation = go.transform.rotation;
        audioSource.transform.SetParent(go.transform);

        AudioSource aS = audioSource.AddComponent<AudioSource>();
        aS.minDistance = minDistance;
        aS.volume = volume;
        aS.clip = audioClip;
        aS.loop = loop;
        aS.spatialBlend = 1f;

        if (playNow)
            aS.Play();

        if (destroyAfterFinished) {

            if (audioClip)
                Destroy(audioSource, audioClip.length);
            else
                Destroy(audioSource);

        }

        audioSource.transform.SetParent(go.transform);

        return audioSource.GetComponent<AudioSource>();

    }

}
