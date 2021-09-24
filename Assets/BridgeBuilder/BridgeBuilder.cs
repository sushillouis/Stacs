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
    public string bridgeName = "";
    public string objFile = "Assets/BridgeBuilder/test-bridge.obj";
    public string outputPath = "";

    public GameObject defaultVertex;
    public GameObject defaultEdge;

    public List<BridgeVertex> vertices;
    public List<BridgeEdge> edges;

    // bridge generation variables
    public int numSegments = 3;
    public float segmentSpacing = 1;
    public float roadWidth = 2;
    public float bridgeHeight = 2;

    public string bridgeType = "K-Truss";

    // private members variables
    private GameObject primaryObject;

    void Awake()
    {
        primaryObject = new GameObject();
        primaryObject.name = "Bridge";
    }

    void Start()
    {
        if (objFile != "")
        {
            LoadFromOBJ(objFile);
        } else
        {
            GenerateBridge();
        }
    }

    // ------------------------------------------- GENERATING BRIDGES ----------------------------------- //

    public void GenerateBridge()
    {
        print("generating a bridge");

        // get positive z position on one side of road
        float zPos = roadWidth / 2.0f;

        // make starting vertices in the center of the bridge
        BridgeVertex v1 = CreateVertex(new Vector3(0, 0, zPos), false, true);
        BridgeVertex v2 = CreateVertex(new Vector3(0, bridgeHeight, zPos), false, true);
        CreateEdge(v1, v2, false, true);
        CreateEdge(GetVertex(new Vector3(0, bridgeHeight, -zPos)), v2, false, true);


        // make middle segments starting from the center out
        float xPos = segmentSpacing;
        float heightReduction = 0;
        BridgeVertex prev_v2 = v2;
        for (int i = 1; i < numSegments / 2; ++i)
        {
            prev_v2 = v2;
            switch(bridgeType) {
                case "Pratt":
                    (v1, v2) = MakePrattSegement(v1, v2, xPos, zPos, bridgeHeight - heightReduction);
                    break;
                case "Howe":
                    (v1, v2) = MakeHoweSegement(v1, v2, xPos, zPos, bridgeHeight - heightReduction);
                    break;
                case "K-Truss":
                    (v1, v2) = MakeKTrussSegement(v1, v2, xPos, zPos, bridgeHeight - heightReduction);
                    break;
                case "Warren":
                    (v1, v2) = MakeWarrenSegement(v1, v2, xPos, zPos, bridgeHeight - heightReduction);
                    break;
            }
            MakeUpperXSegment(v2, prev_v2, xPos, zPos);
            xPos += segmentSpacing;
            heightReduction += (1 - Mathf.Cos(i / (float) Math.PI))/2.0f;
        }

        // make end segment
        BridgeVertex endv = CreateVertex(new Vector3(xPos, 0, zPos), true, true);
        CreateEdge(v1, endv, true, true);
        CreateEdge(v2, endv, true, true);

    }

    public (BridgeVertex newV1, BridgeVertex newV2) MakeHoweSegement(BridgeVertex prev_v1, BridgeVertex prev_v2, float xPos, float zPos, float height)
    {
        // make vertices
        BridgeVertex newV1 = CreateVertex(new Vector3(xPos, 0, zPos), true, true);
        BridgeVertex newV2 = CreateVertex(new Vector3(xPos, height, zPos), true, true);

        // make edges along x axis
            // horizontal
        CreateEdge(newV1, prev_v1, true, true);
        CreateEdge(newV2, prev_v2, true, true);
            // vertical
        CreateEdge(newV1, newV2, true, true);
            // diagonal
        CreateEdge(newV1, prev_v2, true, true);

        return (newV1, newV2);
    }

    public (BridgeVertex newV1, BridgeVertex newV2) MakePrattSegement(BridgeVertex prev_v1, BridgeVertex prev_v2, float xPos, float zPos, float height)
    {
        // make vertices
        BridgeVertex newV1 = CreateVertex(new Vector3(xPos, 0, zPos), true, true);
        BridgeVertex newV2 = CreateVertex(new Vector3(xPos, height, zPos), true, true);

        // make edges along x axis
        // horizontal
        CreateEdge(newV1, prev_v1, true, true);
        CreateEdge(newV2, prev_v2, true, true);
        // vertical
        CreateEdge(newV1, newV2, true, true);
        // diagonal
        CreateEdge(newV2, prev_v1, true, true);

        return (newV1, newV2);
    }

    public (BridgeVertex newV1, BridgeVertex newV2) MakeWarrenSegement(BridgeVertex prev_v1, BridgeVertex prev_v2, float xPos, float zPos, float height)
    {
        // make vertices
        BridgeVertex newV1 = CreateVertex(new Vector3(xPos, 0, zPos), true, true);
        BridgeVertex newV2 = CreateVertex(new Vector3(xPos, height, zPos), true, true);

        BridgeVertex vmid = CreateVertex(new Vector3(xPos - (0.5f * (float) segmentSpacing), 0, zPos), true, true);

        // make edges along x axis
        // horizontal
        CreateEdge(newV1, vmid, true, true);
        CreateEdge(vmid, prev_v1, true, true);
        CreateEdge(newV2, prev_v2, true, true);
        // diagonal
        CreateEdge(newV2, vmid, true, true);
        CreateEdge(prev_v2, vmid, true, true);
        // diagonal
        //CreateEdge(newV2, prev_v1, true, true);

        return (newV1, newV2);
    }

    public (BridgeVertex newV1, BridgeVertex newV2) MakeKTrussSegement(BridgeVertex prev_v1, BridgeVertex prev_v2, float xPos, float zPos, float height)
    {
        // make vertices
        BridgeVertex newV1 = CreateVertex(new Vector3(xPos, 0, zPos), true, true);
        BridgeVertex newV2 = CreateVertex(new Vector3(xPos, height, zPos), true, true);

        BridgeVertex midV = CreateVertex(new Vector3(xPos, height / 2, zPos), true, true);
        //BridgeVertex prevMidV = GetVertex(new Vector3(xPos - segmentSpacing, bridgeHeight / 2, zPos));

        // make edges along x axis
        // horizontal
        CreateEdge(newV1, prev_v1, true, true);
        CreateEdge(newV2, prev_v2, true, true);
        // vertical
        CreateEdge(newV1, midV, true, true);
        CreateEdge(newV2, midV, true, true);
        // diagonal
        CreateEdge(prev_v1, midV, true, true);
        CreateEdge(prev_v2, midV, true, true);

        return (newV1, newV2);
    }

    public void MakeUpperXSegment(BridgeVertex new_v2, BridgeVertex prev_v2, float xPos, float zPos)
    {
        // create upper edges
        CreateEdge(GetVertex(new Vector3(xPos, new_v2.transform.position.y, -zPos)), new_v2, true, false);

        BridgeVertex centerv = CreateVertex(new Vector3(xPos - segmentSpacing / 2.0f, new_v2.transform.position.y, 0), true, false);
        CreateEdge(centerv, prev_v2, true, true);
        CreateEdge(centerv, new_v2, true, true);
    }


    // ------------------------------------------------- VERTICES --------------------------------------- //

    public BridgeVertex GetVertex(Vector3 coordinate)
    {
        foreach (BridgeVertex otherbv in vertices)
            if (otherbv.transform.position.x == coordinate.x && otherbv.transform.position.y == coordinate.y && otherbv.transform.position.z == coordinate.z)
                return otherbv;
        return null;
    }

    public BridgeVertex CreateVertex(Vector3 coordinate)
    {
        // make sure vertex does not already exist
        if (GetVertex(coordinate) != null)
            return null;

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

    // public methods
    public BridgeVertex CreateVertex(Vector3 coordinate, bool mirrorAlongX, bool mirrorAlongZ)
    {
        // mirroring
        if (mirrorAlongX)
            CreateVertex(new Vector3(-coordinate.x, coordinate.y, coordinate.z));

        if (mirrorAlongZ)
            CreateVertex(new Vector3(coordinate.x, coordinate.y, -coordinate.z));

        if (mirrorAlongX && mirrorAlongZ)
            CreateVertex(new Vector3(-coordinate.x, coordinate.y, -coordinate.z));

        return CreateVertex(coordinate);
    }

    // ------------------------------------------------- EDGES --------------------------------------- //


    public BridgeEdge GetEdge(BridgeVertex vertex1, BridgeVertex vertex2)
    {
        foreach (BridgeEdge otherbe in edges)
            if ((otherbe.v1 == vertex1 && otherbe.v2 == vertex2) || (otherbe.v1 == vertex2 && otherbe.v2 == vertex1))
                return otherbe;
        return null;
    }

    public BridgeEdge CreateEdge(BridgeVertex vertex1, BridgeVertex vertex2)
    {
        // check if edge exists
        if (GetEdge(vertex1, vertex2) != null)
            return null;

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

    public BridgeEdge CreateEdge(BridgeVertex vertex1, BridgeVertex vertex2, bool mirrorAlongX, bool mirrorAlongZ)
    {
        // mirroring
        if (mirrorAlongX)
        {
            BridgeVertex v1 = GetVertex(new Vector3(-vertex1.transform.position.x, vertex1.transform.position.y, vertex1.transform.position.z));
            BridgeVertex v2 = GetVertex(new Vector3(-vertex2.transform.position.x, vertex2.transform.position.y, vertex2.transform.position.z));
            if (v1 != null && v2 != null)
                CreateEdge(v1, v2);
            else
                print("must create vertices fist to mirror an edge");
        }

        if (mirrorAlongZ)
        {
            BridgeVertex v1 = GetVertex(new Vector3(vertex1.transform.position.x, vertex1.transform.position.y, -vertex1.transform.position.z));
            BridgeVertex v2 = GetVertex(new Vector3(vertex2.transform.position.x, vertex2.transform.position.y, -vertex2.transform.position.z));
            if (v1 != null && v2 != null)
                CreateEdge(v1, v2);
            else
                print("must create vertices fist to mirror an edge");
        }

        if (mirrorAlongX && mirrorAlongZ)
        {
            BridgeVertex v1 = GetVertex(new Vector3(-vertex1.transform.position.x, vertex1.transform.position.y, -vertex1.transform.position.z));
            BridgeVertex v2 = GetVertex(new Vector3(-vertex2.transform.position.x, vertex2.transform.position.y, -vertex2.transform.position.z));
            if (v1 != null && v2 != null)
                CreateEdge(v1, v2);
            else
                print("must create vertices fist to mirror an edge");
        }

        return CreateEdge(vertex1, vertex2);
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
                CreateVertex(new Vector3(float.Parse(tokens[1]), float.Parse(tokens[2]), float.Parse(tokens[3])));
            } else if (tokens[0] == "l") {
                // todo verify tokens are all valid floats
                CreateEdge(vertices[Convert.ToInt32(tokens[1]) - 1], vertices[Convert.ToInt32(tokens[2]) - 1]);
            }
        }

        istream.Close();
    }


}
