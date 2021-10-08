using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotGenerator : Generator
{
    public int numRobots = 4;
    public GameObject climbingRobot;

    public ScenarioGenerator scenarioGenerator;

    public override void Generate()
    {
        base.Generate();
        for (int i = 0; i < numRobots; i++)
        {
            int ei = (int) Mathf.Floor(Random.Range(0, scenarioGenerator.bridgeGenerator.edges.Count - 1));
            GameObject edge = scenarioGenerator.bridgeGenerator.edges[ei].transform.gameObject;

            GameObject defect = Instantiate(climbingRobot);
            defect.transform.parent = rootObject.transform;
            Vector3 offsetOnEdge = edge.transform.forward * Random.Range(-edge.transform.localScale.z * 0.5f, edge.transform.localScale.z * 0.5f);
            defect.transform.position = edge.transform.position + offsetOnEdge;
        }
    }
}
