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

    public float maxZ = 1000;
    public float maxX = 1000;

    public override void Awake()
    {
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
    }

    public void Start()
    {
        Generate();
    }

    public override void Clear()
    {
        base.Clear();
        surfaceGenerator.Clear();
        bridgeGenerator.Clear();
        defectsGenerator.Clear();
        landGenerator.Clear();
        robotGenerator.Clear();
    }

    public override void Generate()
    {
        surfaceGenerator.Generate();
        bridgeGenerator.Generate();
        defectsGenerator.Generate();
        landGenerator.Generate();
        robotGenerator.Generate();
    }
}
