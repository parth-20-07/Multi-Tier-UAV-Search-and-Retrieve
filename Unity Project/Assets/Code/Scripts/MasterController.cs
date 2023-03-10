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

    UnityEngine.Vector3 Traj_evaluate(float t)
    {
        int t0, tf, x0, y0, z0, xf, yf, zf, v0, vf, ac0, acf;

        if (t <= 70)
        {
            if (t <= 5)
            {
                UnityEngine.Vector3 pos = new(0, (0.08 * Mathf.Pow(t, 3)) - (0.024 * Mathf.Pow(t, 4)) + (0.0019 * Mathf.Pow(t, 5)), 0);


                t0 = 0; tf = 5; x0 = 0; y0 = 0; z0 = 0; xf = 0; yf = 0; zf = 1; v0 = 0; vf = 0; ac0 = 0; acf = 0;
                Debug.Log("Lift OFF - T:" + Mathf.Round(t));
            }
            else if (t <= 20)
            {
                t0 = 5; tf = 20; x0 = 0; y0 = 0; z0 = 1; xf = 1; yf = 0; zf = 1; v0 = 0; vf = 0; ac0 = 0; acf = 0;
                Debug.Log("Path 1 - T:" + Mathf.Round(t));
            }
            else if (t <= 35)
            {
                t0 = 20; tf = 35; x0 = 1; y0 = 0; z0 = 1; xf = 1; yf = 1; zf = 1; v0 = 0; vf = 0; ac0 = 0; acf = 0;
                Debug.Log("Path 2 - T:" + Mathf.Round(t));
            }
            else if (t <= 50)
            {
                t0 = 35; tf = 50; x0 = 1; y0 = 1; z0 = 1; xf = 0; yf = 1; zf = 1; v0 = 0; vf = 0; ac0 = 0; acf = 0;
                Debug.Log("Path 3 - T:" + Mathf.Round(t));
            }
            else if (t <= 65)
            {
                t0 = 50; tf = 65; x0 = 0; y0 = 1; z0 = 1; xf = 0; yf = 0; zf = 1; v0 = 0; vf = 0; ac0 = 0; acf = 0;
                Debug.Log("Path 4 - T:" + Mathf.Round(t));
            }
            else
            {
                t0 = 65; tf = 70; x0 = 0; y0 = 0; z0 = 1; xf = 0; yf = 0; zf = 0; v0 = 0; vf = 0; ac0 = 0; acf = 0;
                Debug.Log("Path 5 - T:" + Mathf.Round(t));
            }
        }
        else
        {
            t0 = 65; tf = 70; x0 = 0; y0 = 0; z0 = 1; xf = 0; yf = 0; zf = 0; v0 = 0; vf = 0; ac0 = 0; acf = 0;
            Debug.Log("Path 5 - T:" + Mathf.Round(t));
        }
        #endregion//CustomMethod
    }
}