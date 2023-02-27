using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterController : MonoBehaviour
{
    #region Variables
    [Header("Drones")]
    [SerializeField] DroneController drone;//Create an instance of the drone here!

    #endregion// Variables

    #region MainMethod
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = new Vector3(0, 0, 0);
        drone.set_drone_location(pos);//Set drone position here!
    }
    #endregion //MainMethod
}
