using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GAMgr))]
public class GAEditor : Editor
{
    SerializedProperty runs;
    SerializedProperty robots;

    void OnEnable()
    {
        runs = serializedObject.FindProperty("numRuns");
        robots = serializedObject.FindProperty("numRobots");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GAMgr gaMgr = (GAMgr)target;

        EditorGUILayout.PropertyField(runs, new GUIContent("Number of Runs"));
        EditorGUILayout.PropertyField(robots, new GUIContent("Number of Robots"));

        serializedObject.ApplyModifiedProperties();

        if(GUILayout.Button("RunGA"))
        {
            string dir = Application.dataPath + "/MMkCPP/ga.exe";
            string graph = Application.dataPath + "/Routing/graph.csv";
            gaMgr.RunGA();
        }
    }
}
