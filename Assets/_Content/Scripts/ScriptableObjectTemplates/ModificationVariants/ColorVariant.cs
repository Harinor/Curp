using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ColorVariant", menuName = "Variants/ColorVariant", order = 1)]
public class ColorVariant : ModificationVariant
{
    [SerializeField] Color color;
    [SerializeField] Material material;
    [SerializeField] string bodySlot = "body";
    public override void Apply()
    {
        base.Apply();
        Transform body = MasterManager.ActiveVehicle.transform.Find(bodySlot);
        if (body != null && body.TryGetComponent(out Renderer rend))
        {
            if (material != null)
            {
                rend.material = material;
            }
            else if (color != null)
            {
                rend.material.color = color;
            }

        }            
    }
}

   
