using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotGenerator : Generator
{
    public int numClimbingV1Robots = 4;
    public int numClimbingV2Robots = 4;
    public int numDrones = 4;
    public GameObject climbingV1Robot;
    public GameObject climbingV2Robot;
    public GameObject droneRobot;


    public ScenarioGenerator scenarioGenerator;

    public List<GameObject> robots;

    private bool placing;
    private float robotSpacing = 1;

    public override void Awake()
    {
        rootObjectName = "Robots";
        base.Awake();
        robots = new List<GameObject>();

    }

    public override void Generate()
    {
        base.Generate();
        for (int i = 0; i < numClimbingV1Robots; i++)
        {
            GameObject vertex = scenarioGenerator.bridgeGenerator.deploymentVertex.gameObject;
            Vector3 rayPoint = vertex.transform.position + new Vector3(1, 0, 0);
            Vector3 posOnSurface = vertex.transform.position + new Vector3((robotSpacing * i) - (numClimbingV1Robots * robotSpacing)/2, 0.3f, -0.4f);
            posOnSurface += .15f * posOnSurface.normalized;

            Vector3 surfaceNormal = Vector3.up;

            GameObject robot = Instantiate(climbingV1Robot);
            robot.transform.parent = rootObject.transform;
            //Vector3 offsetOnEdge = vertex.transform.forward;
            robot.transform.position = posOnSurface;// + offsetOnEdge;
            robot.transform.up = surfaceNormal;
            robot.transform.forward = Vector3.right;

            robots.Add(robot);
        }

        for (int i = 0; i < numClimbingV2Robots; i++)
        {
            GameObject vertex = scenarioGenerator.bridgeGenerator.deploymentVertex.gameObject;
            Vector3 rayPoint = vertex.transform.position + new Vector3(1, 0, 0);
            Vector3 posOnSurface = vertex.transform.position + new Vector3((robotSpacing * i) - (numClimbingV2Robots * robotSpacing) / 2, 0.3f, -0.9f);
            posOnSurface += .15f * posOnSurface.normalized;

            Vector3 surfaceNormal = Vector3.up;

            GameObject robot = Instantiate(climbingV2Robot);
            robot.transform.parent = rootObject.transform;
            //Vector3 offsetOnEdge = vertex.transform.forward;
            robot.transform.position = posOnSurface;// + offsetOnEdge;
            robot.transform.up = surfaceNormal;
            robot.transform.forward = Vector3.right;

            robots.Add(robot);
        }

        for (int i = 0; i < numDrones; i++)
        {
            GameObject vertex = scenarioGenerator.bridgeGenerator.deploymentVertex.gameObject;
            Vector3 rayPoint = vertex.transform.position + new Vector3(1, 0, 0);
            Vector3 posOnSurface = vertex.transform.position + new Vector3((robotSpacing * i) + 1, 0, + 1);
            posOnSurface += .15f * posOnSurface.normalized;

            Vector3 surfaceNormal = Vector3.up;

            GameObject robot = Instantiate(droneRobot);
            robot.transform.parent = rootObject.transform;
            //Vector3 offsetOnEdge = vertex.transform.forward;
            robot.transform.position = posOnSurface;// + offsetOnEdge;
            robot.transform.up = surfaceNormal;
            robot.transform.forward = Vector3.right;

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
