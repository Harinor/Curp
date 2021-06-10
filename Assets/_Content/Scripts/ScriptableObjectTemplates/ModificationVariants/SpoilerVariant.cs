using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpoilerVariant", menuName = "Variants/SpoilerVariant", order = 1)]
public class SpoilerVariant : ModificationVariant
{
    [SerializeField] GameObject spoiler;
    [SerializeField] string spoilerSlot;

    [SerializeField, Tooltip("Whether the color should match the color of the body.")]
    bool syncBodyColor = false;

    public override void Apply()
    {
        base.Apply();
        Transform slot = MasterManager.ActiveVehicle.transform.Find(spoilerSlot);
        foreach (Transform child in slot)
        {
            Destroy(child.gameObject);
        }
        if (spoiler)
        {
            Instantiate(spoiler, slot);
            if (spoiler != null && syncBodyColor && !spoiler.TryGetComponent(out MaterialSynchronizer _))
            {
                spoiler.AddComponent<MaterialSynchronizer>();
                MasterManager.ActiveVehicle.UpdateVehicleMaterial();
            }
        }        
    }

}


