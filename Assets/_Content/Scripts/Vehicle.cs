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
    [Tooltip("The scale of the preview.")]
    public float previewScale = 1f;

    [Tooltip("The object to be toggled to turn the lights on/off.")]
    public GameObject lights;

    [Tooltip("The list of available modifications.")]
    public List<ModificationSlot> modificationSlots = new List<ModificationSlot>();

    [Tooltip("The provided information.")]
    public Information information;

    private Material bodyMatrial;

    /// <summary>
    /// Holds the main material of the vehicle. Changing the value calls synchronize method for modification that should share the material.
    /// </summary>
    public Material BodyMaterial
    {
        get { return bodyMatrial; }
        set 
        { 
            bodyMatrial = value; 
            UpdateVehicleMaterial();
            MasterManager.instance.TriggerOnActiveVehicleMaterialChangedEvent();
        }
    }


    #region --- METHODS ---
    public void UpdateVehicleMaterial()
    {
        BroadcastMessage("OnVehicleMaterialUpdated", SendMessageOptions.DontRequireReceiver);
    } 
    #endregion

    #region --- INNER CLASSES ---
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

    [System.Serializable]
    public class Information
    {
        /// <summary>
        /// Mainly for the inspector.
        /// </summary>
        [TextArea(3, 20)]
        public string description;
        public float price = 0;
    } 
    #endregion
}
