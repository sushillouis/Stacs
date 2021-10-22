using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeVertex : MonoBehaviour
{
    public int id;
    public string model;
    public List<BridgeEdge> edges;
    public List<BridgeVertex> neighborVertices;
    public BridgeGenerator bridgeBuilder;

    public List<GameObject> nearWaypoints;
    public List<GameObject> farWaypoints;


    private void Awake()
    {
        edges = new List<BridgeEdge>();
        neighborVertices = new List<BridgeVertex>();
        nearWaypoints = new List<GameObject>();
        farWaypoints = new List<GameObject>();
    }

    Vector2 Get2DPos()
    {
        float x = transform.position.x;
        // y pos is the magnitude from top center of bridge
        float y = transform.position.z * (new Vector3(x, bridgeBuilder.bridgeHeight, 0) - transform.position).magnitude;
        Vector2 twoDPos = new Vector2(x, y);
        return twoDPos;
    }

    public string GetHumanReadableJSON()
    {
        string data = "\t{\n";
        data += "\t\t\"id\": " + id.ToString() + ",\n";
        data += "\t\t\"mesh\": \"" + GetComponent<MeshFilter>().mesh.name + "\",\n";
        Vector2 twoDPos = Get2DPos();
        data += "\t\t\"v2Pos\": [" + twoDPos.x.ToString() + ", " + (twoDPos.y).ToString()  + "],\n";
        data += "\t\t\"v3Pos\": [" + transform.position.x.ToString() + ", " + transform.position.y.ToString() + ", " + transform.position.z.ToString() + "]\n";
        data += "\t}";
        return data;
    }
}
