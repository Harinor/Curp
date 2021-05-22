using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ModificationVariant : ScriptableObject
{
    public string Name = "variant name";

    public virtual void Apply()
    {
        
    }
}
