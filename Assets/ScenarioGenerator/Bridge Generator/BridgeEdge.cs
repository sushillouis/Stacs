using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeEdge : MonoBehaviour
{
    public int id;
    public float cost;
    public BridgeVertex v1;
    public BridgeVertex v2;
    public BridgeGenerator bridgeBuilder;

    public string GetHumanReadableJSON()
    {
        string data = "\t{\n";
        data += "\t\t\"id\": " + id.ToString() + ",\n";
        data += "\t\t\"mesh\": \"" + GetComponent<MeshFilter>().mesh.name + "\",\n";
        data += "\t\t\"length\": " + transform.localScale.z + ",\n";
        data += "\t\t\"vIDs\": [" + v1.id.ToString() + ", " + v2.id.ToString() + "]\n"; 
        data += "\t}";
        return data;
    }
}
