using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphPath
{
    public List<GraphVertex> vertices;
    //public List<int> edgeIds;
    public float length;

    public GraphPath()
    {
        vertices = new List<GraphVertex>();
        length = 0;
    }

    public string StringTo()
    {
        return "V: " + Vtos() + "\nLength: " + length;
    }
    public string Vtos()
    {
        string tmp = "";
        foreach(GraphVertex vd in vertices) {
            tmp += vd.vertex.ToString("0") + ", " + vd.distance.ToString() + " | ";
        }
        return tmp;
    }
}

public class Edge
{
    public int vertex1;
    public int vertex2;
    public bool visited;
    public float length;
    public Edge(int v1, int v2, float len)
    {
        vertex1 = v1;
        vertex2 = v2;
        visited = false;
        length = len;
    }

    public bool Same(Edge other)
    {
        return ((vertex1 == other.vertex1 && vertex2 == other.vertex2) ||
            (vertex2 == other.vertex1 && vertex1 == other.vertex2));
    }

    public string StringTo()
    {
        return "(" + vertex1 + ", " + vertex2 + ")";
    }
}


public class GraphVertex : System.IComparable<GraphVertex>
{
    public int vertex;
    public float distance; //for distance from a previously visited vertex (Dijsktra)
    public bool visited;
    //public List<Vertex> adjacents;
    public GraphVertex(int v, float dist)
    {
        vertex = v;
        distance = dist;
        visited = false;
        //adjacents = new List<Vertex>();
    }

    public int CompareTo(GraphVertex v2)
    {
        return (int) (distance - v2.distance);
    }

}

public class RobotRoute
{
    public int robot;
    public GraphPath route;
    public RobotRoute(int r)
    {
        robot = r;
        route = new GraphPath();
    }

    public void AddPath(GraphPath p)
    {
        for(int i = 1; i < p.vertices.Count; i++){//skip first vertex
            route.vertices.Add(p.vertices[i]);
        }
        route.length += p.length;
    }
}

public class Graph
{
    public int nVertices;
    public int nEdges;
    //public int[,] vertices = new int[Constants.MAX_VERTICES, Constants.MAX_VERTICES];
    /*
    public int[,] vertices = new int[6, 6]
    {
        {0, 10, 20, -1, -1, -1},
        {10, 0, -1, 50, 10, -1},
        {20, -1, 0, 20, 33, -1},
        {-1, 50, 20, 0, 20, 2},
        {-1, 10, 33, 20, 0, 1},
        {-1, -1, -1, 2, 1, 0}
    };
    */
    public float[,] vertices = new float[6, 6]
    {
        {0, 10, -1, -1, -1, 10},
        {10, 0, 10, 10, 10, -1},
        {-1, 10, 0, 10, -1, -1},
        {-1, 10, 10, 0, -1, -1},
        {-1, 10, -1, -1, 0, 10},
        {10, -1, -1, -1, 10, 0}
    };

    public List<GraphVertex>[] adjacency = new List<GraphVertex>[Constants.MAX_VERTICES]; // an array of nVertices lists
    public List<Edge> edges;
    public Options options;

    public Graph(string graphFilename)
    {
        //ReadGraph(graphFilename);
        nVertices = 6;
        Debug.Log(StringTo());
        MakeAdjacencyListEdgesList();
        Debug.Log(AdjacencyStringTo());
        GraphPath[] testPaths = Dijsktra(0);
        MakePathCache();
        //PrintCache();
        EulerCircuit(new GraphVertex(1, 0));
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
            foreach( GraphVertex vd in adjacency[i]) {
                outs += vd.vertex.ToString("0") + "," + vd.distance.ToString("0") + " | ";
            }
            outs += "\n";
        }
        return outs;
    }

    public bool ExistsEdge(Edge edge, List<Edge> edges)
    {
        foreach(Edge e in edges)
        {
            if (edge.Same(e))
                return true;
        }
        return false;
    }

    public void MakeAdjacencyListEdgesList()
    {
        for(int i = 0; i < nVertices; i++) { // allocate lists
            adjacency[i] = new List<GraphVertex>();
        }
        edges = new List<Edge>();
        for(int i = 0; i < nVertices; i++) {
            for(int j = 0; j < nVertices; j++) {
                if(i != j) {
                    if(vertices[i, j] != -1) {
                        adjacency[i].Add(new GraphVertex(j, vertices[i, j]));
                        Edge e = new Edge(i, j, vertices[i, j]);
                        if(!ExistsEdge(e, edges))
                            edges.Add(e);
                    }
                }
            }
        }
        nEdges = edges.Count;
        Debug.Log("N Edges: " + nEdges);
        Debug.Log("Edges: ");
        int n = 0;
        foreach(Edge e in edges) {
            Debug.Log("[" + n + "]: " + e.StringTo());
            n += 1;
        }

    }

    /// <summary>
    /// Assuming a graph that only contains vertices of even degree.
    /// CPP needs a prior algorithm to convert the graph into one with only even degree vertices
    /// </summary>
    public void EulerCircuit(GraphVertex vertex)
    {
        GraphPath tour = new GraphPath();
        List<GraphPath> subtours = new List<GraphPath>();
        GraphPath subtour = new GraphPath();

        GraphVertex start = vertex;
        subtour.vertices.Add(vertex);
        tour.vertices.Add(vertex);
        //int tourIndex = tour.vertices.Count ;
        int counter = 0;
        while(!AllEdgesInTour(tour) && counter++ < 100) {
            GraphVertex unvisited = ChooseUnvisitedEdge(start, subtour);
            //Debug.Log("start: " + start.vertex + " unvisited: " + unvisited.vertex);
            if(unvisited == null) { // if no unvisited edges on this vertex,
                Debug.Log("Closing subtour" + subtour);
                subtours.Add(subtour); // close subtour
                InsertSubtour(subtour, tour, start);
                start = FindVertexWithUnvisitedEdges(subtour); //start new subtour
                if(start == null)
                    break;
                else
                    subtour = new GraphPath();
            } else {
                //Debug.Log("adding vertex: " + unvisited.vertex);
                subtour.vertices.Add(unvisited);
                subtour.length += vertices[start.vertex, unvisited.vertex];
                VisitEdge(start, unvisited);
                start = unvisited;
            }
        }
        subtours.Add(subtour);
        InsertSubtour(subtour, tour, start);

        Debug.Log("Tour: " + tour.StringTo());
        foreach(GraphPath p in subtours) {
            Debug.Log("Subtour: " + p.StringTo());
        }

    }
    
    public int FindVertexInTour(GraphVertex vd, GraphPath tour)
    {
        int index = -1;
        foreach(GraphVertex tvd in tour.vertices) {
            index += 1;
            if(vd.vertex == tvd.vertex)
                return index;
        }
        return index;
    }

    public void InsertSubtour(GraphPath subtour, GraphPath tour, GraphVertex start)
    {
        Debug.Log("Inserting subtour at: " + start.vertex);
        int index = FindVertexInTour(start, tour);
        if(index == -1) 
            Debug.Log("Error in inserting subtour: " + tour.StringTo() +
                "\nSubtour: " + subtour.StringTo() +
                "\nStart: " + start.vertex + ", index: " + index);
        tour.vertices.RemoveAt(index);
        tour.vertices.AddRange(subtour.vertices);
        tour.length += subtour.length;
        //Debug.Log("New tour: " + tour.StringTo());
    }

    public GraphVertex FindVertexWithUnvisitedEdges(GraphPath tour)
    {
        foreach(GraphVertex vertex in tour.vertices) {
            GraphVertex neighbor = ChooseUnvisitedEdge(vertex, tour);
            if(neighbor != null)
                return vertex;
        }
        return null;
    }

    public bool AllEdgesInTour(GraphPath tour)
    {
        foreach(Edge edge in edges) {
            if(!edge.visited) return false;
        }
        return true;
    }

    public int ChooseVertex()
    {
        return 0;
    }

    public GraphVertex ChooseUnvisitedEdge(GraphVertex vd, GraphPath tour)
    {
        GraphVertex start = vd;
        foreach(GraphVertex neighbor in adjacency[start.vertex]) {
            if(!IsVisitedEdge(vd, neighbor))
                return neighbor;
        }
        return null;
    }
    
    public void VisitEdge(GraphVertex v1, GraphVertex v2)
    {
        Edge edge = new Edge(v1.vertex, v2.vertex, vertices[v1.vertex, v2.vertex]);
        bool found = false;
        foreach(Edge e in edges) {
            if(edge.Same(e)) {
                e.visited = true;
                found = true;
            }
        }
        if(!found) {
            Debug.Log("VisitEdge: Cannot find edge! FATAL ERROR: " + v1.vertex + ". " + v2.vertex);
        }
    }


    public bool IsVisitedEdge(GraphVertex v1, GraphVertex v2)
    {
        Edge edge = new Edge(v1.vertex, v2.vertex, vertices[v1.vertex, v2.vertex]);
        foreach(Edge e in edges) {
            if(e.Same(edge))
                return e.visited;
        }
        Debug.Log("IsVisitedEdge: Cannot find edge! FATAL ERROR: " + v1.vertex + ". " + v2.vertex);
        return false;
    }

    public GraphPath[] Dijsktra(int vertexId)
    {
        List<float> dist = new List<float>(nVertices);
        List<int> verts = new List<int>(nVertices);
        List<GraphVertex> pq = new List<GraphVertex>();
        pq.Add(new GraphVertex(vertexId, 0));

        for(int i = 0; i < nVertices; i++) {
            //dist.Add(System.Int32.MaxValue);
            dist.Add(float.MaxValue);
            verts.Add(-1);
        }
        dist[vertexId] = 0;

        while(pq.Count != 0) {
            int vertM = pq[0].vertex;
            pq.RemoveAt(0);
            foreach(GraphVertex vdM in adjacency[vertM]) {
                float distance = vdM.distance;
                int newAdjacentVertex = vdM.vertex;
                if(dist[newAdjacentVertex] > dist[vertM] + distance) {
                    dist[newAdjacentVertex] = dist[vertM] + distance;
                    verts[newAdjacentVertex] = vertM;
                    pq.Add(new GraphVertex(newAdjacentVertex, dist[newAdjacentVertex]));
                    pq.Sort();
                }
            }
        }
        GraphPath[] pathsTo = new GraphPath[nVertices];
        for(int i = 0; i < nVertices; i++) {
            if(i != vertexId) {
                GraphPath p = TracePath(dist, verts, i, vertexId);
                p.vertices.Reverse();
                pathsTo[i] = p;
//                Debug.Log(pathsTo[i].StringTo());
            } else {
                pathsTo[i] = null;
            }

        }
//        for(int i = 0; i< nVertices; i++) { 
//            Debug.Log("Dijsktra Paths: " + vertexId + ": " + pathsTo[i].StringTo());
//        }
        return pathsTo;
    }

    public GraphPath TracePath(List<float> dist, List<int> verts, int dest, int src)
    {
        GraphPath p = new GraphPath();
        int index = 0;
        p.vertices.Add(new GraphVertex(dest, -1));
        p.length = dist[dest];
        while(dest != src && index < nVertices) {
            index++;
            int vertex = verts[dest];
            p.vertices.Add(new GraphVertex(vertex, vertices[dest, vertex]));
            dest = vertex;
        }
        return p;

    }
    public GraphPath[,] pathCache;
    public void MakePathCache()
    {
        pathCache = new GraphPath[nVertices, nVertices];

        for(int i = 0; i < nVertices; i++) {
            GraphPath[] paths = new GraphPath[nVertices];
            paths = Dijsktra(i);
            for(int j = 0; j < nVertices; j++) {
                pathCache[i, j] = paths[j];
            }
        }
    }
    public void PrintCache()
    {
        for(int i = 0; i < nVertices; i++) {
            for(int j = 0; j < nVertices; j++) {
                if(pathCache[i,j] != null)
                    Debug.Log("Path: " + i + "," + j + " : " + pathCache[i, j].StringTo());
            }

        }

    }
}
