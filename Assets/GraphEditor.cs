using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GraphManager))]
public class GraphEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        GraphManager gm = (GraphManager)target;

        if(GUILayout.Button("Set Edges"))
        {
            //SceneMgr.inst.GetAllClimbingWaypoints();
            EdgeEditor window = (EdgeEditor)EditorWindow.GetWindow(typeof(EdgeEditor), false, "Set Edges");
            window.Show();
        }
    }
}

public class EdgeEditor : EditorWindow
{
    SceneMgr sceneMgr;
    GraphManager graphManager;
    int length = 0;
    
    [MenuItem("Window/EdgeEditor")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(EdgeEditor));
    }
     
    void OnGUI()
    {
        DrawMatrix();
        UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
    }

    void Awake()
    {
        //SceneMgr.inst.AllClimbingWaypointsRoot.SetActive(true);
        sceneMgr = Selection.activeGameObject.GetComponent<SceneMgr>();
        graphManager = Selection.activeGameObject.GetComponent<GraphManager>();
        sceneMgr.AllClimbingWaypointsRoot.SetActive(true);
        length = sceneMgr.AllClimbingWaypoints.Count;
        if(graphManager.unweightedGraph == null || graphManager.unweightedGraph.GetLength(0) != length)
        {
            graphManager.unweightedGraph = new bool[length, length];
        }
        graphManager.ReadGraph();
    }
 
    public void DrawMatrix()
    {
        int width = 28;
        int height = 28;
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField((string)null, GUILayout.Width(width), GUILayout.Height(height));
        for (int i = 0; i < length - 1; i++)
        {
            GUILayout.Label(sceneMgr.AllClimbingWaypoints[i].name, GUILayout.Width(width), GUILayout.Height(height));
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginVertical();
        for (int i = length - 1; i > 0; i--)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(sceneMgr.AllClimbingWaypoints[i].name, GUILayout.Width(width), GUILayout.Height(height));
            for (int j = 0; j < i; j++)
            {
                //fieldsArray[i, j] = EditorGUILayout.Toggle(GraphManager.inst.graphContainer.unweightedGraph[i, j], GUILayout.Width(width), GUILayout.Height(height));
                graphManager.unweightedGraph[i, j] = EditorGUILayout.Toggle(graphManager.unweightedGraph[i, j], GUILayout.Width(width), GUILayout.Height(height));
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();

        if(GUILayout.Button("Clear Graph"))
        {
            for(int i = 0; i < length; i++)
            {
                for(int j = 0; j < length; j++)
                {
                    graphManager.unweightedGraph[i, j] = false;
                }
            }
        }

        EditorGUILayout.Space();

        if(GUILayout.Button("Save Graph"))
        {
            graphManager.SaveGraph();
        }
    }
}