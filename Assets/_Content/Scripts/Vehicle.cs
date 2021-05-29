using ConditionalInspectorFields;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    [Tooltip("The name of the vehicle.")]
    public string Name = "vehicle";
    [Tooltip("The list of available modifications.")]
    public List<Modification> modifications = new List<Modification>();
    [Tooltip("The object to be toggled to turn the lights on/off.")]
    public GameObject lights;
    public Color currentColor;
    [Tooltip("The list of available modifications.")]
    public List<ModificationSlot> modificationSlots = new List<ModificationSlot>();

    [System.Serializable]
    public class ModificationSlot
    {
        public Modification modification;
        public int activeVariant;
    }

}
