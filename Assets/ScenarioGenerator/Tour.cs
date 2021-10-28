using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tour : MonoBehaviour
{
    public Graphv2 graph;

    public List<int> edgeSequence;
    public List<int> vertexSequence;
    public float cost;

    private void Awake()
    {
        edgeSequence = new List<int>();
        vertexSequence = new List<int>();
    }

    private bool GraphExists()
    {
        if (graph == null)
        {
            Debug.Log("Attempting to add vertex in tour for unknown graph.");
            return false;
        }
        return true;
    }

    public void AddVertex(int vertexId)
    {
        if (!GraphExists()) return;


        //TODO implement
    }

    void Clear()
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
                if (!graph->IsValidEdge(vertexSequence[i], vertexSequence[i + 1]))
                {
                    cout << "Invalid edge found" << endl;
                    return false;
                }
                if (graph->GetEdge(vertexSequence[i], vertexSequence[i + 1]).id != edgeSequence[i].id)
                {
                    cout << "Invalid vertex found" << endl;
                    return false;
                }
                tempCost += edgeSequence[i].cost;
            }
            if (tempCost != cost)
            {
                cout << "Invalid cost" << endl;
                return false;
            }
        }
        return true;
    }

    // Add the vertex regardless if the path makes sense
    void InsertVertex(int& vertexId)
    {
        if (vertexSequence.Count == 0 && edgeSequence.Count == 0)
        {
            vertexSequence.push_back(vertexId);
        }
        else if (vertexSequence.Count > 0 && edgeSequence.Count == 0 && vertexId != vertexSequence.back())
        {
            edgeSequence.push_back(graph->GetEdge(vertexId, vertexSequence.back()));
            vertexSequence.push_back(vertexId);
            cost += edgeSequence.back().cost;
        }
        else if (vertexSequence.Count > 0 && edgeSequence.Count > 0 && vertexId != vertexSequence.back())
        {
            edgeSequence.push_back(graph->GetEdge(vertexId, vertexSequence.back()));
            vertexSequence.push_back(vertexId);
            cost += edgeSequence.back().cost;
        }
    }

    void InjectShortestPathToVertex(int& vertex, Path& shortestPath)
    {
        for (int i = 0; i < shortestPath.vertexSequence.Count; i++)
        {
            AddVertex(shortestPath.vertexSequence[i]);
        }
    }

    void HandleFirstVertexNoEdges(int& vertex)
    {
        vertexSequence.push_back(vertex);
    }

    void HandleFirstVertexOneEdge(int& vertex)
    {
        vertexSequence.push_back(vertex);
        vertexSequence.push_back(graph->GetOppositeVertexOnEdge(vertex, edgeSequence.back()));
    }

    void HandleSecondVertexNoEdges(int& vertex)
    {
        if (vertex != vertexSequence.back())
        {
            edgeSequence.push_back(graph->GetEdge(vertexSequence.back(), vertex));
            cost += edgeSequence.back().cost;
            vertexSequence.push_back(vertex);
        }
    }

    void HandleAllOtherVertexCases(int& vertex)
    {
        if (vertex != vertexSequence.back())
        {
            if (graph->IsValidEdge(vertexSequence.back(), vertex))
            {
                Edge edge = graph->GetEdge(vertexSequence.back(), vertex);
                edgeSequence.push_back(edge);
                vertexSequence.push_back(vertex);
                cost += edge.cost;
            }
            else
            {
                InjectShortestPathToVertex(vertex, *graph->GetShortestPathBetweenVertices(vertexSequence.back(), vertex));
            }
        }
    }

    // Adds a vertex and resolves the path
    void AddVertex(int& vertex)
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
            HandleSecondVertexNoEdges(vertex);
        }
        else if (vertexSequence.Count > 0 && edgeSequence.Count > 0)
        {
            HandleAllOtherVertexCases(vertex);
        }
    }

    void InjectShortestPathToEdge(Edge& edge, Path& shortestPath)
    {
        for (int i = 0; i < shortestPath.edgeSequence.Count; i++)
        {
            AddEdge(shortestPath.edgeSequence[i]);
        }
        AddEdge(edge);
    }

    void HandleFirstEdgeNoStartingVertex(Edge& edge)
    {
        edgeSequence.push_back(edge);
        cost += edge.cost;
    }

    void HandleFirstEdgeWithStartingVertex(Edge& edge)
    {
        if (!(edge.vertexA == vertexSequence.back() || edge.vertexB == vertexSequence.back()))
        {
            InjectShortestPathToEdge(edge, *graph->GetShortestPathBetweenVertexAndEdge(vertexSequence.back(), edge));
        }
        else
        {
            edgeSequence.push_back(edge);
            vertexSequence.push_back(graph->GetOppositeVertexOnEdge(vertexSequence.back(), edge));
            cost += edge.cost;
        }
    }

    void HandleSecondEdgeNoStartingVertex(Edge& edge)
    {
        if (!graph->EdgesAreConnectedByVertex(edge, edgeSequence.back()))
        {
            InjectShortestPathToEdge(edge, *graph->GetShortestPathBetweenEdges(edgeSequence.back(), edge));
        }
        else
        {
            int sharedVertex = graph->GetEdgesConnectingVertex(edgeSequence.back(), edge);
            int startVertex = graph->GetOppositeVertexOnEdge(sharedVertex, edgeSequence.back());
            vertexSequence.push_back(startVertex);
            vertexSequence.push_back(sharedVertex);
            edgeSequence.push_back(edge);
            vertexSequence.push_back(graph->GetOppositeVertexOnEdge(sharedVertex, edgeSequence.back()));
            cost += edge.cost;
        }
    }

    void HandleAllOtherEdgeCases(Edge& edge)
    {
        if (!graph->EdgesAreConnectedByVertex(edge, edgeSequence.back()))
        {
            InjectShortestPathToEdge(edge, *graph->GetShortestPathBetweenEdges(edgeSequence.back(), edge));
        }
        else
        {
            if (edge.id != edgeSequence.back().id)
            {
                int sharedVertex = graph->GetEdgesConnectingVertex(edgeSequence.back(), edge);
                if (sharedVertex != vertexSequence.back())
                {
                    if (!graph->IsValidEdge(vertexSequence.back(), sharedVertex))
                        cout << "HELP" << endl;
                    vertexSequence.push_back(sharedVertex);
                    cost += edgeSequence.back().cost;
                    edgeSequence.push_back(edgeSequence.back());
                }
            }

            // add any other edge
            int oppositeVertex = graph->GetOppositeVertexOnEdge(vertexSequence.back(), edge);
            if (!graph->IsValidEdge(vertexSequence.back(), oppositeVertex))
                cout << "HELP" << endl;
            vertexSequence.push_back(oppositeVertex);
            edgeSequence.push_back(edge);
            cost += edge.cost;
        }
    }

    // Adds a edge and resolves the path
    void AddEdge(Edge& edge)
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

    vector<Edge> GetEdgePath()
    {
        return edgeSequence;
    }

    vector<int> GetVertexPath()
    {
        return vertexSequence;
    }

    const float GetCost()
    {
        return cost;
    }
}
