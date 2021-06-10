using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "RoofscoopVariant", menuName = "Variants/RoofscoopVariant", order = 1)]
public class RoofscoopVariant : ModificationVariant
{
    [SerializeField] 
    GameObject roofScoop;

    [SerializeField] 
    string roofscoopSlot;

    [SerializeField, Tooltip("Whether the color should match the color of the body.")]
    bool syncBodyColor = false;

    public override void Apply()
    {
        base.Apply();
        Transform slot = MasterManager.ActiveVehicle.transform.Find(roofscoopSlot);

        foreach (Transform child in slot)
        {
            Destroy(child.gameObject);
        }

        if (roofScoop)
        {
            var newMod = Instantiate(roofScoop, slot);
            if (newMod != null && syncBodyColor && !newMod.TryGetComponent(out MaterialSynchronizer _) )
            {
                newMod.AddComponent<MaterialSynchronizer>();
                MasterManager.ActiveVehicle.UpdateVehicleMaterial();
            }
        }
    }
}


