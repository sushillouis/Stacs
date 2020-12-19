using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path
{
    public List<VertexDistance> vertices;
    //public List<int> edgeIds;
    public float length;

    public Path()
    {
        vertices = new List<VertexDistance>();
    }

    public string StringTo()
    {
        return "V: " + Vtos() + "\nLength: " + length;
    }
    public string Vtos()
    {
        string tmp = "";
        foreach(VertexDistance vd in vertices) {
            tmp += vd.vertex.ToString("0") + ", " + vd.distance.ToString() + " | ";
        }
        return tmp;
    }
}

public class VertexDistance : System.IComparable<VertexDistance>
{
    public int vertex;
    public int distance;
    public VertexDistance(int v2, int dist)
    {
        vertex = v2;
        distance = dist;
    }

    public int CompareTo(VertexDistance v2)
    {
        return (distance - v2.distance);
    }

}

public class Graph
{
    public int nRobots;
    public int nVertices;
    public int nEdges;
    //public int[,] vertices = new int[Constants.MAX_VERTICES, Constants.MAX_VERTICES];
    public int[,] vertices = new int[6, 6]
    {
        {0, 10, 20, -1, -1, -1},
        {10, 0, -1, 50, 10, -1},
        {20, -1, 0, 20, 33, -1},
        {-1, 50, 20, 0, 20, 2},
        {-1, 10, 33, 20, 0, 1},
        {-1, -1, -1, 2, 1, 0}
    };
    public List<VertexDistance>[] adjacency = new List<VertexDistance>[Constants.MAX_VERTICES]; // an array of nVertices lists

    public Options options;

    public Graph(string graphFilename)
    {
        //ReadGraph(graphFilename);
        nVertices = 6;
        Debug.Log(StringTo());
        MakeAdjacencyList();
        Debug.Log(AdjacencyStringTo());
        Dijsktra(0);
    }

    public void ReadGraph(string filepath)
    {
        if(System.IO.File.Exists(filepath)) {
            string[] lines = System.IO.File.ReadAllLines(filepath);
            nVertices = lines[0].Split(',').Length;
            Debug.Log("nVertices: " + nVertices);
            for(int i = 0; i < nVertices; i++) {
                string line = lines[i];
                string[] verts = line.Split(',');
                for(int j = 0; j < nVertices; j++) {
                    vertices[i, j] = int.Parse(verts[j]);
                }
            }
            Debug.Log("Finished reading graph with " + nVertices + " vertices");
        } else {
            Debug.Log("File Not found: " + filepath);
        }
    }

    public string StringTo()
    {
        string outs = "";
        for(int i = 0; i < nVertices; i++) {
            for(int j = 0; j < nVertices; j++) {
                outs += vertices[i, j].ToString("0") + " ";
            }
            outs += "\n";
        }
        return outs;
    }
    public string AdjacencyStringTo()
    {
        string outs = "";
        for(int i = 0; i < nVertices; i++) {
            foreach( VertexDistance vd in adjacency[i]) {
                outs += vd.vertex.ToString("0") + "," + vd.distance.ToString("0") + " | ";
            }
            outs += "\n";
        }
        return outs;
    }

    public void MakeAdjacencyList()
    {
        for(int i = 0; i < nVertices; i++) { // allocate lists
            adjacency[i] = new List<VertexDistance>();
        }
        for(int i = 0; i < nVertices; i++) {
            for(int j = 0; j < nVertices; j++) {
                if(i != j) {
                    if(vertices[i, j] != -1) {
                        adjacency[i].Add(new VertexDistance(j, vertices[i, j]));
                    }
                }
            }
        }
    }

    public void Dijsktra(int vertexId)
    {
        List<int> dist = new List<int>(nVertices);
        List<int> verts = new List<int>(nVertices);
        List<VertexDistance> pq = new List<VertexDistance>();
        pq.Add(new VertexDistance(vertexId, 0));

        for(int i = 0; i < nVertices; i++) {
            dist.Add(System.Int32.MaxValue);
            verts.Add(-1);
        }
        dist[vertexId] = 0;

        while(pq.Count != 0) {
            int vertM = pq[0].vertex;
            pq.RemoveAt(0);
            foreach(VertexDistance vdM in adjacency[vertM]) {
                int distance = vdM.distance;
                int newAdjacentVertex = vdM.vertex;
                if(dist[newAdjacentVertex] > dist[vertM] + distance) {
                    dist[newAdjacentVertex] = dist[vertM] + distance;
                    verts[newAdjacentVertex] = vertM;
                    pq.Add(new VertexDistance(newAdjacentVertex, dist[newAdjacentVertex]));
                    pq.Sort();
                }
            }
        }
        Path[] pathsTo = new Path[nVertices];
        for(int i = 0; i < nVertices; i++) {
            if(i != vertexId) {
                Path p = TracePath(dist, verts, i, vertexId);
                p.vertices.Reverse();
                pathsTo[i] = p;
                Debug.Log(p.StringTo());
            }
        }
    }

    public Path TracePath(List<int> dist, List<int> verts, int dest, int src)
    {
        Path p = new Path();
        int index = 0;
        p.vertices.Add(new VertexDistance(dest, -1));
        p.length = dist[dest];
        while(dest != src && index < nVertices) {
            index++;
            int vertex = verts[dest];
            p.vertices.Add(new VertexDistance(vertex, vertices[dest, vertex]));
            dest = vertex;
        }
        return p;

    }

}
