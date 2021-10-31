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

    public int SizeV()
    {
        return adjacencyMatrix.Count;
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
                str += cachedDijkstras[i][j].cost.ToString();
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
        }

        // Fix the cols
        for (i = 0; i < newLim; ++i)
        {
            while (adjacencyMatrix[i].Count < newLim)
            {
                adjacencyMatrix[i].Add(0);
            }
        }
    }

    public bool IsValidEdge(int v1, int v2)
    {
        if (edgeList.ContainsKey((v1, v2)))
        {
            return true;
        }
        //print("Trying to access edge (" + v1.ToString() + " " + v2.ToString() + ")");
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
            return -1;
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
// print(startVertex.ToString() + endVertex.ToString());
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

    int minDistance(ref float[] dist, ref bool[] spSet)
    {
        (int, float) best = (-1, float.PositiveInfinity);

        for (int v = 0; v < SizeV(); ++v)
            if (!spSet[v] && dist[v] <= best.Item2)
                best = (v, dist[v]);

        return best.Item1;
    }

    private void Dijkstras(int src)
    {
        // initialization
        float[] dist = new float[SizeV()];
        bool[] spSet = new bool[SizeV()];
       
        for (int i = 0; i < SizeV(); i++)
        {
            dist[i] = float.PositiveInfinity;
            spSet[i] = false;
            cachedDijkstras[src][i].AddVertex(src);
        }

        dist[src] = 0;

        // Find shortest paths
        for (int count = 0; count < SizeV() - 1; count++)
        {
            int u = minDistance(ref dist, ref spSet);
            spSet[u] = true;

            for (int v = 0; v < SizeV(); v++)
            {
                if (!spSet[v] 
                    && adjacencyMatrix[u][v] > 0 
                    && !float.IsPositiveInfinity(dist[u])
                    && dist[u] + adjacencyMatrix[u][v] < dist[v])
                {
                    dist[v] = dist[u] + adjacencyMatrix[u][v];

                    //paths[v] = paths[u];
                    //paths[v].Add(v);
                    for (int i = 0; i < cachedDijkstras[src][u].vertexSequence.Count; i++)
                    {
                        cachedDijkstras[src][v].InsertVertex(cachedDijkstras[src][u].vertexSequence[i]);
                    }
                    cachedDijkstras[src][v].InsertVertex(v);

/*                    for (int i = 0; i < cachedDijkstras[src][v].Count; i++)
                    {
                        cachedDijkstras[src][v].AddVertex(paths[v][i]);
                    }*/

/*                    for (int i = 0; i < tempTours[v].vertexSequence.Count; i++)
                    {
                        cachedDijkstras[src][v].AddVertex(tempTours[v].vertexSequence[i]);
                    }*/

                }
            }
        }
    }

    public void SolveAndCacheShortestPaths()
    {
        // init cache
        cachedDijkstras = new List<List<Tour>>();
        for (int i = 0; i < SizeV(); i++)
        {
            cachedDijkstras.Add(new List<Tour>());
            for (int j = 0; j < SizeV(); j++)
            {
                cachedDijkstras[i].Add(new Tour());
                cachedDijkstras[i][j].graph = this;
            }
        }

        // Solve dijkstras
        for (int v = 0; v < SizeV(); v++)
        {
            Dijkstras(v);
        }
    }
}
