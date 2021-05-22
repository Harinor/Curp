using ConditionalInspectorFields;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    public List<Modification> modifications = new List<Modification>();

    public bool WanderAround;
    [ConditionalField("WanderAround")] public float WanderDistance = 5;



    public void Initialize()
    {
        
    }
}
