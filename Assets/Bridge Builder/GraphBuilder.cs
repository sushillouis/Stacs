using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Description: This class give the user interaction functionality to construct graphs in 3D space.
 * 
 * Dependencies: 
 *  - GACode/Graph.cs - vertex class, edge class
 *  - GraphManager.cs - file io
 */

public class GraphBuilder
{
    // public members variables
    public List<Vertex> vertices;
    public List<Edge> edges;

    // private members variables


    // public methods
    /// <summary>
    /// Creates a single vertex at in world space
    /// </summary>
    /// <param name="coordinate">the 3d world coordinate</param>
    /// <returns>bool state if sucessfully created</returns>
    public bool CreateVertex(vector3 coordinate)
    {

    }

    public bool CreateEdge(Vertex vertex1, Vertex vertex2)
    {
        // Check if edge exists

        // Create Edge

        // Update graph adjacency matrix
    }

    /// <summary>
    /// Creates an edge between two vertices
    /// </summary>
    /// <param name="isbn"></param>
    /// <returns></returns>
    public bool ConnectVertices(Vertex vertex1, Vertex vertex2)
    {
        CreateEdge(vertex1, vertex2);
    }



}
