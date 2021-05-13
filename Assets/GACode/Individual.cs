using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class FitnessComparer : IComparer
{
    public int Compare(object x, object y)
    {
        return (int) (((Individual)y).fitness - ((Individual)x).fitness);
    }
}

public class Individual
{
    public Options options;
    public int[] chromosome = new int[Constants.MAX_CHROMOSOME_LENGTH];
    public int chromosomeLength = 0;
    public float fitness;
    public float objectiveFunction;

    public Individual(Options opts)
    {
        options = opts;
        chromosomeLength = opts.chromosomeLength; //opts.graph.nEdges + opts.nRobots;
        //InitBinaryChromosome();
        InitIntegerChromosome();
    }
    
    public void InitBinaryChromosome()
    {
        for(int i = 0; i < chromosomeLength; i++) {
            chromosome[i] = (GAUtils.Flip(0.5f) ? 1 : 0);
        }
    }

    public void InitIntegerChromosome()
    {
        for(int i = 0; i < chromosomeLength; i++) {
            chromosome[i] = i;
        }
        for(int i = 0; i < chromosomeLength; i++) {
            Swap(GAUtils.RandInt(0, chromosomeLength), GAUtils.RandInt(0, chromosomeLength));
        }
    }

    public void Copy(Individual other)
    {
        fitness = other.fitness;
        objectiveFunction = other.objectiveFunction;
        chromosomeLength = other.chromosomeLength;
        for(int i = 0; i < chromosomeLength; i++) {
            chromosome[i] = other.chromosome[i];
        }
    }

    public int RobotId(int n)
    {
        //return n % nEdges;
        return 0;
    }


    public void Swap(int i, int j)
    {
        int tmp = chromosome[i];
        chromosome[i] = chromosome[j];
        chromosome[j] = tmp;
    }

    public void SwapMutate(float prob)
    {//swap two locations
        if(GAUtils.Flip(prob)) {
            Swap(GAUtils.RandInt(0, chromosomeLength), GAUtils.RandInt(0, chromosomeLength));
        }
    }

    public void ReverseMutate(float prob)
    {//reverse a slice of the string
        if(GAUtils.Flip(prob)) {
            int x1 = GAUtils.RandInt(0, chromosomeLength);
            int x2 = GAUtils.RandInt(0, chromosomeLength);
            int start = System.Math.Min(x1, x2);
            int end = System.Math.Max(x1, x2);

            int j = end;
            for(int i = start; i < j; i++, j--) {
                Swap(i, j);
            }
        }
    }

    public void Mutate(float prob)
    {
        for(int i = 0; i < options.chromosomeLength; i++)
        {
            if(GAUtils.Flip(prob))
            {
                chromosome[i] = 1 - chromosome[i];
            }
        }
    }

    public string StringTo()
    {
        string tmp = "";
        for(int i = 0; i < options.chromosomeLength; i++)
        {
            tmp += chromosome[i].ToString("0");
        }
        tmp += "\nFit: " + fitness.ToString("0.0") + "\n";
        return tmp;
    }

    public int[] CreateRouteChromosome()
    {
        int[] rc = new int[chromosomeLength];
        int index = FindIndexOfFirstRobot(chromosome);
        if(index < 0) {
                Debug.Log("Cannot find any robots, expecting: " + options.nRobots);
                options.nRobots = 1;
        }
        for(int i = 0; i < chromosomeLength; i++) {
            rc[i] = chromosome[index];
            index = GAUtils.AddOneModulo(index, chromosomeLength);
        }

        return rc;
    }

    public int FindIndexOfFirstRobot(int[] chromosome)
    {
        for(int i = 0; i < chromosomeLength; i++) {
            if(IsRobotAtIndex(i, chromosome))
                return i;
        }
        return -1;
    }

    public bool IsRobotAtIndex(int i, int[] chrom)
    {
        //Debug.Assert(i < chromosomeLength);
        //Debug.Log("IsRobot: index: " + i + " val: " + routeChromosome[i]);
        return (chrom[i] >= options.graph.nEdges);
    }

    public List<RobotRoute> routes;
    public RobotRoute currentRoute;
    int[] routeChromosome;
    public float Evaluate()
    {
        //redo chromosome to start with robot
        routeChromosome = CreateRouteChromosome();
        routes = new List<RobotRoute>();
        //string tmp = "";
        //foreach(int x in routeChromosome) {
        //tmp += x.ToString("0") + ",";
        //}
        //Debug.Log("RouteChrom: " + tmp);

        //foreach(int r in routeChromosome) { //routeChromosome[0] must be robot
        for(int i = 0; i < chromosomeLength; i++) { 
            if(IsRobotAtIndex(i, routeChromosome)) {
                StartNewRoute(i);
            } else {
                AddEdgeToRoute(i);
            }
            //Debug.Log("PartRoute: " + i + ", e: " + routeChromosome[i] + ": " + routes[0].route.StringTo());
        }
        //Debug.Log("Evaluation Route: " + routes[0].route.StringTo());
        return routes[0].route.length; ;
    }

    public void StartNewRoute(int index)
    {//r is a robot
        //Debug.Log("StartNewRoute for: " + routeChromosome[index] + " at index: " + index);
        RobotRoute route = new RobotRoute(routeChromosome[index]);
        routes.Add(route);
        currentRoute = route;

    }

    public void HandleFirstEdgeDirection(int index)
    {//What is the first vertex? v1 or v2. Depends on the distance from each
     //Vertex to the next edge, so get next edge, check distance between all four vertices
     //then pick vertex that is shortest distance to v11 or v22 as the SECOND vertex
        int edgeIndex = routeChromosome[index];
        Edge e = options.graph.edges[edgeIndex];
        int v1 = e.vertex1;
        int v2 = e.vertex2;

        if(index + 1 < chromosomeLength) {
            if(!IsRobotAtIndex(index + 1, routeChromosome)) {
                int nextEdgeIndex = routeChromosome[index + 1];
                int v11 = options.graph.edges[nextEdgeIndex].vertex1;
                int v22 = options.graph.edges[nextEdgeIndex].vertex2;
                List<GraphPath> paths = new List<GraphPath>
                {
                options.graph.pathCache[v1, v11],
                options.graph.pathCache[v1, v22],
                options.graph.pathCache[v2, v11],
                options.graph.pathCache[v2, v22]
                };
                GraphPath minPath = FindMinLengthPath(paths);
                if(minPath == null) {
                    Debug.Log("Fatal Error. MinPath does not exist");
                    throw new System.Exception("Min Path does not exist");
                } 
                if(minPath.vertices[0].vertex == v1) { // if v1 is closer to second edge
                    currentRoute.route.vertices.Add(new GraphVertex(v2, 0)); // start at v2 and end at v1
                    currentRoute.route.vertices.Add(new GraphVertex(v1, e.length)); // so you can get to 2nd edge
                } else {
                    currentRoute.route.vertices.Add(new GraphVertex(v1, 0));
                    currentRoute.route.vertices.Add(new GraphVertex(v2, e.length));
                }
                currentRoute.route.length += e.length;
            }
        } else {//True => IsRobotAtIndex(index+1) so just add a one edge route
            currentRoute.route.vertices.Add(new GraphVertex(v1, 0)); // or v2
            currentRoute.route.vertices.Add(new GraphVertex(v2, e.length));
            currentRoute.route.length += e.length;
        }
    }

    public GraphPath FindMinLengthPath(List<GraphPath> paths)
    {//uses the fact that legal paths have length >= 0. 
     //Path from vertex to itself has length -1
        float min = float.MaxValue;
        GraphPath minPath = null;
        foreach(GraphPath p in paths) {
            if(p != null) { //ensures checked first
                if(p.length < min) {
                    min = p.length;
                    minPath = p;
                }
            }
        }
        return minPath;
    }

    public void AddEdgeToRoute(int index)
    {// if empty route handle first edge, else
        //pick shorter path distance vertex of edge to add to route
        if(currentRoute.route.length == 0) {
            HandleFirstEdgeDirection(index);
        } else {
            HandleAddEdgeToRoute(index);
        }
    }

    public void HandleAddEdgeToRoute(int index)
    {
        Edge e = options.graph.edges[routeChromosome[index]];
        //Get route endpoint vertex
        GraphVertex start = currentRoute.route.vertices[currentRoute.route.vertices.Count - 1];

        if(IsAdjacentEdge(e, start)) {
            AddAdjacentEdge(e, start);
        } else {

            //Debug.Log("Adding to vertex: " + start.vertex);
            //Check both vertices of edge to see what is shortest path
            List<GraphPath> paths = new List<GraphPath>{
            options.graph.pathCache[start.vertex, e.vertex1],
            options.graph.pathCache[start.vertex, e.vertex2]
            };

            GraphPath minPath = FindMinLengthPath(paths);
            currentRoute.AddPath(minPath); //Also adds path length
            if(minPath.vertices[minPath.vertices.Count - 1].vertex == e.vertex1) {
                currentRoute.route.vertices.Add(new GraphVertex(e.vertex2, e.length));
            } else {
                currentRoute.route.vertices.Add(new GraphVertex(e.vertex1, e.length));
            }
            currentRoute.route.length += e.length;
        }
    }
    
    public bool IsAdjacentEdge(Edge e, GraphVertex start)
    {
        return (start.vertex == e.vertex1 || start.vertex == e.vertex2);
    }

    public void AddAdjacentEdge(Edge e, GraphVertex start)
    {
        if(start.vertex == e.vertex1) {
            currentRoute.route.vertices.Add(new GraphVertex(e.vertex2, e.length));
        } else if (start.vertex == e.vertex2) {
            currentRoute.route.vertices.Add(new GraphVertex(e.vertex1, e.length));
        }
        currentRoute.route.length += e.length;
    }
}
