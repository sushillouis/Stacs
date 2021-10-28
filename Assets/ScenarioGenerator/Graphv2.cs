using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graphv2 : MonoBehaviour
{
    private List<List<float>> adjacencyMatrix;
    private Dictionary<int, Vector2> edgeList; // edge id to which two vertices, this ensures we can relate edges to real edges later

    private List<List<Tour>> cachedDijkstras;

    public void Awake()
    {
        adjacencyMatrix = new List<List<float>>();
        edgeList = new Dictionary<int, Vector2>();
        cachedDijkstras = new List<List<Tour>>();
    }
    public void Clear()
    {
        adjacencyMatrix = new List<List<float>>();
        edgeList = new Dictionary<int, Vector2>();
        cachedDijkstras = new List<List<Tour>>();

    }

    public override string ToString()
    {
        string str = "";
        for(int i = 0; i < adjacencyMatrix.Count; i++)
        {
            str += "[";
            for (int j = 0; j < adjacencyMatrix[i].Count; j++)
            {
                str += adjacencyMatrix[i][j].ToString();
                if (j != adjacencyMatrix[i].Count - 1)
                    str += ",";
            }
            str += "]\n";
        }
        return str;
    }

    // Fixes the adjacency matrix dimenions if not the right size for the vertex id.
    void FixAdjDimensions(int newLim)
    {
        // Zero base indexing means we need to increase the counts to 1+ newLim
        newLim += 1;

        // Extend the size of the matrix to match the new limit
        int i = adjacencyMatrix.Count - 1;
        while (adjacencyMatrix.Count < newLim)
        {
            i++;
            adjacencyMatrix.Add(new List<float>());
            cachedDijkstras.Add(new List<Tour>());
        }

        // Fix the cols
        for (i = 0; i < newLim; ++i)
        {
            while (adjacencyMatrix[i].Count < newLim)
            {
                adjacencyMatrix[i].Add(0);
                cachedDijkstras[i].Add(null);
            }
        }
    }

    void AddVertex(int vId)
    {
        // Make sure the adjacency matrix is the right size
        FixAdjDimensions(vId);
    }

    // Add a new edge to the adjacency matrix
    public void AddEdge(int id, int v1, int v2, float cost)
    {
        // Add vertices regarless if they exist because add vertex will resolve that
        AddVertex(v1);
        AddVertex(v2);

        // Check if edge exists
        if (adjacencyMatrix[v1][v2] == 0)
        {
            adjacencyMatrix[v1][v2] = cost;
            adjacencyMatrix[v2][v1] = cost;
        }

        if (!edgeList.ContainsKey(id))
        {
            edgeList.Add(id, new Vector2(v1, v2));
        }
    }

    int MinDistance(ref List<float> dist, ref List<bool> visited)
    {
        // Initialize min value
        KeyValuePair<int, float> bestVertDist = new KeyValuePair<int, float>(-1, float.PositiveInfinity);

        for (int v = 0; v < adjacencyMatrix.Count; v++)
            if (visited[v] == false && dist[v] <= bestVertDist.Value)
                bestVertDist = new KeyValuePair<int, float>(v, dist[v]);

        return bestVertDist.Key;
    }

    public void SolveAndCacheDijkstras()
    {
        int numVertices = adjacencyMatrix.Count;
        for (int startVertex = 0; startVertex < numVertices; startVertex++)
        {
            List<float> dist = new List<float>();
            List<bool> visited = new List<bool>();
            List<List<int>> subTours = new List<List<int>>();

            for (int i = 0; i < numVertices; i++)
            {
                dist.Add(float.PositiveInfinity);
                visited.Add(false);
                subTours.Add(new List<int>());
                subTours[i].Add(startVertex);
            }

            dist[startVertex] = 0;

            // The path to self is self
            cachedDijkstras[startVertex][startVertex] = new Tour();
            cachedDijkstras[startVertex][startVertex].graph = this;
            cachedDijkstras[startVertex][startVertex].AddVertex(startVertex);

            for (int count = 0; count < numVertices - 1; count++)
            {
                int nearestUnvisitedVertex = MinDistance(ref dist, ref visited);

                visited[nearestUnvisitedVertex] = true;

                for (int v = 0; v < numVertices; v++)
                {
                    if (!visited[v]     // not visited
                        && 0 < adjacencyMatrix[nearestUnvisitedVertex][v] // edge exists
                        && dist[nearestUnvisitedVertex] != float.PositiveInfinity  // explored
                        && dist[nearestUnvisitedVertex] + adjacencyMatrix[nearestUnvisitedVertex][v] < dist[v])
                    {

                        dist[v] = dist[nearestUnvisitedVertex] + adjacencyMatrix[nearestUnvisitedVertex][v];

                        subTours[v] = subTours[nearestUnvisitedVertex];
                        subTours[v].Add(v);

                        cachedDijkstras[startVertex][v] = new Tour();
                        cachedDijkstras[startVertex][v].graph = this;
                        for (int i = 0; i < subTours[v].Count; i++)
                        {
                            cachedDijkstras[startVertex][v].AddVertex(subTours[v][i]);
                        }
                    }
                }
            }
        }
    }
}
