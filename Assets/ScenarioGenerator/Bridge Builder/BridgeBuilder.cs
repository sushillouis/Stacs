using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using SFB; // Standalone file browser package



/*
 * Description: This class give the user interaction functionality to construct graphs in 3D space.
 * 
 * Dependencies: 
 */

public class BridgeBuilder : MonoBehaviour
{
    // public members variables

    public bool arched = false;
    public string objFile = "Assets/BridgeBuilder/test-bridge.obj";
    public string outputPath = "";

    public GameObject defaultVertex;
    public GameObject defaultEdge;

    public List<BridgeVertex> vertices;
    public List<BridgeEdge> edges;

    // bridge generation variables
    public int numSegments = 3;
    public float segmentSpacing = 1;
    private float surfaceWidth = 2;
    public float bridgeHeight = 2;
    private float bridgeLength = 0;
    private float trussWidth = 0.3f;


    public string bridgeType = "K-Truss";

    public bool mirrorZ = true;
    public bool mirrorX = true;

    // private members variables
    private GameObject primaryObject;
    private string bridgeName = "";

    void Awake()
    {
        primaryObject = new GameObject();
        primaryObject.name = "Bridge";
    }

    void Update()
    {

    }

    public void ClearBridge()
    {
        // TODO Prompt are you sure?
        foreach (BridgeVertex bv in vertices)
        {
            Destroy(bv.gameObject);
        }
        vertices.Clear();
        foreach (BridgeEdge be in edges)
        {
            Destroy(be.gameObject);
        }
        edges.Clear();
    }

    public void SaveBridge()
    {
        var path = StandaloneFileBrowser.SaveFilePanel("Save File", "", bridgeName, "json");

        File.WriteAllText(path, GetHumanReadableJSON());


        //ShowOutputFile();
    }

    public void SaveOneFaceOfBridge()
    {
        var path = StandaloneFileBrowser.SaveFilePanel("Save File", "", bridgeName, "json");

        File.WriteAllText(path, GetHumanReadableJSON());


        //ShowOutputFile();
    }

    public void SetNumSegments(string segments)
    {
        numSegments = Int32.Parse(segments);
    }

    public void SetBridgeHeight(string height)
    {
        bridgeHeight = float.Parse(height);
    }

    public void SetBridgeWidth(string width)
    {
        surfaceWidth = float.Parse(width);
    }

    public void SetSegmentLength(string len)
    {
        segmentSpacing = float.Parse(len);
    }

    public float GetLength()
    {
        return segmentSpacing * numSegments;
    }

    public void SetBridgeType(int type)
    {
        switch(type)
        {
            case 0:
                bridgeType = "Pratt";
                break;
            case 1:
                bridgeType = "Howe";
                break;
            case 2:
                bridgeType = "Warren";
                break;
            case 3:
                bridgeType = "K-Truss";
                break;
        }
    }

    void ShowOutputFile()
    {

    }

    private string GetHumanReadableJSON()
    {
        string data = "{\n";
        data += "\t\"name\": \"" + bridgeName + "\",\n";
        data += "\t\"type\": \"" + bridgeType.ToLower() + "\",\n";
        data += "\t\"numVertices\": " + vertices.Count + ",\n";
        data += "\t\"numEdges\": " + edges.Count + ",\n";
        data += "\t\"length\": " + segmentSpacing * Mathf.Ceil(numSegments) + ",\n";
        data += "\t\"width\": " + surfaceWidth + ",\n";
        data += "\t\"height\": " + bridgeHeight + ",\n";
        // vertices
        data += "\t\"vertices\": [\n";
        int count = 0;
        foreach (var vertex in vertices)
        {
            data += vertex.GetHumanReadableJSON();
            count++;
            if (count == vertices.Count)
                data += "\n";
            else
                data += ",\n";
        }
        data += "],\n";
        // edges
        data += "\t\"edges\": [\n";
        count = 0;
        foreach (var edge in edges)
        {
            data += edge.GetHumanReadableJSON();
            count++;
            if (count == edges.Count)
                data += "\n";
            else
                data += ",\n";
        }
        data += "]\n";
        data += "}";
        return data;
    }

    // ------------------------------------------- GENERATING BRIDGES ----------------------------------- //

    public void Generate(float width)
    {
        ClearBridge();
        surfaceWidth = width + trussWidth;
        bridgeLength = numSegments * segmentSpacing;

        if (arched)
            bridgeName = "arched-" + bridgeType.ToLower() + "-truss-bridge-" + numSegments.ToString() + "-segment";
        else
            bridgeName = bridgeType.ToLower() + "-truss-bridge-" + numSegments.ToString() + "-segment";

        print("Generating: " + bridgeName);
        // get positive z position on one side of road
        float zPos = surfaceWidth / 2.0f;

        // make starting vertices in the center of the bridge
        List<BridgeVertex> prevVertices = new List<BridgeVertex>();
        prevVertices.Add(CreateVertex(new Vector3(0, 0, zPos), false, true));
        prevVertices.Add(CreateVertex(new Vector3(0, bridgeHeight, zPos), false, true));

        if (bridgeType != "Warren")
        {
            CreateEdge(prevVertices[0], prevVertices[1], false, true);
        }
        CreateEdge(GetVertex(new Vector3(0, bridgeHeight, -zPos)), prevVertices[1], false, true);

        // make middle segments starting from the center out
        float xPos = segmentSpacing;
        float heightReduction = 0;
        //BridgeVertex prev_v2 = v2;
        for (int i = 1; i < Mathf.Ceil(numSegments / 2); ++i)
        {
            //prev_v2 = v2;
            switch(bridgeType) {
                case "Pratt":
                    prevVertices = MakePrattSegement(prevVertices, xPos, zPos, bridgeHeight - heightReduction);
                    break;
                case "Howe":
                    prevVertices = MakeHoweSegement(prevVertices, xPos, zPos, bridgeHeight - heightReduction);
                    break;
                case "K-Truss":
                    prevVertices = MakeKTrussSegement(prevVertices, xPos, zPos, bridgeHeight - heightReduction);
                    break;
                case "Warren":
                    prevVertices = MakeWarrenSegement(prevVertices, i, xPos, zPos, bridgeHeight - heightReduction);
                    break;
            }
            //MakeUpperXSegment(v2, prev_v2, xPos, zPos);
            xPos += segmentSpacing;
            //heightReduction += (1 - Mathf.Cos(i / (float) Math.PI))/2.0f;
        }

        // make end segment
        BridgeVertex endv;
        if (bridgeType != "Warren")
            endv = CreateVertex(new Vector3(xPos, 0, zPos), true, true);
        else
            endv = CreateVertex(new Vector3(xPos - segmentSpacing / 2.0f, 0, zPos), true, true);
        CreateEdge(prevVertices[0], endv, true, true);
        CreateEdge(prevVertices[1], endv, true, true);
    }

    public List<BridgeVertex> MakeHoweSegement(List<BridgeVertex> prev, float xPos, float zPos, float height)
    {
        // make vertices
        List<BridgeVertex> newV = new List<BridgeVertex>();
        newV.Add(CreateVertex(new Vector3(xPos, 0, zPos), true, true));
        newV.Add(CreateVertex(new Vector3(xPos, height, zPos), true, true));

        // horizontal
        CreateEdge(newV[0], prev[0], true, true);
        CreateEdge(newV[1], prev[1], true, true);
        // vertical
        CreateEdge(newV[0], newV[1], true, true);
        // diagonal
        CreateEdge(newV[0], prev[1], true, true);
        // top
        if (mirrorZ)
        {
            newV.Add(CreateVertex(new Vector3(xPos - segmentSpacing / 2.0f, newV[1].transform.position.y, 0), true, false));

            CreateEdge(newV[2], prev[1], true, true);
            CreateEdge(newV[2], newV[1], true, true);
            CreateEdge(GetVertex(new Vector3(xPos, newV[1].transform.position.y, -zPos)), newV[1], true, false);
        }


        return newV;
    }

    public List<BridgeVertex> MakePrattSegement(List<BridgeVertex> prev, float xPos, float zPos, float height)
    {
        // make vertices
        List<BridgeVertex> newV = new List<BridgeVertex>();
        newV.Add(CreateVertex(new Vector3(xPos, 0, zPos), true, true));
        newV.Add(CreateVertex(new Vector3(xPos, height, zPos), true, true));

        // horizontal
        CreateEdge(newV[0], prev[0], true, true);
        CreateEdge(newV[1], prev[1], true, true);
        // vertical
        CreateEdge(newV[0], newV[1], true, true);
        // diagonal
        CreateEdge(newV[1], prev[0], true, true);
        // top
        if (mirrorZ)
        {
            newV.Add(CreateVertex(new Vector3(xPos - segmentSpacing / 2.0f, newV[1].transform.position.y, 0), true, false));
            CreateEdge(newV[2], prev[1], true, true);
            CreateEdge(newV[2], newV[1], true, true);
            CreateEdge(GetVertex(new Vector3(xPos, newV[1].transform.position.y, -zPos)), newV[1], true, false);
        }

        return newV;
    }

    public List<BridgeVertex> MakeWarrenSegement(List<BridgeVertex> prev, int seg, float xPos, float zPos, float height)
    {
        // make vertices
        List<BridgeVertex> newV = new List<BridgeVertex>();
        newV.Add(CreateVertex(new Vector3(xPos, 0, zPos), true, true));
        newV.Add(CreateVertex(new Vector3(xPos, height, zPos), true, true));
        newV.Add(CreateVertex(new Vector3(xPos - segmentSpacing / 2.0f, 0, zPos), true, true));

        // horizontal
        CreateEdge(newV[0], newV[2], true, true);
        CreateEdge(newV[2], prev[0], true, true);
        CreateEdge(newV[1], prev[1], true, true);
        // vertical
        //CreateEdge(newV[0], newV[1], true, true);
        // diagonal
        CreateEdge(newV[2], prev[1], true, true);
        CreateEdge(newV[2], newV[1], true, true);

        // top
        if (mirrorZ)
        {
            newV.Add(CreateVertex(new Vector3(xPos - segmentSpacing / 2.0f, newV[1].transform.position.y, 0), true, false));
            CreateEdge(newV[3], prev[1], true, true);
            CreateEdge(newV[3], newV[1], true, true);
            CreateEdge(GetVertex(new Vector3(xPos, newV[1].transform.position.y, -zPos)), newV[1], true, false);
        }

        return newV;
    }

    public List<BridgeVertex> MakeKTrussSegement(List<BridgeVertex> prev, float xPos, float zPos, float height)
    {
        // make vertices
        List<BridgeVertex> newV = new List<BridgeVertex>();
        newV.Add(CreateVertex(new Vector3(xPos, 0, zPos), true, true));
        newV.Add(CreateVertex(new Vector3(xPos, height, zPos), true, true));
        newV.Add(CreateVertex(new Vector3(xPos, height / 2, zPos), true, true));

        // make edges along x axis
        // horizontal
        CreateEdge(newV[0], prev[0], true, true);
        CreateEdge(newV[1], prev[1], true, true);
        // vertical
        CreateEdge(newV[0], newV[2], true, true);
        CreateEdge(newV[1], newV[2], true, true);
        // diagonal
        CreateEdge(prev[0], newV[2], true, true);
        CreateEdge(prev[1], newV[2], true, true);

        // top
        if (mirrorZ)
        {
            newV.Add(CreateVertex(new Vector3(xPos - segmentSpacing / 2.0f, newV[1].transform.position.y, 0), true, false));
            CreateEdge(newV[3], prev[1], true, true);
            CreateEdge(newV[3], newV[1], true, true);
            CreateEdge(GetVertex(new Vector3(xPos, newV[1].transform.position.y, -zPos)), newV[1], true, false);
        }

        return newV;
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
        bv.bridgeBuilder = this;
        bv.id = vertices.Count;
        vertices.Add(bv);

        return bv;
    }

    // public methods
    public BridgeVertex CreateVertex(Vector3 coordinate, bool mirrorAlongX, bool mirrorAlongZ)
    {
        mirrorAlongX = mirrorAlongX && mirrorX;
        mirrorAlongZ = mirrorAlongZ && mirrorZ;

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
        if (vertex1 == null || vertex2 == null)
            return null;
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
        be.bridgeBuilder = this;
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
        mirrorAlongX = mirrorAlongX && mirrorX;
        mirrorAlongZ = mirrorAlongZ && mirrorZ;

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

    public void Select()
    {

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
