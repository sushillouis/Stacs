using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotGenerator : Generator
{
    public int numRobots = 4;
    public GameObject climbingRobot;

    public ScenarioGenerator scenarioGenerator;

    public List<GameObject> robots;

    public int deployVertex = 0;
    private bool placing;

    public override void Awake()
    {
        base.Awake();
        robots = new List<GameObject>();
    }

    public override void Generate()
    {
        base.Generate();
        for (int i = 0; i < numRobots; i++)
        {
            int ei = (int) Mathf.Floor(Random.Range(0, scenarioGenerator.bridgeGenerator.edges.Count - 1));
            ei = deployVertex;
            GameObject vertex = scenarioGenerator.bridgeGenerator.vertices[ei].transform.gameObject;
            Vector3 actualPosOnSurface = vertex.transform.position;
            actualPosOnSurface += .15f * actualPosOnSurface.normalized;

            Vector3 surfaceNormal = actualPosOnSurface.normalized;

            GameObject robot = Instantiate(climbingRobot);
            robot.transform.parent = rootObject.transform;
            //Vector3 offsetOnEdge = vertex.transform.forward;
            robot.transform.position = actualPosOnSurface;// + offsetOnEdge;
            robot.transform.up = surfaceNormal;

            robots.Add(robot);
        }
    }

    public override void Clear()
    {
        base.Clear();
        robots = new List<GameObject>();
    }

    private void Update()
    {
        if (placing)
        {
            RaycastHit hit;
            Ray ray = Camera.current.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Transform objectHit = hit.transform;

                // Do something with the object that was hit by the raycast.
            }
        }
    }
}
