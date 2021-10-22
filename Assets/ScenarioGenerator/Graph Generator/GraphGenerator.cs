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
        rootObjectName = "Waypoints";
        base.Awake();
    }

    public override void Generate()
    {
        base.Generate();
        foreach(BridgeVertex vertex in scenarioGenerator.bridgeGenerator.vertices)
        {
            foreach (GameObject waypoint in vertex.nearWaypoints)
            {
                foreach (BridgeVertex otherVertex in scenarioGenerator.bridgeGenerator.vertices)
                {
                    if (vertex != otherVertex)
                    {
                        foreach (GameObject otherWaypoint in otherVertex.nearWaypoints)
                        {
                            int layerMask = 1 << 8;
                            float maxRange = 1000;

                            RaycastHit hit;
                            Vector3 direction = (otherWaypoint.transform.position - waypoint.transform.position);
                            // Does the ray intersect any objects excluding the player layer
                            if (Physics.Raycast(waypoint.transform.position, direction, out hit, maxRange))
                            {
                                //Debug.DrawRay(waypoint.transform.position, direction * hit.distance, Color.yellow, 100);
                                Debug.DrawLine(waypoint.transform.position, direction , Color.yellow, 100);
                                Debug.Log("Did Hit");
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
}
