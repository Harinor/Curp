using Cinemachine;
using ConditionalInspectorFields;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    [Tooltip("The name of the vehicle.")]
    public string Name = "vehicle";

    [Tooltip("The associated vehicle prefab for the selection menu.")]
    public GameObject previewPrefab;

    [Tooltip("The object to be toggled to turn the lights on/off.")]
    public GameObject lights;

    [Tooltip("The list of available modifications.")]
    public List<ModificationSlot> modificationSlots = new List<ModificationSlot>();




    [System.Serializable]
    public class ModificationSlot
    {
        /// <summary>
        /// Mainly for the inspector.
        /// </summary>
        public string modTitle;
        public Modification modification;
        public int activeVariant;
        public CinemachineVirtualCamera previewCamera;
    }

}
