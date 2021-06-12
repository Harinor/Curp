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

    public Renderer[] renderers;

    public void OnVehicleMaterialUpdated()
    {
        if (MasterManager.ActiveVehicle.BodyMaterial != null)
        {   
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
            renderers = gameObject.GetComponentsInChildren<Renderer>();
            MasterManager.instance.OnActiveVehicleMaterialChanged += OnVehicleMaterialUpdated;
            OnVehicleMaterialUpdated();
            StartCoroutine(RemoteUpdate());
        }
    }

    private void OnDisable()
    {
        if (remote)
        {
            MasterManager.instance.OnActiveVehicleMaterialChanged -= OnVehicleMaterialUpdated;
            StopAllCoroutines();
        }
    }

    IEnumerator RemoteUpdate()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            OnVehicleMaterialUpdated();
        }
    }
}
