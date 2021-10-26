using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Router : MonoBehaviour
{
    public BridgeGenerator bridgeGenerator;
    public RobotGenerator robotGenerator;
    public List<Tour> tours;

    public List<BridgeEdge> planned;
/*    public List<BridgeEdge> unplanned;
    public List<BridgeEdge> visited;
    public List<BridgeEdge> unvisited;*/

    public void Awake()
    {
        tours = new List<Tour>();
/*
        planned = new List<BridgeEdge>();
        unplanned = new List<BridgeEdge>();
        visited = new List<BridgeEdge>();
        unvisited = new List<BridgeEdge>();*/
    }

    public void CreateTours()
    {
        // Call GA or Tabu Search or CPP

    }

    public void RandomEdgeAssignment(int numRobots)
    {

    }
}
