using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(MicLightEffect))]
public class MicEditor : Editor
{
    int micIndex = 0;
    /**
     * Used to determine when you have pressed the Update Surround Group List and Smells List button to call the functions to generate new enum lists.
     */
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        

        MicLightEffect micLightEffectInstance = (MicLightEffect)target;
        micIndex = Array.IndexOf(MicList.MicStringsList, micLightEffectInstance.GetMicChoice());
        if(micIndex < 0)
        {
            micIndex = 0;
        }
        micIndex = EditorGUILayout.Popup(micIndex, MicList.MicStringsList);
        micLightEffectInstance.SetMicChoice(MicList.MicStringsList[micIndex]);
        if (GUILayout.Button("Update Mic List"))
        {
            micLightEffectInstance.AddMicList();
            AssetDatabase.Refresh();
        }
    }
}
