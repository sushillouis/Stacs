using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

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

public class GraphManager : MonoBehaviour
{


    public static GraphManager inst;

    public bool[,] unweightedGraph;

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

    void OnDrawGizmos()
    {
        if(unweightedGraph != null)
        {
            for (int i = unweightedGraph.GetLength(0) - 1; i > 0; i--)
            {
                for (int j = 0; j < i; j++)
                {
                    if(GraphManager.inst.unweightedGraph[i, j])
                    {
                        Vector3 from = SceneMgr.inst.AllClimbingWaypoints[i].position;
                        Vector3 direction = SceneMgr.inst.AllClimbingWaypoints[j].position - from;
                        Gizmos.DrawRay(from, direction);
                    }
                }
            }
        }
    }
}
