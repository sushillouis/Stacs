using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SceneMgr))]
public class SceneEditor : Editor
{
    SerializedProperty climbingWaypointsRoot;
    SerializedProperty climbingWaypoints;
    SerializedProperty climbingRobotRoutes;

    void OnEnable()
    {
        //climbingWaypointsRoot = serializedObject.FindProperty("AllClimbingWaypointsRoot");
        //climbingWaypoints = serializedObject.FindProperty("AllClimbingWaypoints");
        //climbingRobotRoutes = serializedObject.FindProperty("ClimbingRobotRoutes");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        //serializedObject.Update();

        SceneMgr sceneMgr = (SceneMgr)target;

        //EditorGUILayout.PropertyField(climbingWaypointsRoot, new GUIContent("Climbing Waypoints Root"));

        if(GUILayout.Button("Update Waypoints"))
        {
            sceneMgr.GetAllClimbingWaypoints();
        }

        //EditorGUILayout.PropertyField(climbingWaypoints, new GUIContent("All Climbing Waypoints"));

        if(GUILayout.Button("Read Climbing Routes"))
        {
            sceneMgr.ReadClimbingRobotRoutes();
        }

        //EditorGUILayout.PropertyField(climbingRobotRoutes, new GUIContent("Climbing RobotRoutes"));

        //serializedObject.ApplyModifiedProperties();
    }
}
/*
[CustomPropertyDrawer (typeof(Route))]
public class RoutePD : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty waypoints = property.FindPropertyRelative("Waypoints");
        EditorGUILayout.PropertyField(waypoints, new GUIContent("Waypoints"));
    }
}
*/