using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tour
{
    public Graphv2 graph;

    public List<int> edgeSequence = new List<int>();
    public List<int> vertexSequence = new List<int>();
    public float cost;

    private bool GraphExists()
    {
        if (graph == null)
        {
            Debug.Log("Attempting to add vertex in tour for unknown graph.");
            return false;
        }
        return true;
    }

    public override string ToString()
    {
        string str = "Tour(" + cost.ToString() + ") : v[";
        for (int i = 0; i < vertexSequence.Count; ++i)
        {
            str += vertexSequence[i].ToString();
            if (i < vertexSequence.Count - 1)
            {
                str += ",";
            }
        }
        str += "], e[";
        for (int i = 0; i < edgeSequence.Count; ++i)
        {
            str += edgeSequence[i].ToString();
            if (i < edgeSequence.Count - 1)
            {
                str += ",";
            }
        }
        str += "]";
        return str;
    }

    public void Clear()
    {
        cost = 0;
        vertexSequence.Clear();
        edgeSequence.Clear();
    }

    // checks if the vertex path, edge path, and cost all make sense
    bool SanityCheckPass()
    {
        if (vertexSequence.Count > 1 && edgeSequence.Count > 0)
        {
            float tempCost = 0;
            for (int i = 0; i < vertexSequence.Count - 1; i++)
            {
                // If edge is valid
                if (!graph.IsValidEdge(vertexSequence[i], vertexSequence[i + 1]))
                {
                    Debug.Log("Invalid edge found");
                    return false;
                }
                // Idk what's happening here.
                if (graph.GetEdge(vertexSequence[i], vertexSequence[i + 1]) != edgeSequence[i])
                {
                    Debug.Log("Invalid vertex found");
                    return false;
                }
                tempCost += graph.GetEdgeCost(edgeSequence[i]);
            }
            if (tempCost != cost)
            {
                Debug.Log("Invalid cost");
                return false;
            }
        }
        return true;
    }

    // Force insert the vertex. This will not resolve issues in the tour (ie intermediate edges)
    public void InsertVertex(int vertexId)
    {
        if (vertexSequence.Count == 0 && edgeSequence.Count == 0)
        {
            vertexSequence.Add(vertexId);
        }
        else if (vertexSequence.Count > 0 && edgeSequence.Count == 0 && vertexId != vertexSequence[vertexSequence.Count - 1])
        {
            edgeSequence.Add(graph.GetEdge(vertexId, vertexSequence[vertexSequence.Count - 1]));
            vertexSequence.Add(vertexId);
            cost += graph.GetEdgeCost(edgeSequence[edgeSequence.Count - 1]);
        }
        else if (vertexSequence.Count > 0 && edgeSequence.Count > 0 && vertexId != vertexSequence[vertexSequence.Count - 1])
        {
            edgeSequence.Add(graph.GetEdge(vertexId, vertexSequence[vertexSequence.Count - 1]));
            vertexSequence.Add(vertexId);
            cost += graph.GetEdgeCost(edgeSequence[edgeSequence.Count - 1]);
        }
    }

    void InjectShortestPathToVertex(int vertex, Tour shortestPath)
    {
        for (int i = 0; i < shortestPath.vertexSequence.Count; i++)
        {
            AddVertex(shortestPath.vertexSequence[i]);
        }
    }

    void HandleFirstVertexNoEdges(int vertex)
    {
        vertexSequence.Add(vertex);
    }

    void HandleFirstVertexOneEdge(int vertex)
    {
        vertexSequence.Add(vertex);
        vertexSequence.Add(graph.GetOppositeVertexOnEdge(vertex, edgeSequence[edgeSequence.Count - 1]));
    }

    void HandleSecondVertexNoEdges(int vertex)
    {
        if (vertex != vertexSequence[vertexSequence.Count - 1])
        {
            edgeSequence.Add(graph.GetEdge(vertexSequence[vertexSequence.Count - 1], vertex));
            cost += graph.GetEdgeCost(edgeSequence[edgeSequence.Count - 1]);
            vertexSequence.Add(vertex);
        }
    }

    void HandleAllOtherVertexCases(int vertex)
    {
        if (vertex != vertexSequence[vertexSequence.Count - 1])
        {
            if (graph.IsValidEdge(vertexSequence[vertexSequence.Count - 1], vertex))
            {
                int edge = graph.GetEdge(vertexSequence[vertexSequence.Count - 1], vertex);
                edgeSequence.Add(edge);
                vertexSequence.Add(vertex);
                cost += graph.GetEdgeCost(edge);
            }
            else
            {
                InjectShortestPathToVertex(vertex, graph.GetShortestTourBetweenVertices(vertexSequence[vertexSequence.Count - 1], vertex));
            }
        }
    }

    // Adds a vertex and resolves missing edges inbetween vertices
    public void AddVertex(int vertex)
    {
        if (vertexSequence.Count == 0 && edgeSequence.Count == 0)
        {
            HandleFirstVertexNoEdges(vertex);
        }
        else if (vertexSequence.Count == 0 && edgeSequence.Count == 1)
        {
            HandleFirstVertexOneEdge(vertex);
        }
        else if (vertexSequence.Count == 1 && edgeSequence.Count == 0)
        {
            HandleAllOtherVertexCases(vertex);
        }
        else if (vertexSequence.Count > 0 && edgeSequence.Count > 0)
        {
            HandleAllOtherVertexCases(vertex);
        }
    }

    void InjectShortestTourToEdge(int edge, Tour shortestPath)
    {
        for (int i = 0; i < shortestPath.edgeSequence.Count; i++)
        {
            AddEdge(shortestPath.edgeSequence[i]);
        }
        AddEdge(edge);
    }

    void HandleFirstEdgeNoStartingVertex(int edge)
    {
        edgeSequence.Add(edge);
        cost += graph.GetEdgeCost(edge);
    }

    void HandleFirstEdgeWithStartingVertex(int edge)
    {
        (int, int) vertices = graph.GetEdgeVertices(edge);
        if (!(vertices.Item1 == vertexSequence[vertexSequence.Count - 1] || vertices.Item2 == vertexSequence[vertexSequence.Count - 1]))
        {
            InjectShortestTourToEdge(edge, graph.GetShortestTourBetweenVertexAndEdge(vertexSequence[vertexSequence.Count - 1], edge));
        }
        else
        {
            edgeSequence.Add(edge);
            vertexSequence.Add(graph.GetOppositeVertexOnEdge(vertexSequence[vertexSequence.Count - 1], edge));
            cost += graph.GetEdgeCost(edge);
        }
    }

    void HandleSecondEdgeNoStartingVertex(int edge)
    {
        int connectingVertex = graph.GetEdgesConnectingVertex(edge, edgeSequence[edgeSequence.Count - 1]);
        if (connectingVertex == -1)
        {
            InjectShortestTourToEdge(edge, graph.GetShortestTourBetweenEdges(edgeSequence[edgeSequence.Count - 1], edge));
        }
        else
        {
            int startVertex = graph.GetOppositeVertexOnEdge(connectingVertex, edgeSequence[edgeSequence.Count - 1]);
            vertexSequence.Add(startVertex);
            vertexSequence.Add(connectingVertex);
            edgeSequence.Add(edge);
            vertexSequence.Add(graph.GetOppositeVertexOnEdge(connectingVertex, edgeSequence[edgeSequence.Count - 1]));
            cost += graph.GetEdgeCost(edge);
        }
    }

    void HandleAllOtherEdgeCases(int edge)
    {
        int connectingVertex = graph.GetEdgesConnectingVertex(edge, edgeSequence[edgeSequence.Count - 1]);
        if (connectingVertex == -1)
        {
            InjectShortestTourToEdge(edge, graph.GetShortestTourBetweenEdges(edgeSequence[edgeSequence.Count - 1], edge));
        }
        else
        {
            if (edge != edgeSequence[edgeSequence.Count - 1])
            {
                int sharedVertex = graph.GetEdgesConnectingVertex(edgeSequence[edgeSequence.Count - 1], edge);
                if (sharedVertex != vertexSequence[vertexSequence.Count - 1])
                {
                    if (!graph.IsValidEdge(vertexSequence[vertexSequence.Count - 1], sharedVertex))
                        Debug.Log("HELP");
                    vertexSequence.Add(sharedVertex);
                    cost += graph.GetEdgeCost(edgeSequence[edgeSequence.Count - 1]);
                    edgeSequence.Add(edgeSequence[edgeSequence.Count - 1]);
                }
            }

            // add any other edge
            int oppositeVertex = graph.GetOppositeVertexOnEdge(vertexSequence[vertexSequence.Count - 1], edge);
            if (!graph.IsValidEdge(vertexSequence[vertexSequence.Count - 1], oppositeVertex))
                Debug.Log("HELP");
            vertexSequence.Add(oppositeVertex);
            edgeSequence.Add(edge);
            cost += graph.GetEdgeCost(edge);
        }
    }

    // Adds a edge and resolves the path
    public void AddEdge(int edge)
    {
        if (vertexSequence.Count == 0 && edgeSequence.Count == 0)
        {
            HandleFirstEdgeNoStartingVertex(edge);
        }
        else if (vertexSequence.Count == 1 && edgeSequence.Count == 0)
        {
            HandleFirstEdgeWithStartingVertex(edge);
        }
        else if (vertexSequence.Count == 0 && edgeSequence.Count == 1)
        {
            HandleSecondEdgeNoStartingVertex(edge);
        }
        else
        {
            HandleAllOtherEdgeCases(edge);
        }
    }

    List<int> GetEdgePath()
    {
        return edgeSequence;
    }

    List<int> GetVertexPath()
    {
        return vertexSequence;
    }
}
