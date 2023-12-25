//----------------------------------------------
//      Realistic Hovercraft Controller
//
// Copyright © 2015  - 2022 BoneCracker Games
// http://www.bonecrackergames.com
//
//----------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RHC_Inputs {

    public float throttleInput = 0f;
    public float brakeInput = 0f;
    public float steerInput = 0f;
    public float elevationInput = 0f;

    public float orbitX = 0f;
    public float orbitY = 0f;

}
