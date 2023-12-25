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
using UnityEditor;

[CustomEditor(typeof(RHC_HovercraftController))]
public class RHC_Editor : Editor {

    private RHC_HovercraftController prop;

    public override void OnInspectorGUI() {

        prop = (RHC_HovercraftController)target;
        serializedObject.Update();

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Torques", MessageType.None);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("motorTorque"), false);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("steerTorque"), false);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("brakeTorque"), false);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("motorTorqueCurve"), false);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("autoCreateMotorTorqueCurve"), false);
        EditorGUI.indentLevel--;
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("COM"), false);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("canControl"), false);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("engineRunning"), false);
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Thrusters", MessageType.None);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("thrusterSpring"), false);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("thrusterDamp"), false);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("stableHeight"), false);
        EditorGUI.indentLevel--;
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("maximumSpeed"), false);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("maximumAngularVelocity"), false);
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Input Sensitivities", MessageType.None);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("throttleSensitivity"), false);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("steeringSensitivity"), false);
        EditorGUI.indentLevel--;
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Audio Clips", MessageType.None);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("engineSound"), false);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("crashSounds"), true);
        EditorGUI.indentLevel--;
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Damage", MessageType.None);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("contactSparkle"), false);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("useDamage"), false);
        EditorGUI.indentLevel--;

        if (prop.useDamage) {

            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("damage"), true);
            EditorGUI.indentLevel--;

        }

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Head Lights", MessageType.None);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("headlightsOn"), false);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("headLights"), true);
        EditorGUI.indentLevel--;
        EditorGUILayout.Space();

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Particles", MessageType.None);
        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("boosterParticles"), true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("thrusterParticles"), true);
        EditorGUI.indentLevel--;
        EditorGUILayout.Space();

        CheckUp();

        if (prop.autoCreateMotorTorqueCurve)
            CheckMotorTorqueCurve();

        if (GUI.changed)
            EditorUtility.SetDirty(prop);

        serializedObject.ApplyModifiedProperties();

    }

    private void CheckMotorTorqueCurve() {

        if (prop.motorTorqueCurve == null)
            prop.motorTorqueCurve = new AnimationCurve();

        if (prop.motorTorqueCurve.length < 2)
            prop.motorTorqueCurve = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(prop.maximumSpeed, .1f));

    }

    private void CheckUp() {

        Collider[] colliders = prop.gameObject.GetComponentsInChildren<Collider>();

        bool bodyColliderFound = false;

        foreach (Collider collider in colliders) {

            if (collider is MeshCollider || collider is BoxCollider)
                bodyColliderFound = true;

        }

        if (!bodyColliderFound)
            EditorGUILayout.HelpBox("Body collider not found, please add a body collider to your vehicle!", MessageType.Error);

        if (prop.COM == null) {

            prop.COM = prop.transform.Find("COM");

            if (prop.COM == null) {

                GameObject c = new GameObject("COM");
                c.transform.SetParent(prop.transform, false);
                c.transform.localPosition = new Vector3(0f, -.5f, 0f);
                c.transform.localRotation = Quaternion.identity;
                prop.COM = c.transform;

            }

        }

    }

}
