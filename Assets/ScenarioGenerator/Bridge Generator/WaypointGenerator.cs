using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointGenerator : Generator
{
    //public int numDefects = 4;
    public GameObject waypointObject;
    public ScenarioGenerator scenarioGenerator;

    float nearDistance = 0.3f;

    public override void Awake()
    {
        scenarioGenerator = transform.GetComponent<ScenarioGenerator>();
        rootObjectName = "Waypoints";
        base.Awake();
    }

    private void CreateWaypoint(BridgeVertex vertex, Vector3 direction)
    {
        GameObject obj = Instantiate(waypointObject);
        obj.transform.parent = rootObject.transform;
        obj.transform.position = vertex.transform.position + direction * nearDistance;
/*
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        Collider collider = obj.GetComponent<Collider>();
        collider.is
        rb*/

        vertex.nearWaypoints.Add(obj);
    }

    public override void Generate()
    {
        base.Generate();
        foreach(BridgeVertex vertex in scenarioGenerator.bridgeGenerator.vertices)
        {
            CreateWaypoint(vertex, Vector3.up);
            CreateWaypoint(vertex, Vector3.down);
            CreateWaypoint(vertex, Vector3.left);
            CreateWaypoint(vertex, Vector3.right);
            CreateWaypoint(vertex, Vector3.forward);
            CreateWaypoint(vertex, Vector3.back);

        }
        /*        for (int i = 0; i < numDefects; i++)
                {
                    int ei = (int) Mathf.Floor(Random.Range(0, scenarioGenerator.bridgeGenerator.edges.Count - 1));
                    GameObject edge = scenarioGenerator.bridgeGenerator.edges[ei].transform.gameObject;

                    GameObject defect = Instantiate(defectObject);
                    defect.transform.parent = rootObject.transform;
                    Vector3 offsetOnEdge = edge.transform.forward * Random.Range(-edge.transform.localScale.z * 0.5f, edge.transform.localScale.z * 0.5f);
                    defect.transform.position = edge.transform.position + offsetOnEdge;
                }*/
    }
}
