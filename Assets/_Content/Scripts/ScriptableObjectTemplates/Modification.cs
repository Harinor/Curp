using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Modification", menuName = "Modification", order = 1)]
public class Modification : ScriptableObject
{
    public string Name = string.Empty;
    public List<ModificationVariant> variants = new List<ModificationVariant>();

    private int currentModIndex;
    public int CurrentModIndex
    {
        get { return currentModIndex; }
    }


    public void ApplyVariant(int index)
    {
        ModificationVariant newVAriant = variants[index];
        newVAriant.Apply();
        currentModIndex = index;
    }


    public bool IsValid()
    {
        bool output = true;
        if (!HasValidName())
        {
            output = false;
            Debug.LogWarning($"{GetName()} has no valid name.");
        }
        if (variants.Count < 1)
        {
            output = false;
            Debug.LogWarning($"{GetName()} has no variants.");
        }
        return output;
    }

    private string GetName()
    {
        if (HasValidName())
        {
            return Name;
        }
        return "this modification";
    }

    private bool HasValidName()
    {
        if (Name == null || Name == string.Empty)
        {
            return false;
        }
        return true;
    }
}
