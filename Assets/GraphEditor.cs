using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GraphManager))]
public class GraphEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GraphManager gm = (GraphManager)target;

        if(GUILayout.Button("Set Edges"))
        {
            EdgeEditor window = (EdgeEditor)EditorWindow.GetWindow(typeof(EdgeEditor), false, "Set Edges");
            window.Show();
        }

        if(GUILayout.Button("Generate Graph"))
        {
            gm.SaveGraph();
        }
    }
}

public class EdgeEditor : EditorWindow
{
    bool[,] fieldsArray = new bool[0,0];
    int length;
    int levelNumber = 1;
    
    [MenuItem("Window/LevelEditor")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(EdgeEditor));
    }
     
    void OnGUI()
    {
        //length = SceneMgr.inst.AllClimbingWaypoints.Count;
        /*
        if (length != fieldsArray.GetLength(0) || length != fieldsArray.GetLength(1)) {
            fieldsArray = new bool[length, length];
        }
        */
        DrawMatrix();
        //GraphManager.inst.UpdateGraph(fieldsArray);
    }

    void Awake()
    {
        SceneMgr.inst.AllClimbingWaypointsRoot.SetActive(true);
        length = SceneMgr.inst.AllClimbingWaypoints.Count;
        //DrawMatrix();
    }
 
    void DrawMatrix()
    {
        if(GraphManager.inst.unweightedGraph == null)
        {
            GraphManager.inst.unweightedGraph = new bool[length, length];
        }

        int width = 24;
        int height = 24;
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField((string)null, GUILayout.Width(width), GUILayout.Height(height));
        for (int i = 0; i < length - 1; i++)
        {
            GUILayout.Label(SceneMgr.inst.AllClimbingWaypoints[i].name, GUILayout.Width(width), GUILayout.Height(height));
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginVertical();
        for (int i = length - 1; i > 0; i--)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(SceneMgr.inst.AllClimbingWaypoints[i].name, GUILayout.Width(width), GUILayout.Height(height));
            for (int j = 0; j < i; j++)
            {
                //fieldsArray[i, j] = EditorGUILayout.Toggle(GraphManager.inst.graphContainer.unweightedGraph[i, j], GUILayout.Width(width), GUILayout.Height(height));
                GraphManager.inst.unweightedGraph[i, j] = EditorGUILayout.Toggle(GraphManager.inst.unweightedGraph[i, j], GUILayout.Width(width), GUILayout.Height(height));
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();
    }
}
