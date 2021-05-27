using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorVariant", menuName = "Variants/ColorVariant", order = 1)]
public class ColorVariant : ModificationVariant
{
    [SerializeField] Color color;
    [SerializeField] string bodySlot = "body";
    public override void Apply()
    {
        base.Apply();
        Transform body = MasterManager.ActiveVehicle.transform.Find(bodySlot);
        if (body != null)
        {
            if (body.TryGetComponent(out Renderer rend))
            {
                rend.material.color = color;
                MasterManager.ActiveVehicle.currentColor = color;
            }     
        }
    }
}

   
