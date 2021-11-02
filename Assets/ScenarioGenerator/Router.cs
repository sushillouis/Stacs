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
        float offset = 0.1f;
        foreach(Tour tour in tours)
        {
            Color tourColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
            int v0;
            int v1;
            print("Rendering" + tour.ToString());

            Vector3 pos;
            Vector3 dir;
            Vector3 prevpos = bridgeGenerator.vertices[tour.vertexSequence[0]].transform.position + new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)) * offset;
            for (int i = 1; i < tour.vertexSequence.Count; i++)
            {
                // convert vId to actual object
                v0 = tour.vertexSequence[i - 1];
                v1 = tour.vertexSequence[i];

                pos = bridgeGenerator.vertices[tour.vertexSequence[i]].transform.position + new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)) * offset;
                dir = (pos - prevpos);
                Debug.DrawRay(prevpos, dir, tourColor, 30);
                prevpos = pos;
            }
        }
    }

    public void Clear()
    {
        foreach (Tour tour in tours)
            tour.Clear();
        tours.Clear();
    }

    public void CreateTours()
    {
        Clear();
        // Call GA or Tabu Search or CPP
        RandomSplitTours(Random.Range(1, 5));

        //print(newTour.ToString());
    }

    public void RandomSplitTours(int numTours)
    {
        List<int> unassignedEdges = new List<int>();
        for (int i = 0; i < bridgeGenerator.edges.Count; i++)
        {
            unassignedEdges.Add(i);
        }

        // create empty tours
        for (int i = 0; i < numTours; i++)
        {
            Tour newTour = new Tour();
            newTour.graph = graph;
            tours.Add(newTour);
        }


        // randomly distribute edges
        int robotId = -1;
        while (unassignedEdges.Count > 0)
        {
            robotId++;
            if (robotId >= numTours)
            {
                robotId = 0;
            }

            int index = Random.Range(0, unassignedEdges.Count);
            int edgeId = unassignedEdges[index];
            unassignedEdges.RemoveAt(index);
            tours[robotId].AddEdge(edgeId);
        }
    }
}
