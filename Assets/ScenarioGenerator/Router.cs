using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Router : MonoBehaviour
{
    public BridgeGenerator bridgeGenerator;
    public RobotGenerator robotGenerator;

    public List<Tour> tours;
    public Graphv2 graph;
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

    public void Render()
    {
        foreach(Tour tour in tours)
        {
            print("rending" + tour.ToString());

            int v0;
            int v1;
            Transform t1, t2;
            Vector3 dir;
            for (int i = 1; i < tour.vertexSequence.Count; i++)
            {
                // convert vId to actual object
                v0 = tour.vertexSequence[i - 1];
                v1 = tour.vertexSequence[i];

                t1 = bridgeGenerator.vertices[v0].transform;
                t2 = bridgeGenerator.vertices[v1].transform;
                dir = (t2.position - t1.position);
                Debug.DrawRay(t1.position, dir, new Color(Random.Range(0.0f,1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f)), 10);
            }
        }
    }

    public void Clear()
    {
        tours.Clear();
    }

    public void CreateTours()
    {
        // Call GA or Tabu Search or CPP
        for (int i = 0; i < 5; i++)
        {
            RandomTour(Random.Range(0, graph.SizeV() - 1), Random.Range(0, graph.SizeV() - 1));
        }

        //print(newTour.ToString());
    }

    public void RandomTour(int startV, int endV)
    {
        Tour newTour = new Tour();
        newTour.graph = graph;
        newTour.AddVertex(startV);
        newTour.AddVertex(endV);
        tours.Add(newTour);
    }
}
