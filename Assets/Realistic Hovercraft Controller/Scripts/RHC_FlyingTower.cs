//----------------------------------------------
//      Realistic Hovercraft Controller
//
// Copyright © 2015  - 2022 BoneCracker Games
// http://www.bonecrackergames.com
//
//----------------------------------------------

using UnityEngine;
using System.Collections;

[AddComponentMenu("BoneCracker Games/Realistic Hovercraft Controller/Misc/RHC Flying Tower")]
public class RHC_FlyingTower : MonoBehaviour {

    public float rotatespeed = 0.2f;

    private void Update() {

        transform.Rotate(rotatespeed * Time.deltaTime * Vector3.up);

    }

}
