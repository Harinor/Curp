using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Dragoman))]
public class DragomanEditor : Editor
{
    readonly GUIContent updateDragonButton = new GUIContent(
        "Create a new localization file",
        "Create a new localization file. With the name supplied above.");
    readonly GUIContent generateTMProButton = new GUIContent(
    "Generate Text UI sources",
    "Generate base and TMPro UI localization sources.");

    public override void OnInspectorGUI()
    {
        Dragoman myScript = (Dragoman)target;

        DrawDefaultInspector();
            
        GUILayout.Space(10);

        if (GUILayout.Button(updateDragonButton))
        {
            myScript.CreateNewLocalizationFile();
        }

        if (GUILayout.Button(generateTMProButton))
        {
            myScript.GenerateTMProLocalizationComponents();
        }
    }
}