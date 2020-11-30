using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GAMgr))]
public class GAEditor : Editor
{
    SerializedProperty robots;
    SerializedProperty pop;
    SerializedProperty gens;
    SerializedProperty runs;
    SerializedProperty vert;

    void OnEnable()
    {
        robots = serializedObject.FindProperty("numRobots");
        pop = serializedObject.FindProperty("popSize");
        gens = serializedObject.FindProperty("numGens");
        runs = serializedObject.FindProperty("numRuns");
        vert = serializedObject.FindProperty("startingVertex");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GAMgr gaMgr = (GAMgr)target;

        EditorGUILayout.PropertyField(robots, new GUIContent("Number of Robots"));
        EditorGUILayout.PropertyField(pop, new GUIContent("Population Size"));
        EditorGUILayout.PropertyField(gens, new GUIContent("Number of Generations"));
        EditorGUILayout.PropertyField(runs, new GUIContent("Number of Runs"));
        EditorGUILayout.PropertyField(vert, new GUIContent("Starting Vertex"));

        serializedObject.ApplyModifiedProperties();

        if(GUILayout.Button("RunGA"))
        {
            string dir = Application.dataPath + "/MMkCPP/ga.exe";
            string graph = Application.dataPath + "/Routing/graph.csv";
            gaMgr.RunGA();
        }
    }
}
