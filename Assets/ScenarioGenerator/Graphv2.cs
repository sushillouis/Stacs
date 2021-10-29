using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Graphv2 : MonoBehaviour
{
    private List<List<float>> adjacencyMatrix;
    private Dictionary<(int,int), int> edgeList; // edge id to which two vertices, this ensures we can relate edges to real edges later

    public List<List<Tour>> cachedDijkstras;

    public void Awake()
    {
        adjacencyMatrix = new List<List<float>>();
        edgeList = new Dictionary<(int, int), int>();
        cachedDijkstras = new List<List<Tour>>();
    }
    public void Clear()
    {
        adjacencyMatrix.Clear();
        edgeList.Clear();
        cachedDijkstras.Clear();

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

        str += "Dijkstras:\n ";
        for (int i = 0; i < cachedDijkstras.Count; i++)
        {
            str += "[";
            for (int j = 0; j < cachedDijkstras[i].Count; j++)
            {
                if (cachedDijkstras[i][j] == null)
                {
                    str += "X";
                } else
                {
                    str += "O";
                }
                if (j != cachedDijkstras[i].Count - 1)
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

    public bool IsValidEdge(int v1, int v2)
    {
        if (edgeList.ContainsKey((v1, v2)))
        {
            return true;
        }
        print("Trying to access edge (" + v1.ToString() + " " + v2.ToString() + ")");
        return false;
    }

    public (int, int) GetEdgeVertices(int id)
    {
        foreach (var pair in edgeList)
        {
            if (pair.Value == id)
            {
                return pair.Key;
            }
        }
        // This is dangerous
        return (-1,-1);
    }

    public int GetEdge(int v1, int v2) {
        if (!IsValidEdge(v1, v2)) {
            
        }
        return edgeList[(v1, v2)];
    }

    public float GetEdgeCost(int v1, int v2)
    {
        return adjacencyMatrix[v1][v2];
    }

    public float GetEdgeCost(int id)
    {
        (int, int) vertices = GetEdgeVertices(id);
        return GetEdgeCost(vertices.Item1, vertices.Item2);
    }

    public int GetOppositeVertexOnEdge(int vertex, int edge) {
        (int, int) vertices = GetEdgeVertices(edge);
        if (vertices.Item1 == vertex) {
            return vertices.Item2;
        }

        return vertices.Item1;
    }

    public Tour GetShortestTourBetweenVertices(int startVertex, int endVertex) {
        print(startVertex.ToString() + endVertex.ToString());
        Tour tour = cachedDijkstras[startVertex][endVertex];
        if (tour == null)
        {
            tour = cachedDijkstras[endVertex][startVertex];
        }
        return tour;
    }

    public Tour GetShortestTourBetweenVertexAndEdge(int vertex, int edge)
    {
        (int, int) evs = GetEdgeVertices(edge);
        Tour tour = GetShortestTourBetweenVertices(vertex, evs.Item1);
        Tour bestTour = tour;
        if (tour.cost < bestTour.cost)
        {
            bestTour = tour;
        }
        tour = GetShortestTourBetweenVertices(vertex, evs.Item2);
        if (tour.cost < bestTour.cost)
        {
            bestTour = tour;
        }
        return bestTour;
    }

    public Tour GetShortestTourBetweenEdges(int edge1, int edge2)
    {
        (int, int) e1vs = GetEdgeVertices(edge1);
        Tour tour1 = GetShortestTourBetweenVertexAndEdge(e1vs.Item1, edge2);
        Tour tour2 = GetShortestTourBetweenVertexAndEdge(e1vs.Item2, edge2);
        if (tour1.cost < tour2.cost)
        {
            return tour1;
        } 
        return tour2;
    }

    public int GetEdgesConnectingVertex(int edge1, int edge2)
    {
        (int, int) vertices1 = GetEdgeVertices(edge1);
        (int, int) vertices2 = GetEdgeVertices(edge2);
        if (vertices1.Item1 == vertices2.Item1)
        {
            return vertices1.Item1;
        } else if (vertices1.Item1 == vertices2.Item2)
        {
            return vertices1.Item1;
        }
        else if (vertices1.Item2 == vertices2.Item1)
        {
            return vertices1.Item2;
        }
        else if (vertices1.Item2 == vertices2.Item2)
        {
            return vertices1.Item2;
        }
        return -1;
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

        if (!edgeList.ContainsValue(id))
        {
            edgeList.Add((v1, v2), id);
            edgeList.Add((v2, v1), id);
        }
    }

    int MinDistance(List<float> dist, List<bool> visited)
    {
        // Initialize min value
        float min = float.PositiveInfinity;
        int min_index = 0;

        for (int v = 0; v < adjacencyMatrix.Count; v++)
            if (visited[v] == false && dist[v] <= min && dist[v] != 0)
            {
                min = dist[v];
                min_index = v;
            }

        print(min);
        return min_index;
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
                int nearestUnvisitedVertex = MinDistance(dist, visited);
                visited[nearestUnvisitedVertex] = true;

                for (int v = 0; v < numVertices; v++)
                {
                    if (!visited[v]     // not visited
                        && 0 < adjacencyMatrix[nearestUnvisitedVertex][v] // edge exists
                        && !float.IsPositiveInfinity(dist[nearestUnvisitedVertex])  // explored
                        && dist[nearestUnvisitedVertex] + adjacencyMatrix[nearestUnvisitedVertex][v] < dist[v])
                    {

                        dist[v] = dist[nearestUnvisitedVertex] + adjacencyMatrix[nearestUnvisitedVertex][v];

                        subTours[v] = subTours[nearestUnvisitedVertex];
                        subTours[v].Add(v);

                        cachedDijkstras[startVertex][v] = new Tour();
                        cachedDijkstras[startVertex][v].graph = this;
                        //cachedDijkstras[v][startVertex] = new Tour();
                        //cachedDijkstras[v][startVertex].graph = this;
                        for (int i = 0; i < subTours[v].Count; i++)
                        {
                            cachedDijkstras[startVertex][v].InsertVertex(subTours[v][i]);
                            //cachedDijkstras[v][startVertex].AddVertex(subTours[v][subTours[v].Count - 1 - i]);
                        }
                        print(cachedDijkstras[startVertex][v].ToString());
                    }
                }
            }
        }
    }
}
