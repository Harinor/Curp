using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharedData : MonoBehaviour
{
    public Vehicle activeVehicle;

    private void Awake()
    {
        if (activeVehicle == null)
        {
            activeVehicle = FindObjectOfType<Vehicle>();
        }
    }
}
