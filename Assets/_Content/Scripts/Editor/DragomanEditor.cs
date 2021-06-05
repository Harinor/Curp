using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Dragoman))]
public class DragomanEditor : Editor
{
    GUIContent updateDragonButton = new GUIContent(
        "Update dragons", 
        "Updates all localizeable text.");

    public override void OnInspectorGUI()
    {
        Dragoman myScript = (Dragoman)target;

        DrawDefaultInspector();
            
        GUILayout.Space(10);

        if (GUILayout.Button(updateDragonButton))
        {
            myScript.UpdateDragons();
        }
  
    }
}