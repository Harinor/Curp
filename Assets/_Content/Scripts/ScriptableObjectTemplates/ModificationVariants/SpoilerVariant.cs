using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpoilerVariant", menuName = "Variants/SpoilerVariant", order = 1)]
public class SpoilerVariant : ModificationVariant
{
    [SerializeField] GameObject spoiler;
    [SerializeField] string spoilerSlot;

    public override void Apply()
    {
        base.Apply();
        Transform slot = SharedData.ActiveVehicle.transform.Find(spoilerSlot);
        foreach (Transform child in slot)
        {
            Destroy(child.gameObject);
        }
        if (spoiler)
        {
            Instantiate(spoiler, slot);
        }        
    }

}


