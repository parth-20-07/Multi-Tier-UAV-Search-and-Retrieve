using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;


[RequireComponent(typeof(BoxCollider))]

public class MotorController : MonoBehaviour, IEngine
{
    /**
     * Design and develop behaviour of each of the motors in the drones.
     * This class contains the code for controlling the rotor speed and how the rotor speed
     * will generate lift for the drone in the simulation.
     */

    #region Variables
    [Header("Motor Properties")]
    protected int max_motor_speed = 1200;//Max Possible RPM of the motor

    [Header("Propeller Properties")]
    private Transform propeller;//GameObject Propeller on which the bounding box and the force is applied
    [SerializeField] protected float lift_force_per_rpm = 0.1f;//Ratio of the lift vs rotor rpm. Actually it is a curve but is considered straight line for simplicity
    #endregion //Variables



    #region CustomMethods
    private void Awake()
    {
        propeller = GetComponent<Transform>();//Search for the propeller GameObject
    }

    public void set_rotor_speed(Rigidbody rb, int rotor_speed_in_rpm)
    {
        //Limit the Rotor speed in a range
        if (rotor_speed_in_rpm > max_motor_speed)
            rotor_speed_in_rpm = max_motor_speed;
        else if (rotor_speed_in_rpm < 0)
            rotor_speed_in_rpm = 0;

        //applied_force = mg - F, m = rb.mass/4 because of 4 seperate rotors
        float rotor_force = -((rb.mass / 4) * Physics.gravity.magnitude) + (rotor_speed_in_rpm * lift_force_per_rpm);
        if (rotor_force < 0)
            rotor_force = 0;
        set_motor_rotation_speed(rb, rotor_force, rotor_speed_in_rpm);
    }

    private void set_motor_rotation_speed(Rigidbody rb, float force, int rotor_speed_in_rpm)
    {
        if (!propeller)
            return;

        //Apply Rotor force and generate the rotation for animation
        Vector3 rotor_force = new Vector3(0, force, 0);
        rb.AddRelativeForce(rotor_force, ForceMode.Force);
        propeller.Rotate(Vector3.up, rotor_speed_in_rpm);
    }

    #endregion //CustomMethods
}
