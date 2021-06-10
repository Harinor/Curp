using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Chaneges the main/body material of the vehicle.
/// The "body" GameObject of the vehicle must have the MaterialSynchronizer component to reflect material changes.
/// </summary>
[CreateAssetMenu(fileName = "ColorVariant", menuName = "Variants/ColorVariant", order = 1)]
public class ColorVariant : ModificationVariant
{
    [SerializeField] Material material;

    public override void Apply()
    {
        base.Apply();

        MasterManager.ActiveVehicle.BodyMaterial = material;
    }
}

   
