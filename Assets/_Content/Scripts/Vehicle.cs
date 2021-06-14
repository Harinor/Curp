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

    private Material bodyMaterial;

    /// <summary>
    /// Holds the main material of the vehicle. Changing the value calls synchronize method for modification that should share the material.
    /// </summary>
    public Material BodyMaterial
    {
        get { return bodyMaterial; }
        set 
        { 
            bodyMaterial = value; 
            UpdateVehicleMaterial();
            MasterManager.instance.TriggerOnActiveVehicleMaterialChangedEvent();
        }
    }

    #region --- UNITY CALLBACKS ---
    private void Start()
    {
        if (bodyMaterial == null)
        {
            bodyMaterial = GetDefaultMaterial();
        }
    } 
    #endregion

    #region --- METHODS ---
    public void UpdateVehicleMaterial()
    {
        BroadcastMessage("OnVehicleMaterialUpdated", SendMessageOptions.DontRequireReceiver);
    } 

    private Material GetDefaultMaterial()
    {
        Transform body = transform.Find("body");
        if (body != null && body.TryGetComponent(out Renderer renderer))
        {
            return renderer.material;
        }

        Renderer firstRenderer = GetComponentInChildren<Renderer>();
        if (firstRenderer != null )
        {
            return firstRenderer.material;
        }

        return null;
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
        public float price = 0;       
        /// <summary>
        /// Mainly for the inspector.
        /// </summary>
        [TextArea(3, 20)]
        public string description;
        public List<Specification> specifications;

        static readonly string stringSeparator = ":\t";
        [HideInInspector] public string[] stringSeparators = new string[] { stringSeparator };

        [System.Serializable]
        public class Specification
        {
            public string name;
            public string value;
            public string unit;
        }
    } 
    #endregion
}
