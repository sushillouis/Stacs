using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphGenerator : Generator
{
    //public int numDefects = 4;
    //public GameObject waypointObject;
    public ScenarioGenerator scenarioGenerator;

    //float nearDistance = 0.3f;

    public override void Awake()
    {
        scenarioGenerator = transform.GetComponent<ScenarioGenerator>();
        rootObjectName = "Graph";
        base.Awake();
    }

    public override void Generate()
    {
        base.Generate();
        foreach(BridgeVertex vertex in scenarioGenerator.bridgeGenerator.vertices)
        {
            foreach (GameObject waypoint in vertex.nearWaypoints)
            {
                foreach (BridgeVertex otherVertex in vertex.neighborVertices)
                {
                    foreach (GameObject otherWaypoint in otherVertex.nearWaypoints)
                    {
                        Vector3 direction = (otherWaypoint.transform.position - waypoint.transform.position);
                        // Does the ray intersect any objects excluding the player layer
// Debug.DrawRay(waypoint.transform.position, direction, Color.yellow, 5);

                        RaycastHit hit;

                        if (Physics.Raycast(waypoint.transform.position, direction, out hit))
                        {
                            if (hit.collider.gameObject.name == "Waypoint")
                            {
                                Debug.DrawRay(waypoint.transform.position, direction, Color.yellow, 15);
                                Debug.Log("Did Hit:" + hit.collider.gameObject.name);
                            }

                            //Debug.DrawRay(waypoint.transform.position, direction * hit.distance, Color.yellow, 100);

                        }
                        else
                        {

                        }
                    }

                }
            }
        }
    }
}
