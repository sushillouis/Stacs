using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// This class is a container for holding bridge info and functions
public class Bridge : MonoBehaviour
{

    public static Bridge instance;
    private GameObject container; // holds all the bridge components

    public float edgeDistance = 0;

    private float platformLength;
    private float platformWidth;

    private GameObject containerSide1;
    private GameObject containerSide2;
    private GameObject containerTop;
    private GameObject containerBottem;
    private GameObject platform; // the horizontal surface that vehicles move on

    public Dictionary<Vector3, GameObject> vertices;
    public Dictionary<Tuple<Vector3, Vector3>, Edge> edges;
    public Dictionary<int, Edge> edgesId;
    public string existingBridgeString = "existingBridge";
    

    private int id;

    //used for moving vertex
    private Vector3 mOffset;
    private float mZCoord;

    public string bridgeName = "Default Bridge";
    public string bridgePath = "Bridges/";
    public InputField newBridgeName;
    public GameObject sandBoxObject;

    private void Awake()
    {
        // this keeps instance a singlton
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(this);
        }
        DontDestroyOnLoad(instance);
    }

    // Start is called before the first frame update
    void Start()
    {
        container = this.transform.gameObject;
        vertices = new Dictionary<Vector3, GameObject>();
        edges = new Dictionary<Tuple<Vector3, Vector3>, Edge>();
        edgesId = new Dictionary<int, Edge>();
        if (PlayerPrefs.GetString(existingBridgeString) != "")
            Load();
        if (PlayerPrefs.GetString("nextScene") == "SandboxMode")
        {
            SceneManager.LoadSceneAsync("SandboxSuite 1", LoadSceneMode.Additive);
            container = this.transform.gameObject;
            vertices = new Dictionary<Vector3, GameObject>();
            edges = new Dictionary<Tuple<Vector3, Vector3>, Edge>();
            edgesId = new Dictionary<int, Edge>();
            if (PlayerPrefs.GetString(existingBridgeString) != "")
                Load();
            if (PlayerPrefs.GetString("nextScene") == "SandboxMode")
            {
                Destroy(Camera.current);
                SceneManager.LoadSceneAsync(9);
            }
        }
    }

    /// <summary>
    /// sums up the edge distances
    /// </summary>
    /// <returns>float distance of sum of edges</returns>
    private float GetSumEdges()
    {
        float distance = 0;
        foreach (var edgeRef in edges)
        {
            distance += edgeRef.Value.transform.localScale.z;
        }
        return distance;
    }

    /// <summary>
    /// gets the vertex int ID
    /// </summary>
    /// <param name="pos">a vector 3 position</param>
    /// <returns>the int ID</returns>
    public int GetVertexId(Vector3 pos)
    {
        int index = 0;
        foreach (var vertex in vertices)
        {
            if (vertex.Key == pos)
            {
                return index;  // According to question, you are after the key 
            }
            index++;
        }
        return index;
    }

    /// <summary>
    /// helps save the bridge by encoding bridge connections
    /// </summary>
    /// <returns>the ecoded string</returns>
    private string FormatConnectionsToString() // connections between edges vertices
    {
        string output = "";
        output += vertices.Count.ToString() + "\n";
        output += edges.Count.ToString() + "\n";
        output += GetSumEdges().ToString() + "\n";
        foreach (var vert in vertices)
        {
            output += GetVertexId(vert.Key).ToString() + " " + vert.Key.x.ToString() + " " + vert.Key.y.ToString() + " " + vert.Key.z.ToString() + " " + "\n";
        }
        foreach (var edgeRef in edges)
        {
            output += GetVertexId(edgeRef.Value.pos1).ToString() + " " + GetVertexId(edgeRef.Value.pos2).ToString() + " " + edgeRef.Value.transform.localScale.z.ToString() + "\n";
        }
        return output;
    }

    /// <summary>
    /// encodes a bridge into a saveable string of data
    /// </summary>
    public void Save()
    {
        print("save");
        bridgeName = newBridgeName.text;
        FileIO.instance.WriteToFile(bridgeName + ".txt", FormatConnectionsToString(), true, true);
    }

    /// <summary>
    /// decodes bridge data into an actual bridge
    /// </summary>
    public void Load()
    {
        string[] tempVertices, tempEdges;
        string checkArray = null;
        print("Loading " + PlayerPrefs.GetString(existingBridgeString));
        tempVertices = FileIO.instance.ReadBridgeFile(PlayerPrefs.GetString(existingBridgeString))[0].Split(' ');
        tempEdges = FileIO.instance.ReadBridgeFile(PlayerPrefs.GetString(existingBridgeString))[1].Split(' ');
        float[] tempArray = new float[tempVertices.Length];
        for (int i = 0; i < tempVertices.Length-1 ; i++)
        {
            float.TryParse(tempVertices[i], out tempArray[i]);
            checkArray += tempArray[i].ToString() + ' ';
        }
        print(checkArray);
        for (int i = 0; i < tempArray.GetLength(0)-2; i += 3)
        {
            CreateVertex(new Vector3(tempArray[i], tempArray[i + 1], tempArray[i + 2]), BridgeCreator.instance.vertexPrefab);
            //AddVertex(new Vector3(tempArray[i], tempArray[i + 1], tempArray[1 + 2]));
        }
        int[] intArray = new int[tempEdges.Length];
        for (int i = 0; i < tempEdges.Length-1; i++)
        {
            int.TryParse(tempEdges[i], out intArray[i]);
            print(intArray[i]);
        }
        for (int i = 0; i < intArray.Length-1; i += 2)
        {
            BridgeCreator.instance.lastVertex = CreateVertex(GetVertexKey(intArray[i]), BridgeCreator.instance.vertexPrefab);
            CreateEdge(BridgeCreator.instance.lastVertex, CreateVertex(GetVertexKey(intArray[i + 1]), BridgeCreator.instance.vertexPrefab), BridgeCreator.instance.trussPrefab);
        }
        PlayerPrefs.SetString(existingBridgeString, "");
        BridgeCreator.instance.lastVertex = null;

    }

    /// <summary>
    /// converts a vertex key into a vector3 object
    /// </summary>
    /// <param name="index">a int key</param>
    /// <returns>a vector3 of a vertex</returns>
    private Vector3 GetVertexKey(int index)
    {
        int count = 0;
        foreach (var vert in vertices)
        {
            if (index == count)
                return vert.Key;
            else
                count++;
        }
        return new Vector3(0, 0, 0);
    }

    /// <summary>
    /// sets the type of platform on the bridge
    /// </summary>
    /// <param name="platformPrefab">the prefrab describing the look of the bridge</param>
    public void SetPlatform(GameObject platformPrefab)
    {
        platform = Instantiate(platformPrefab, Vector3.zero, Quaternion.identity, container.transform);
        SetWidth(platform.transform.localScale.z);
        SetLength(platform.transform.localScale.x);
    }

    /// <summary>
    /// gets the int ID of the bridge
    /// </summary>
    /// <returns>the id</returns>
    public int GetId()
    {
        return id;
    }

    /// <summary>
    /// sets a new bridge length
    /// </summary>
    /// <param name="newLength">sets a new bridge length</param>
    public void SetLength(float newLength)
    {
        platformLength = newLength;
        platform.transform.localScale = new Vector3(platformLength, 1, platform.transform.localScale.z);
    }

    /// <summary>
    /// gets the length of the bridge
    /// </summary>
    /// <returns>the float length of the bridge</returns>
    public float GetLength()
    {
        return platformLength;
    }

    /// <summary>
    /// sets the width of a bridge
    /// </summary>
    /// <param name="newWidth">the width of the bridge</param>
    public void SetWidth(float newWidth)
    {
        platformWidth = newWidth;
    }

    /// <summary>
    /// gets the platform width of a bridge
    /// </summary>
    /// <returns>a width</returns>
    public float GetWidth()
    {
        return platformWidth;
    }

    /// <summary>
    /// checks if a vertex exists
    /// </summary>
    /// <param name="position">a vector3 position</param>
    /// <returns>the vertex bridge object</returns>
    public bool VertexExists(Vector3 position)
    {
        return vertices.ContainsKey(position);
    }

    /// <summary>
    /// checks if an edge exists
    /// </summary>
    /// <param name="position1">a vertex position</param>
    /// <param name="position2">a second vertex position</param>
    /// <returns>the edge bridge object</returns>
    public Edge EdgeExists(Vector3 position1, Vector3 position2)
    {
        Tuple<Vector3, Vector3> edgeKey1 = new Tuple<Vector3, Vector3>(position1, position2);

        // check if edge already exsits
        if (edges.ContainsKey(edgeKey1))
        {
            return edges[edgeKey1];
        }

        Tuple<Vector3, Vector3> edgeKey2 = new Tuple<Vector3, Vector3>(position2, position1);

        if (edges.ContainsKey(edgeKey2))
        {
            return edges[edgeKey2];
        }
        return null;
    }

    /// <summary>
    /// creates a vertex
    /// </summary>
    /// <param name="p">the vertex position</param>
    /// <param name="vertex">the vertex object</param>
    /// <returns>the new bridge vertex object</returns>
    public GameObject CreateVertex(Vector3 p, GameObject vertex)
    {
        Debug.Log("create:" + p);
        if (vertices.ContainsKey(p))
        {
            return vertices[p];
        }

        Transform newVertex = Instantiate(vertex.transform);
        newVertex.name = "Vertex";
        newVertex.localPosition = p;
        newVertex.parent = transform;

        vertices.Add(p, newVertex.transform.gameObject);

        return newVertex.gameObject;
    }

    /// <summary>
    /// creates an edge
    /// </summary>
    /// <param name="v1">bridge object vector</param>
    /// <param name="v2">bridge object vector</param>
    /// <param name="truss">truss object type</param>
    /// <returns>the edge object</returns>
    public Edge CreateEdge(Vector3 v1, Vector3 v2, GameObject truss)
    {
        Debug.Log("CreateEdge1:" + v1);
        Debug.Log("CreateEdge2:" + v2);
        Edge edge = EdgeExists(v1, v2);
        if (edge != null)
        {
            return edge;
        }

        // create a new truss
        Transform newTruss = Instantiate(truss.transform);
        newTruss.name = "Truss";

        Vector3 diff = v1 - v2;
        float dist = diff.magnitude;
        newTruss.localScale = new Vector3(truss.transform.localScale.x, truss.transform.localScale.y, dist);
        newTruss.position = v2 + (diff / 2.0f);
        newTruss.LookAt(v1);
        newTruss.parent = transform;

        edgeDistance += dist;

        edge = newTruss.gameObject.GetComponent<Edge>();
        if (edge == null)
        {
            edge = newTruss.gameObject.AddComponent<Edge>();
        }
        edge.pos1 = v1;
        edge.pos2 = v2;
        edge.id = id;
        edge.distance = dist;

        edges.Add(new Tuple<Vector3, Vector3>(v1, v2), edge);
        edgesId.Add(id, edge);


        id++;
        return edge;
    }

    /// <summary>
    /// creates an edge inbetween two vertices
    /// </summary>
    /// <param name="v1">bridge object vector</param>
    /// <param name="v2">bridge object vector</param>
    /// <param name="truss">truss object type</param>
    /// <returns>the new edge object</returns>
    public Edge CreateEdge(GameObject v1, GameObject v2, GameObject truss)
    { 
        return CreateEdge(v1.transform.position, v2.transform.position, truss);
    }

    /// <summary>
    /// removes a vertex
    /// </summary>
    /// <param name="atPosition">the dictionary key position</param>
    public void RemoveVertex(Vector3 atPosition)
    {
        // find all the edge keys that contain the position
        List<Tuple<Vector3, Vector3>> edgekeysWithPosition = new List<Tuple<Vector3, Vector3>>();

        foreach (var pair in edges)
        {
            if (pair.Key.Item1 == atPosition || pair.Key.Item2 == atPosition)
            {
                edgekeysWithPosition.Add(pair.Key);
            }
        }

        // remove edges with the keys that we stored
        for (; edgekeysWithPosition.Count > 0;)
        {
            RemoveEdge(edgekeysWithPosition[0].Item1, edgekeysWithPosition[0].Item2);
            edgekeysWithPosition.Remove(edgekeysWithPosition[0]);
        }


        if (vertices.ContainsKey(atPosition))
        {
            Destroy(vertices[atPosition]);
            vertices.Remove(atPosition);
        }
    }

    /// <summary>
    /// removes and edge
    /// </summary>
    /// <param name="p1">one of the edge keys</param>
    /// <param name="p2">one of the edge keys</param>
    public void RemoveEdge(Vector3 p1, Vector3 p2)
    {
        Edge edge = null;
        Tuple<Vector3, Vector3> edgeKey1 = new Tuple<Vector3, Vector3>(p1, p2);

        // check if edge already exsits
        if (edges.ContainsKey(edgeKey1))
        {
            edge = edges[edgeKey1];
            edges.Remove(edgeKey1);
        }

        Tuple<Vector3, Vector3> edgeKey2 = new Tuple<Vector3, Vector3>(p2, p1);

        if (edges.ContainsKey(edgeKey2))
        {
            edge = edges[edgeKey2];
            edges.Remove(edgeKey2);
        }
        if (edge != null)
        {
            edgeDistance -= edge.distance;
            Destroy(edge.gameObject);
        }
    }

    /// <summary>
    /// removes an edge
    /// </summary>
    /// <param name="edge">an edge object</param>
    public void RemoveEdge(Edge edge)
    {
        RemoveEdge(edge.pos1, edge.pos2);
    }

    /// <summary>
    /// removes an edge
    /// </summary>
    /// <param name="edge">the edge game object</param>
    public void RemoveEdge(GameObject edge)
    {
        Edge edgeComponent = edge.GetComponent<Edge>();
        if (edgeComponent != null)
        {
            RemoveEdge(edgeComponent);
        }
    }

    /// <summary>
    /// updates an edge position
    /// </summary>
    /// <param name="p">is the position of the edge</param>
    /// <returns>the new vector position</returns>
    public Vector3 UpdateEdge(Vector3 p)
    {
        //iterate through all possible pairs
        //check for edge connections with p, old vertex
        //return the first value that is a pair with p
       
        foreach(KeyValuePair<Vector3, GameObject> V in vertices)
        {
            Tuple<Vector3, Vector3> edgeKey = new Tuple<Vector3, Vector3>(p, V.Key);
            Tuple<Vector3, Vector3> edgeKey2 = new Tuple<Vector3, Vector3>(V.Key, p);
            
            if (edges.ContainsKey(edgeKey))
            {
                RemoveEdge(edges[edgeKey]);
                return V.Key;
            }

            if (edges.ContainsKey(edgeKey2))
            {
                RemoveEdge(edges[edgeKey2]);
                return V.Key;
            }
        }
        Vector3 nullVec;
        //"null vector"
        nullVec.x = 100000;
        nullVec.y = 100000;
        nullVec.z = 100000;
        return nullVec; 
    }

    /// <summary>
    /// gets the edge connected to a vertex position
    /// </summary>
    /// <param name="p">vector position</param>
    /// <returns>a vector3 for the edge dictionary location</returns>
    public Vector3 GetEdge(Vector3 p)
    {
        //iterate through all possible pairs
        //check for edge connections with p, old vertex
        //return the first value that is a pair with p

        foreach (KeyValuePair<Vector3, GameObject> V in vertices)
        {
            Tuple<Vector3, Vector3> edgeKey = new Tuple<Vector3, Vector3>(p, V.Key);
            Tuple<Vector3, Vector3> edgeKey2 = new Tuple<Vector3, Vector3>(V.Key, p);

            if (edges.ContainsKey(edgeKey))
            {
                edges.Remove(edgeKey);
                return V.Key;
            }

            if (edges.ContainsKey(edgeKey2))
            {
                edges.Remove(edgeKey2);
                return V.Key;
            }
        }
        Vector3 nullVec;
        //"null vector"
        nullVec.x = 100000;
        nullVec.y = 100000;
        nullVec.z = 100000;
        return nullVec;
    }

    /// <summary>
    /// gets all the edges connected to a vertex
    /// </summary>
    /// <param name="pos">a vector3 of the dictionary index for the position of a vertex</param>
    /// <returns>a list of edges</returns>
    public List<Edge> GetAllEdgesContainingPosition(Vector3 pos)
    {
        List<Edge> edgesWithPos = new List<Edge>();

        foreach (var edgePair in edges)
        {
            if (edgePair.Value.pos1 == pos || edgePair.Value.pos2 == pos)
            {
                edgesWithPos.Add(edgePair.Value);
            }
        }

        return edgesWithPos;
    }

    /// <summary>
    /// moves a vertex from one position to another
    /// </summary>
    /// <param name="vertex">the gameobject vertex</param>
    /// <param name="newPos">the new position</param>
    public void MoveVertex(GameObject vertex, Vector3 newPos)
    {
        /*        RemoveVertex(oldloc);
                return CreateVertex(newloc, fab); */
        Debug.Log("New Vertex at:" + newPos);
        Vector3 currentPos = vertex.transform.position;
        // get all edges containing that current position
        List<Edge> edgesWithPos = GetAllEdgesContainingPosition(currentPos);

        foreach (Edge edge in edgesWithPos)
        {

            Vector3 otherPos = edge.pos1;
            if (edge.pos1 == currentPos)
            {
                otherPos = edge.pos2;
            }

            CreateEdge(newPos, otherPos, edge.transform.gameObject);
            RemoveEdge(edge);
        }

        // remove the vertex from the dictionary
        vertices.Remove(currentPos);

        // move and add to vertex dictionary
        vertex.transform.position = newPos;
        vertices.Add(newPos, vertex.gameObject);
    }

    /// <summary>
    /// gets the vertices count
    /// </summary>
    /// <returns>the count of vertices</returns>
    public int GetVertices()
    {
        return vertices.Count;
    }

    /// <summary>
    /// gets the edge count
    /// </summary>
    /// <returns>the count of the edges</returns>
    public int GetEdges()
    {
        return edges.Count;
    }
}

