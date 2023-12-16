using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Cilia))]
public class SurroundGroupEditor : Editor
{
    /**
     * Used to determine when you have pressed the Update Surround Group List and Smells List button to call the functions to generate new enum lists.
     */
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Cilia ciliaInstance = (Cilia)target;

        if (GUILayout.Button("Update Surround Group List and Smells List"))
        {
            ciliaInstance.AddSurroundGroups();
            ciliaInstance.AddSmellsList();
            AssetDatabase.Refresh();
        }
    }
}
