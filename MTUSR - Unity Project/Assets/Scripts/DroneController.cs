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
    [Header("Drone Rigidbody Properties")]
    private Vector3 moment_of_inertia;
    [SerializeField] private float drone_arm_length;

    [Header("Propeller Rigidbody Properties")]
    [SerializeField] private float I_p;//Moment of Intertia of Propeller
    [SerializeField] private float K_f;//Propeller Thrust Factor
    [SerializeField] private float K_m;//Propeller Moment Factor
    [SerializeField] private float rpm_to_thrust_ratio;

    [Header("Tuning Properties")]
    [SerializeField] private float lambda_y, lambda_rpy_x, lambda_rpy_z, lambda_rpy_y; //Sliding Mode Tuning Parameter
    [SerializeField] private float k_y, k_rpy_x, k_rpy_z, k_rpy_y; //Sliding Mode Tuning Parameter
    [SerializeField] private float kp, kd; // Parameter

    [Header("Motor Properties")]
    private MotorController front_left, front_right, back_left, back_right;//Create Instance for Propellers

    [Header("Position Properties")]
    private Vector3 drone_set_position;
    private Vector3 drone_set_velocity;
    private Vector3 drone_set_acceleration;

    [Header("System Properties")]
    private float gravity;
    private float omega = 0;
    private float aa, bb, cc;//For calculating allocation matrix
    #endregion //Variables

    #region MainMethods
    private void Start()
    {
        //Search for the MotorController type and allote the correct propeller to each variable
        List<MotorController> motors = GetComponentsInChildren<MotorController>().ToList();
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
        gravity = Physics.gravity.magnitude;
        if (!rb)
            while (true)
                Debug.Log("Inertia Information Missing");

        moment_of_inertia = rb.inertiaTensor;
        front_left.lift_force_per_rpm = rpm_to_thrust_ratio;
        front_right.lift_force_per_rpm = rpm_to_thrust_ratio;
        back_left.lift_force_per_rpm = rpm_to_thrust_ratio;
        back_right.lift_force_per_rpm = rpm_to_thrust_ratio;

        aa = 1 / (4 * K_f);
        bb = Mathf.Sqrt(2) / (4 * K_f * drone_arm_length);
        cc = 1 / (4 * K_f * K_m);
    }
    #endregion// MainMethods

    #region CustomMethods
    protected override void HandlePhysics()// Implementation for the function from the "DroneRigidBody.cs" script
    {
        Sliding_mode_controller(rb, drone_set_position, drone_set_velocity, drone_set_acceleration);
    }

    private void Sliding_mode_controller(Rigidbody rb, Vector3 desired_position_in_meters, Vector3 desired_velocity_in_meter_per_seconds, Vector3 desired_acceleration_in_meters_per_seconds_square)
    {
        int fl_rpm, fr_rpm, bl_rpm, br_rpm;

        //Desired Pose
        Vector3 desired_position = desired_position_in_meters;
        Vector3 desired_velocity = desired_velocity_in_meter_per_seconds;
        Vector3 desired_acceleration = desired_acceleration_in_meters_per_seconds_square;
        Vector3 desired_rpy = Vector3.zero;
        Vector3 desired_drpy = Vector3.zero;
        Vector3 desired_ddrpy = Vector3.zero;

        //Current Pose
        Vector3 position = rb.position;
        Vector3 velocity = rb.velocity;
        Vector3 rpy = rb.rotation.eulerAngles;//x:roll->phi, y:pitch->theta, z:yaw->psi
        Vector3 drpy = rb.angularVelocity;

        // Sliding Mode Controller For U1
        float de1 = velocity.y - drone_set_velocity.y;
        float e1 = position.y - drone_set_position.y;
        int s1 = Signum_function(de1 + (lambda_y * e1));
        float u1 = -(rb.mass / (Mathf.Cos(rpy.x) * Mathf.Cos(rpy.z))) * (-gravity - desired_acceleration.z + (lambda_y * de1) + (k_y * s1));

        //Checking for Translational Turbulance caused by motion and correcting it.
        Vector3 position_error = position - desired_position;
        Vector3 velocity_error = velocity - desired_velocity;
        float F_x = rb.mass * ((-kp * position_error.x) - (kd * velocity_error.x) + desired_acceleration.x);
        float F_z = rb.mass * ((-kp * position_error.z) - (kd * velocity_error.z) + desired_acceleration.z);
        desired_rpy.y = Mathf.Asin(F_x / u1);
        desired_rpy.x = Mathf.Asin(-(F_z / u1));

        // Sliding Mode Controller for U2 
        float de2 = Wrap_angle(drpy.x - desired_drpy.x);
        float e2 = Wrap_angle(rpy.x - desired_rpy.x);
        int s2 = Signum_function(de2 + (lambda_rpy_x * e2));
        float u2 = -((drpy.z * drpy.y * (moment_of_inertia.z - moment_of_inertia.y)) - (I_p * omega * drpy.z) - (moment_of_inertia.x * desired_ddrpy.x) + (lambda_rpy_x * moment_of_inertia.x * de2) + (moment_of_inertia.x * k_rpy_x * s2));

        // Sliding Mode Controller for U3
        float de3 = Wrap_angle(drpy.z - desired_drpy.z);
        float e3 = Wrap_angle(rpy.z - desired_rpy.z);
        int s3 = Signum_function(de3 + (lambda_rpy_z * e3));
        float u3 = -((drpy.x * drpy.y * (moment_of_inertia.y - moment_of_inertia.x)) + (I_p * omega * drpy.x) - (moment_of_inertia.z * desired_ddrpy.z) + (lambda_rpy_z * moment_of_inertia.y * de3) + (moment_of_inertia.z * k_rpy_z * s3));

        // Sliding Mode Controller for U4
        float de4 = Wrap_angle(drpy.y - desired_drpy.y);
        float e4 = Wrap_angle(rpy.y - desired_rpy.y);
        int s4 = Signum_function(de4 + (lambda_rpy_y * e4));
        float u4 = -((drpy.x * drpy.z * (moment_of_inertia.x - moment_of_inertia.z)) - (moment_of_inertia.y * desired_ddrpy.y) + (lambda_rpy_x * moment_of_inertia.y * de4) + (moment_of_inertia.y * k_rpy_y * s4));

        //Allocation Matrix
        int w1_sq = (int)((aa * u1) - (bb * u2) - (bb * u3) - (cc * u4));
        int w2_sq = (int)((aa * u1) - (bb * u2) + (bb * u3) + (cc * u4));
        int w3_sq = (int)((aa * u1) + (bb * u2) + (bb * u3) - (cc * u4));
        int w4_sq = (int)((aa * u1) + (bb * u2) - (bb * u3) + (cc * u4));

        fl_rpm = (int)Mathf.Abs(Mathf.Sqrt(w1_sq));
        fr_rpm = (int)Mathf.Abs(Mathf.Sqrt(w2_sq));
        bl_rpm = (int)Mathf.Abs(Mathf.Sqrt(w4_sq));
        br_rpm = (int)Mathf.Abs(Mathf.Sqrt(w3_sq));

        omega = fl_rpm - fr_rpm + bl_rpm - br_rpm;
        Set_drone_rotor_speed(fl_rpm, fr_rpm, bl_rpm, br_rpm);
    }

    private float Wrap_angle(float angle)
    {
        return (int)((angle + 360) % (2 * 360) - 360);
    }

    private int Signum_function(float s)
    {
        if (s > 0)
            return 1;
        else if (s < 0)
            return -1;
        else
            return 0;
    }

    //Set Rotor Speed
    private void Set_drone_rotor_speed(int fl_rpm, int fr_rpm, int bl_rpm, int br_rpm)
    {
        front_left.Set_rotor_speed(rb, fl_rpm);
        front_right.Set_rotor_speed(rb, fr_rpm);
        back_left.Set_rotor_speed(rb, bl_rpm);
        back_right.Set_rotor_speed(rb, br_rpm);
    }

    public void Set_drone_Kinematics(Vector3 position, Vector3 velocity, Vector3 acceleration)
    {
        drone_set_position = position;
        drone_set_velocity = velocity;
        drone_set_acceleration = acceleration;
    }

    #endregion //CustomMethods
}
