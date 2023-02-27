using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(BoxCollider))]
public class DroneController : DroneRigidBody
{
    /**
     * This is the primary script for each drone. This script contains the controller and the
     * implementation for the drone movement.
     */

    #region Variables
    [Header("Control Properties")]
    [SerializeField] protected float Kp, Ki, Kd;//Tuning Parameters

    [Header("Motor Properties")]
    private MotorController front_left, front_right, back_left, back_right;//Create Instance for Propellers
    protected int fl_rpm, fr_rpm, bl_rpm, br_rpm;//Rotor Speeds
    #endregion //Variables

    #region MainMethods
    private void Start()
    {
        //Search for the MotorController type and allote the correct propeller to each variable
        List<MotorController> motors = new List<MotorController>();
        motors = GetComponentsInChildren<MotorController>().ToList();
        foreach (var motor in motors)
        {
            if (motor.name == "FL")
                front_left = motor;
            else if (motor.name == "FR")
                front_right = motor;
            else if (motor.name == "BL")
                back_left = motor;
            else if (motor.name == "BR")
                back_right = motor;
        }
    }
    #endregion// MainMethods

    #region CustomMethods
    protected override void HandlePhysics()// Implementation for the function from the "DroneRigidBody.cs" script
    {
        ControlMotorSpeed(rb, fl_rpm, fr_rpm, bl_rpm, br_rpm);
    }
    protected virtual void ControlMotorSpeed(Rigidbody rb, int fl_rpm, int fr_rpm, int bl_rpm, int br_rpm)
    {
        front_left.set_rotor_speed(rb, fl_rpm);
        front_right.set_rotor_speed(rb, fr_rpm);
        back_left.set_rotor_speed(rb, bl_rpm);
        back_right.set_rotor_speed(rb, br_rpm);
    }


    //Convert Floats to Int for Rotor Speed
    private void set_drone_rotor_speed(float fl_speed, float fr_speed, float bl_speed, float br_speed)
    {
        fl_rpm = (int)fl_speed;
        fr_rpm = (int)fr_speed;
        bl_rpm = (int)bl_speed;
        br_rpm = (int)br_speed;
    }

    public void set_drone_location(Vector3 position)
    {
        //Design SlidingMode/Robust Controller Here!
        //Call the set_drone_rotor_speed here!
    }

    #endregion //CustomMethods
}
