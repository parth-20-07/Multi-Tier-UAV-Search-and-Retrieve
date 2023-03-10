using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
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
        UnityEngine.Vector3 pos = new UnityEngine.Vector3(0, 0, 0);

        float t = Time.realtimeSinceStartup;

        drone.Set_drone_Kinematics(pos, pos, pos);
    }
    #endregion //MainMethod

    #region CustomMethod

    UnityEngine.Vector3[] Traj_evaluate(float t, Mathf mathf)
    {
        UnityEngine.Vector3 pos = UnityEngine.Vector3.zero;
        UnityEngine.Vector3 vel = UnityEngine.Vector3.zero;
        UnityEngine.Vector3 acc = UnityEngine.Vector3.zero;

        float px, py, pz, vx, vy, vz, ax, ay, az;
        if (t <= 70)
        {
            if (t <= 5)
            {
                px = 0;
                py = (float)((0.08 * Math.Pow(t, 3)) - (0.024 * Math.Pow(t, 4)) + (0.0019 * Math.Pow(t, 5)));
                pz = 0;
                vx = 0;
                vy = (float)((0.024 * Math.Pow(t, 2)) - (0.096 * Math.Pow(t, 3)) + (0.0096 * Math.Pow(t, 4)));
                vz = 0;
                ax = 0;
                ay = (float)((0.48 * t) - (0.288 * Math.Pow(t, 2)) + (0.0348 * Math.Pow(t, 3)));
                az = 0;

                pos = new UnityEngine.Vector3(px, py, pz);
                vel = new UnityEngine.Vector3(vx, vy, vz);
                acc = new UnityEngine.Vector3(ax, ay, az);

                Debug.Log("Lift OFF - T:" + Mathf.Round(t));
            }
            else if (t <= 20)
            {
                Debug.Log("Path 1 - T:" + Mathf.Round(t));
            }
            else if (t <= 35)
            {
                Debug.Log("Path 2 - T:" + Mathf.Round(t));
            }
            else if (t <= 50)
            {
                Debug.Log("Path 3 - T:" + Mathf.Round(t));
            }
            else if (t <= 65)
            {
                Debug.Log("Path 4 - T:" + Mathf.Round(t));
            }
            else
            {
                Debug.Log("Path 5 - T:" + Mathf.Round(t));
            }
        }
        else
        {
            Debug.Log("Stable - T:" + Mathf.Round(t));
        }

        UnityEngine.Vector3[] return_val = { pos, vel, acc };
        return return_val;
    }
    #endregion//CustomMethod
}