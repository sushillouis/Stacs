using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

/*
 * Description: This class give the user interaction functionality to construct graphs in 3D space.
 * 
 * Dependencies: 
 *  - GACode/Graph.cs - vertex class, edge class
 *  - GraphManager.cs - file io
 */

public class BridgeBuilder : MonoBehaviour
{
    // public members variables
    public List<BridgeVertex> vertices;
    public List<BridgeEdge> edges;

    public GameObject defaultVertex;
    public GameObject defaultEdge;

    // private members variables
    private GameObject primaryObject;

    void Awake()
    {
        primaryObject = new GameObject();
        primaryObject.name = "Bridge";
    }

    void Start()
    {
        LoadFromOBJ("Assets/BridgeBuilder/test-bridge.obj");
    }

    // public methods
    public BridgeVertex CreateOrGetVertex(Vector3 coordinate)
    {
        // make sure vertex does not already exist
        foreach (BridgeVertex otherbv in vertices)
            if (otherbv.transform.position.x == coordinate.x && otherbv.transform.position.y == coordinate.y && otherbv.transform.position.z == coordinate.z)
                return otherbv;

        // create vertex object
        GameObject go = Instantiate(defaultVertex);
        go.name = "V" + vertices.Count.ToString();
        go.transform.position = coordinate;
        go.transform.parent = primaryObject.transform;

        // create vertex component
        BridgeVertex bv = go.AddComponent<BridgeVertex>();
        bv.id = vertices.Count;
        vertices.Add(bv);
        return bv;
    }

    public BridgeEdge CreateOrGetEdge(BridgeVertex vertex1, BridgeVertex vertex2)
    {
        // check if edge exists
        foreach (BridgeEdge otherbe in edges)
            if ((otherbe.v1 == vertex1 && otherbe.v2 == vertex2) || (otherbe.v1 == vertex2 && otherbe.v2 == vertex1))
                return otherbe;

        // create edge object
        GameObject go = Instantiate(defaultEdge);
        go.name = "E" + edges.Count.ToString();
        go.transform.parent = primaryObject.transform;
        Vector3 dir = (vertex2.transform.position - vertex1.transform.position);
        go.transform.position = vertex1.transform.position + (dir / 2);
        go.transform.LookAt(vertex2.transform);
        go.transform.localScale = new Vector3(go.transform.localScale.x, go.transform.localScale.y, dir.magnitude);

        // create edge component
        BridgeEdge be = go.AddComponent<BridgeEdge>();
        be.id = edges.Count;
        be.v1 = vertex1;
        be.v2 = vertex2;
        edges.Add(be);

        // update vertices
        vertex1.edges.Add(be);

        return be;
    }

    public void RemoveVertex(BridgeVertex vertex)
    {
        // remove vertex from list
        // remove connecting edges
    }

    public void RemoveEdge(BridgeEdge edge)
    {
        // remove edge from list
    }

    public void LoadFromOBJ(string file_path)
    {
        StreamReader istream = new StreamReader(file_path);
        // todo verify files exists and all that jazz
        while (!istream.EndOfStream)
        {
            string ln = istream.ReadLine();
            string[] tokens = ln.Split(' ');
            if (tokens[0] == "v")
            {
                // todo verify tokens are all valid floats
                CreateOrGetVertex(new Vector3(float.Parse(tokens[1]), float.Parse(tokens[2]), float.Parse(tokens[3])));
            } else if (tokens[0] == "l") {
                // todo verify tokens are all valid floats
                CreateOrGetEdge(vertices[Convert.ToInt32(tokens[1]) - 1], vertices[Convert.ToInt32(tokens[2]) - 1]);
            }
        }

        istream.Close();
    }


}
