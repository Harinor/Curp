using ConditionalInspectorFields;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    public string Name = "vehicle";
    public List<Modification> modifications = new List<Modification>();
    public Color currentColor;
}
