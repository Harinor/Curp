using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExtensionMethods
{
    public static class ExtensionMethods
    {
        public static void AddOnce<T>(this List<T> list, T newObject)
        {
            if (!list.Contains(newObject))
            {
                list.Add(newObject);
            }
        }

    } 
}
