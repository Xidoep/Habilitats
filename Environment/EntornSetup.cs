using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntornSetup : MonoBehaviour
{
    public PhysicMaterial dinamic;
    public PhysicMaterial estatic;
    public LayerMask layerMask;

    Rigidbody rb;
    Collider col;

    private void Awake()
    {
        Setup();
    }

    [ContextMenu("Setup")]
    void Setup()
    {
        foreach (var item in GetComponentsInChildren<Collider>())
        {
            item.material = estatic;
            item.gameObject.layer = gameObject.layer;
        }
        foreach (var item in GetComponentsInChildren<Rigidbody>())
        {
            item.interpolation = RigidbodyInterpolation.Interpolate;
            item.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            col = item.gameObject.GetComponent<Collider>();
            if (col != null) 
            {
                col.material = dinamic;
                if (col is MeshCollider) (col as MeshCollider).convex = true;
            }
            item.gameObject.AddComponent<Environment.Effector>().LayerMask = layerMask;
        }
        foreach (var item in GetComponentsInChildren<Animator>())
        {
            item.updateMode = AnimatorUpdateMode.AnimatePhysics;
            item.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
            rb = item.gameObject.AddComponent<Rigidbody>();
            if(rb != null)
            {
                rb.useGravity = false;
                rb.isKinematic = true;
            }
            col = item.gameObject.GetComponent<Collider>();
            if (col != null) col.material = dinamic;
        }
    }
}
