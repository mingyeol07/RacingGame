using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCar : MonoBehaviour
{
    [SerializeField] private WheelCollider[] wheels = new WheelCollider[4];
    [SerializeField] private GameObject[] wheelMeshes = new GameObject[4];
    [SerializeField] private float motorTorque = 300f;
    [SerializeField] private float brakePower = 1000f;
    [SerializeField] private float steeringMaxAngle = 45f;

    private void FixedUpdate()
    {
        Move();
        steerCar();
    }

    private void Move()
    {
        for (int i = 0; i < wheels.Length; i++)
        {
            wheels[i].motorTorque = motorTorque * Input.GetAxis("Vertical");
        }
    }

    private void steerCar()
    {
        for (int i = 0; i < 2; i++)
        {
            wheels[i].steerAngle = steeringMaxAngle * Input.GetAxis("Horizontal");
        }
    }
}
