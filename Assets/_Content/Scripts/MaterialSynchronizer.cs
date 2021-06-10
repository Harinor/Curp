using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This component will keep the material of this GameObject (typically a modification) synchronized with the body of the ActiveVehicle.
/// </summary>
public class MaterialSynchronizer : MonoBehaviour
{
    [SerializeField, Tooltip("If checked will register for global events via MasterManager instead of receiving broadcast messages.")]
    bool remote = false;

    public void OnVehicleMaterialUpdated()
    {
        if (MasterManager.ActiveVehicle.BodyMaterial != null)
        {
            var renderers = GetComponentsInChildren<Renderer>();
            foreach (Renderer rend in renderers)
            {
                rend.material = MasterManager.ActiveVehicle.BodyMaterial;
            }
        }
    }

    private void OnEnable()
    {
        if (remote)
        {
            MasterManager.instance.OnActiveVehicleMaterialChanged += OnVehicleMaterialUpdated;
            OnVehicleMaterialUpdated();
        }
    }

    private void OnDisable()
    {
        if (remote)
        {
            MasterManager.instance.OnActiveVehicleMaterialChanged -= OnVehicleMaterialUpdated;
        }
    }
}
