using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEngine
{
    /**
     * Interface Function for each of the motors, this will force the motor controller script
     * to write the implementation of the essential functions for proper functioning of a motor
     * in simulation and force production
     */
    void set_rotor_speed(Rigidbody rb, int rotor_speed_in_rpm);//Provide implementation for this function
}