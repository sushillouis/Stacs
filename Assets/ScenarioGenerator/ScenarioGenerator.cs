using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioGenerator : Generator
{
    public SurfaceGenerator surfaceGenerator;
    public BridgeGenerator bridgeGenerator;
    public DefectsGenerator defectsGenerator;
    public LandGenerator landGenerator;
    public RobotGenerator robotGenerator;
    public WaypointGenerator waypointGenerator;

    public Graphv2 graph;

    private List<Generator> generators;

    public float maxZ = 1000;
    public float maxX = 1000;

    public override void Awake()
    {
        generators = new List<Generator>();
        foreach (var component in transform.GetComponents<Generator>())
        {
            if (component != this)
            {
                generators.Add(component);
            }
        }

        surfaceGenerator = transform.GetComponent<SurfaceGenerator>();
        surfaceGenerator.scenarioGenerator = this;
        bridgeGenerator = transform.GetComponent<BridgeGenerator>();
        bridgeGenerator.scenarioGenerator = this;
        defectsGenerator = transform.GetComponent<DefectsGenerator>();
        defectsGenerator.scenarioGenerator = this;
        landGenerator = transform.GetComponent<LandGenerator>();
        landGenerator.scenarioGenerator = this;
        robotGenerator = transform.GetComponent<RobotGenerator>();
        robotGenerator.scenarioGenerator = this;

        //graph = transform.GetComponent<Graphv2>();
        
        /*        waypointGenerator = transform.GetComponent<WaypointGenerator>();
                waypointGenerator.scenarioGenerator = this;
                graphGenerator = transform.GetComponent<GraphGenerator>();
                graphGenerator.scenarioGenerator = this;*/

    }

    public void Start()
    {
        Generate();
    }

    public override void Clear()
    {
        base.Clear();
        foreach (var generator in generators)
        {
            generator.Clear();
        }
  /*      surfaceGenerator.Clear();
        bridgeGenerator.Clear();
        defectsGenerator.Clear();
        landGenerator.Clear();
        robotGenerator.Clear();
        waypointGenerator.Clear();
        graphGenerator.Clear();*/
    }

    public void GenerateEasy()
    {

        
        bridgeGenerator.numSegments = Random.Range(4, 6);
        switch(Random.Range(1, 4))
        {
            case 1:
                bridgeGenerator.bridgeType = BridgeGenerator.BridgeType.Howe;
                break;
            case 2:
                bridgeGenerator.bridgeType = BridgeGenerator.BridgeType.Pratt;
                break;
            case 3:
                bridgeGenerator.bridgeType = BridgeGenerator.BridgeType.Warren;
                break;
        }

        defectsGenerator.numDefects = Random.Range(1, 5);
        robotGenerator.numClimbingV1Robots = Random.Range(1, 3);
        robotGenerator.numClimbingV2Robots = Random.Range(1, 3);
        robotGenerator.numDrones = Random.Range(1, 3);
        landGenerator.canalDepth = Random.Range(2, 4);
        surfaceGenerator.numLanes = Random.Range(1, 3);
        Generate();
    }

    public void GenerateMedium()
    {
        bridgeGenerator.numSegments = Random.Range(7, 12);
        switch (Random.Range(1, 5))
        {
            case 1:
                bridgeGenerator.bridgeType = BridgeGenerator.BridgeType.Howe;
                break;
            case 2:
                bridgeGenerator.bridgeType = BridgeGenerator.BridgeType.Pratt;
                break;
            case 3:
                bridgeGenerator.bridgeType = BridgeGenerator.BridgeType.Warren;
                break;
            case 4:
                bridgeGenerator.bridgeType = BridgeGenerator.BridgeType.KTruss;
                break;
        }

        defectsGenerator.numDefects = Random.Range(5, 10);
        robotGenerator.numClimbingV1Robots = Random.Range(3, 6);
        robotGenerator.numClimbingV2Robots = Random.Range(3, 6);
        robotGenerator.numDrones = Random.Range(3, 6);
        landGenerator.canalDepth = Random.Range(4, 8);
        surfaceGenerator.numLanes = Random.Range(2, 4);

        Generate();
    }

    public void GenerateHard()
    {
        bridgeGenerator.numSegments = Random.Range(12, 16);
        switch (Random.Range(1, 4))
        {
            case 1:
                bridgeGenerator.bridgeType = BridgeGenerator.BridgeType.Howe;
                break;
            case 2:
                bridgeGenerator.bridgeType = BridgeGenerator.BridgeType.Pratt;
                break;
            case 3:
                bridgeGenerator.bridgeType = BridgeGenerator.BridgeType.Warren;
                break;
            case 4:
                bridgeGenerator.bridgeType = BridgeGenerator.BridgeType.KTruss;
                break;
        }

        defectsGenerator.numDefects = Random.Range(11, 15);
        robotGenerator.numClimbingV1Robots = Random.Range(6, 9);
        robotGenerator.numClimbingV2Robots = Random.Range(6, 9);
        robotGenerator.numDrones = Random.Range(6, 9);
        landGenerator.canalDepth = Random.Range(8, 12);
        surfaceGenerator.numLanes = Random.Range(3, 4);

        Generate();
    }

    public override void Generate()
    {
        //
        surfaceGenerator.Generate();
        bridgeGenerator.Generate();
        defectsGenerator.Generate();
        landGenerator.Generate();
        robotGenerator.Generate();

        // Make graph
        CreateGraphFromBridge();
    }

    public void CreateGraphFromBridge()
    {
        // Clear the graph
        graph.Clear();

        // Add all edges to graph to
        foreach(BridgeEdge edge in bridgeGenerator.edges)
        {
            graph.AddEdge(edge.id, edge.v1.id, edge.v2.id, edge.cost);
        }

        print(graph.ToString());
    }
}
