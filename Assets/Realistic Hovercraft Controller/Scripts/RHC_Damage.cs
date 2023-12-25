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
public class RHC_Damage {

    [HideInInspector] public bool initialized = false;

    private RHC_HovercraftController hoverController;

    // Mesh deformation
    [Space()]
    [Header("Mesh Deformation")]
    public DeformationMode deformationMode;
    public enum DeformationMode { Accurate, Fast }
    public LayerMask damageFilter = -1;     // LayerMask filter. Damage will be taken from the objects with these layers.
    public float damageRadius = .5f;        // Verticies in this radius will be effected on collisions.
    private float minimumVertDistanceForDamagedMesh = .002f;        // Comparing Original Vertex Positions Between Last Vertex Positions To Decide Mesh Is Repaired Or Not.
    public float damageMultiplier = 1f;     // Damage multiplier.
    public float maximumDamage = .5f;       // Maximum Vert Distance For Limiting Damage. 0 Value Will Disable The Limit.
    private readonly float minimumCollisionImpulse = .5f;       // Minimum collision force.

    public struct OriginalMeshVerts { public Vector3[] meshVerts; }     // Struct for Original Mesh Verticies positions.
    public struct OriginalWheel { public Vector3 wheelPosition; public Quaternion wheelRotation; }

    public OriginalMeshVerts[] originalMeshData;        // Array for struct above.
    public OriginalMeshVerts[] damagedMeshData;     // Array for struct above.

    [Space()]
    public bool repairNow = false;      // Repairing now.
    [HideInInspector] public bool repaired = true;        // Returns true if vehicle is completely repaired.
    private bool deforming = false;      //  Deforming the mesh now.
    private bool deformed = true;        //  Returns true if vehicle is completely deformed.
    private float deformationTime = 0f;     //  Timer for deforming the vehicle. 

    [Space()]
    public bool recalculateNormals = true;      //  Recalculate normals while deforming / restoring the mesh.
    public bool recalculateBounds = true;       //  Recalculate bounds while deforming / restoring the mesh.

    [Space()]
    public MeshFilter[] meshFilters;    //  Collected mesh filters.

    /// <summary>
    /// Collecting all meshes and detachable parts of the vehicle.
    /// </summary>
    public void Initialize(RHC_HovercraftController _hoverController, MeshFilter[] allMeshFilters) {

        //  Getting the main car controller.
        hoverController = _hoverController;

        // If you have not setted up the deformable mesh filters, get all mesh filters.
        if (meshFilters.Length == 0) {

            List<MeshFilter> properMeshFilters = new List<MeshFilter>();

            // Model import must be readable. If it's not readable, inform the developer. We don't wanna deform wheel meshes. Exclude any meshes belongts to the wheels.
            foreach (MeshFilter mf in allMeshFilters) {

                if (!mf.mesh.isReadable)
                    Debug.LogError("Not deformable mesh detected. Mesh of the " + mf.transform.name + " isReadable is false; Read/Write must be enabled in import settings for this model!");
                else
                    properMeshFilters.Add(mf);

            }

            //  Assigning deformable mesh filters.
            meshFilters = properMeshFilters.ToArray();

        }

        //  We will be using two structs for deformed sections. Original part struction, and deformed part struction. 
        //  All damaged meshes and wheel transforms will be using these structs. At this section, we're creating them with original struction.
        originalMeshData = new OriginalMeshVerts[meshFilters.Length];

        for (int i = 0; i < meshFilters.Length; i++)
            originalMeshData[i].meshVerts = meshFilters[i].mesh.vertices;

        damagedMeshData = new OriginalMeshVerts[meshFilters.Length];

        for (int i = 0; i < meshFilters.Length; i++)
            damagedMeshData[i].meshVerts = meshFilters[i].mesh.vertices;

        initialized = true;

    }

    /// <summary>
    /// Moving deformed vertices to their original positions while repairing.
    /// </summary>
    public void Repair() {

        if (!initialized)
            return;

        //  If vehicle is not repaired completely, and repairNow is enabled, restore all deformed meshes to their original structions.
        if (!repaired && repairNow) {

            int k;
            repaired = true;

            //  If deformable mesh is still exists, get all verticies of the mesh first. And then move all single verticies to the original positions. If verticies are close enough to the original
            //  position, repaired = true;
            for (k = 0; k < meshFilters.Length; k++) {

                if (meshFilters[k] != null) {

                    //  Get all verticies of the mesh first.
                    Vector3[] vertices = meshFilters[k].mesh.vertices;

                    for (int i = 0; i < vertices.Length; i++) {

                        //  And then move all single verticies to the original positions
                        if (deformationMode == DeformationMode.Accurate)
                            vertices[i] += (originalMeshData[k].meshVerts[i] - vertices[i]) * (Time.deltaTime * 5f);
                        else
                            vertices[i] += (originalMeshData[k].meshVerts[i] - vertices[i]);

                        //  If verticies are close enough to their original positions, repaired = true;
                        if ((originalMeshData[k].meshVerts[i] - vertices[i]).magnitude >= minimumVertDistanceForDamagedMesh)
                            repaired = false;

                    }

                    //  We were using the variable named "vertices" above, therefore we need to set the new verticies to the damaged mesh data.
                    //  Damaged mesh data also restored while repairing with this proccess.
                    damagedMeshData[k].meshVerts = vertices;

                    //  Setting new verticies to the all meshes. Recalculating normals and bounds, and then optimizing. This proccess can be heavy for high poly meshes.
                    //  You may want to disable last three lines.
                    meshFilters[k].mesh.SetVertices(vertices);

                    if (recalculateNormals)
                        meshFilters[k].mesh.RecalculateNormals();

                    if (recalculateBounds)
                        meshFilters[k].mesh.RecalculateBounds();

                }

            }

            //  If all meshes are completely restored, make sure repairing now is false.
            if (repaired)
                repairNow = false;

        }

    }

    /// <summary>
    /// Moving vertices of the collided meshes to the damaged positions while deforming.
    /// </summary>
    public void Damage() {

        if (!initialized)
            return;

        //  If vehicle is not deformed completely, and deforming is enabled, deform all meshes to their damaged structions.
        if (!deformed && deforming) {

            int k;
            deformed = true;
            deformationTime += Time.deltaTime;

            //  If deformable mesh is still exists, get all verticies of the mesh first. And then move all single verticies to the damaged positions. If verticies are close enough to the original
            //  position, deformed = true;
            for (k = 0; k < meshFilters.Length; k++) {

                if (meshFilters[k] != null) {

                    //  Get all verticies of the mesh first.
                    Vector3[] vertices = meshFilters[k].mesh.vertices;

                    //  And then move all single verticies to the damaged positions.
                    for (int i = 0; i < vertices.Length; i++) {

                        if (deformationMode == DeformationMode.Accurate)
                            vertices[i] += (damagedMeshData[k].meshVerts[i] - vertices[i]) * (Time.deltaTime * 5f);
                        else
                            vertices[i] += (damagedMeshData[k].meshVerts[i] - vertices[i]);

                    }

                    //  Setting new verticies to the all meshes. Recalculating normals and bounds, and then optimizing. This proccess can be heavy for high poly meshes.
                    //  You may want to disable last three lines.
                    meshFilters[k].mesh.SetVertices(vertices);

                    if (recalculateNormals)
                        meshFilters[k].mesh.RecalculateNormals();

                    if (recalculateBounds)
                        meshFilters[k].mesh.RecalculateBounds();

                }

            }

            //  Make sure deforming proccess takes only 1 second.
            if (deformationTime <= 1f)
                deformed = false;

            //  If all meshes are completely deformed, make sure deforming is false and timer is set to 0.
            if (deformed) {

                deforming = false;
                deformationTime = 0f;

            }

        }

    }

    /// <summary>
    /// Deforming meshes.
    /// </summary>
    /// <param name="collision"></param>
    /// <param name="impulse"></param>
    private void DamageMesh(Collision collision, float impulse) {

        for (int i = 0; i < meshFilters.Length; i++) {

            if (meshFilters[i] != null && meshFilters[i].gameObject.activeSelf) {

                Vector3[] vertices = damagedMeshData[i].meshVerts;

                ContactPoint[] contactPoints = new ContactPoint[collision.contactCount];
                collision.GetContacts(contactPoints);

                foreach (ContactPoint contactPoint in contactPoints) {

                    Vector3 collisionDirection = contactPoint.point - hoverController.transform.position;
                    collisionDirection = -collisionDirection.normalized;

                    Vector3 point = meshFilters[i].transform.InverseTransformPoint(contactPoint.point);

                    for (int k = 0; k < vertices.Length; k++) {

                        if ((point - vertices[k]).magnitude < damageRadius) {

                            deforming = true;
                            deformed = false;

                            vertices[k] += (hoverController.transform.InverseTransformDirection(collisionDirection) * impulse * (damageMultiplier / 50f));

                            if (maximumDamage > 0 && ((vertices[k] - originalMeshData[i].meshVerts[k]).magnitude) > maximumDamage)
                                vertices[k] = originalMeshData[i].meshVerts[k] + (vertices[k] - originalMeshData[i].meshVerts[k]).normalized * (maximumDamage);

                        }

                    }

                }

            }

        }

    }

    /// <summary>
    /// Raises the collision enter event.
    /// </summary>
    /// <param name="collision">Collision.</param>
    public void OnCollision(Collision collision) {

        if (!initialized)
            return;

        if (!hoverController.useDamage)
            return;

        if (((1 << collision.gameObject.layer) & damageFilter) != 0) {

            float impulse = collision.impulse.magnitude / 10000f;

            if (impulse < minimumCollisionImpulse)
                impulse = 0f;

            if (impulse > 10000f)
                impulse = 10000f;

            if (impulse > 0f) {

                repairNow = false;
                repaired = false;

                DamageMesh(collision, impulse);

            }

        }

    }

}
