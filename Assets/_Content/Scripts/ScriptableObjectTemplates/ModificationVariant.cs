using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A single variant of a specific modification. To be added to its modification's variants list.
/// </summary>
public abstract class ModificationVariant : ScriptableObject
{
    public string Name = "variant name";
    public GameObject previewPrefab;

    public virtual void Apply()
    {
        Debug.Log($"Applying {Name} to {MasterManager.ActiveVehicle.Name}.");
    }
}
