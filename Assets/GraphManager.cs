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

    //Copies values from another bool[,] graph to unweightedGraph
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

    //Updates weightedGraph based on unweightedGraph and calculates weights based on distance
    public void UpdateWeightedGraph()
    {
        //Get length needed for graph
        int length = unweightedGraph.GetLength(0);
        //Check weightedGraph has been created
        if(weightedGraph == null)
        {
            weightedGraph = new int[length, length];
        }
        //QUESTION: Can we just traverse one side of the symetrical bool[,] graph?
        //NOTE: Must fill out both sides of int[,] graph because weights may change based on windiness
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
                    //GA requires -1 as weight for unconnected vertices
                    weightedGraph[i, j] = -1;
                }
            }
        }
    }

    public void SaveGraph()
    {
        //Create writer to write to file path
        StreamWriter writer = new StreamWriter(Application.dataPath + "/Routing/" + "graph.csv");
        //Update weights in int[,] based on bool[,]
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
        string path = Application.dataPath + "/Routing/" + "graph.csv";
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
        //Check if unweightedGraph exists
        if(unweightedGraph != null)
        {
            //Traverse all rows of unweightedGraph
            for (int i = unweightedGraph.GetLength(0) - 1; i > 0; i--)
            {
                //Only traverse each row to the line of symmetery
                for (int j = 0; j < i; j++)
                {
                    //If connected, draw line
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