using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class DroneRigidBody : MonoBehaviour
{
    #region Variables
    [Header("RigidBody Properties")]
    [SerializeField] private float weight_in_grams = 42f;
    const float g_to_kg = 0.001f;

    protected Rigidbody rb;
    private float startDrag;
    private float AngularDrag;
    #endregion //Variables

    #region MainMethods
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb)
        {
            rb.mass = weight_in_grams * g_to_kg;
            startDrag = rb.drag;
            AngularDrag = rb.angularDrag;
        }
    }

    private void FixedUpdate()
    {
        if (!rb)
            return;
        else
            HandlePhysics();
    }


    #endregion //MainMethods

    #region CustomMethods
    protected virtual void HandlePhysics() { }//Enforce other scipts to create method for this
    #endregion //CustomMethods
}
