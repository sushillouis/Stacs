using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GraphManager : MonoBehaviour
{
    public static GraphManager inst;

    public bool[,] unweightedGraph;
    public int[,] weightedGraph;

    void Awake()
    {
        if(inst == null)
        {
            inst = this;
        }
    }

    public void UpdateGraph(bool[,] other)
    {
        for(int i = 0; i < other.GetLength(0); i++)
        {
            for(int j = 0; j < other.GetLength(1); j++)
            {
                unweightedGraph[i, j] = other[i, j];
            }
        }
    }

    public void UpdateWeightedGraph()
    {
        int length = unweightedGraph.GetLength(0);
        if(weightedGraph == null)
        {
            weightedGraph = new int[length, length];
        }
        for(int i = 0; i < length; i++)
        {
            for(int j = 0; j < length; j++)
            {
                if(unweightedGraph[i, j])
                {
                    //TODO: adjust for windiness
                    weightedGraph[i, j] = Mathf.RoundToInt((GetComponent<SceneMgr>().AllClimbingWaypoints[i].position - GetComponent<SceneMgr>().AllClimbingWaypoints[j].position).magnitude * 10);
                    weightedGraph[j, i] = Mathf.RoundToInt((GetComponent<SceneMgr>().AllClimbingWaypoints[i].position - GetComponent<SceneMgr>().AllClimbingWaypoints[j].position).magnitude * 10);
                }
                else
                {
                    weightedGraph[i, j] = -1;
                }
            }
        }
    }

    public void SaveGraph()
    {
        //Create writer to write to file path
        StreamWriter writer = new StreamWriter(Application.dataPath + "/Routes/CSV/" + "graph.csv");
        //Update weights in float[,] based on bool[,]
        UpdateWeightedGraph();
        //Make each row into a string then write as a line to csv
        for(int i = 0; i < weightedGraph.GetLength(0); i++)
        {
            string line = null;
            for(int j = 0; j < weightedGraph.GetLength(0) - 1; j++)
            {
                line += weightedGraph[i, j].ToString() + ',';
            }
            line += weightedGraph[i, weightedGraph.GetLength(0) - 1].ToString();
            writer.WriteLine(line);
        }
        //Flush and close file
        writer.Flush();
        writer.Close();
    }

    public void ReadGraph()
    {
        //string to hold data read from file
        string file = null;
        //hold path to data file, just for convenience
        string path = Application.dataPath + "/Routes/CSV/" + "graph.csv";
        //Open and read file to string
        FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
        StreamReader read = new StreamReader(fileStream);
        file = read.ReadToEnd();
        read.Close();
        fileStream.Close();

        //holds each row from the csv
        string[] lines = file.Split("\n" [0]);
        //separates each row into individual data, then updates unweighted graph
        for(int i = 0; i < lines.Length - 1; i++)   // <-- The graph is saved with an extra blank line, so Length - 1 is necessary here
        {
            string[] parts = lines[i].Split("," [0]);
            for(int j = 0 ; j < i; j++)     // <-- Use j < i because we only case about one side of the symetrical graph
            {
                unweightedGraph[i, j] = (float.Parse(parts[j]) >= 0) ? true : false;
            }
        }
    }

    void OnDrawGizmos()
    {
        if(unweightedGraph != null)
        {
            for (int i = unweightedGraph.GetLength(0) - 1; i > 0; i--)
            {
                for (int j = 0; j < i; j++)
                {
                    if(unweightedGraph[i, j])
                    {
                        Vector3 from = GetComponent<SceneMgr>().AllClimbingWaypoints[i].position;
                        Vector3 to = GetComponent<SceneMgr>().AllClimbingWaypoints[j].position;
                        Gizmos.color = Color.blue;
                        Gizmos.DrawLine(from, to);
                    }
                }
            }
        }
    }
}

    //CURRENTLY UNUSED CODE FOR READING/WRITING TO JSON
    /*

[System.Serializable]
public class GraphContainer
{
    public List<Dimension> graph;

    public GraphContainer(bool[,] other)
    {
        graph = new List<Dimension>();
        int length = other.GetLength(0);
        for(int i = 0; i < length; i++)
        {
            List<float> list = new List<float>();
            for(int j = 0; j < length; j++)
            {
                float value;
                if(other[i, j])
                {
                    value = (SceneMgr.inst.AllClimbingWaypoints[j].position - SceneMgr.inst.AllClimbingWaypoints[i].position).magnitude;
                }
                else
                {
                    value = -1.0f;
                }
                list.Add(value);
            }
            Dimension dimension = new Dimension(list);
            graph.Add(dimension);
        }
    }
}

[System.Serializable]
public class Dimension
{
    public List<float> list;

    public Dimension(List<float> other)
    {
        list = new List<float>();
        foreach(float f in other)
        {
            list.Add(f);
        }
    }
}

    public void SaveGraph()
    {
        GraphContainer container = new GraphContainer(unweightedGraph);

        string json = JsonUtility.ToJson(container, true);
        File.WriteAllText(Application.dataPath + "/Routes/" + "graph.json", json);
    }

    public void ReadGraph()
    {
        string json = File.ReadAllText(Application.dataPath + "/Routes/graph.json");
        GraphContainer container = JsonUtility.FromJson<GraphContainer>(json);
        int i = 0;

        foreach(Dimension dim in container.graph)
        {
            int j = 0;
            foreach(float f in dim.list)
            {
                unweightedGraph[i, j] = (f >= 0.0f) ? true : false;
                j++;
            }
            i++;
        }

    }

    */