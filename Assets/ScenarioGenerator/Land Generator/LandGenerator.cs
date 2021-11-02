using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandGenerator : Generator
{
    public GameObject land;
    public GameObject bank;
    public float canalDepth = 1;
    private float bankHeight = 4;
    private float bankWidth = 4;
    private float canalWidth;

    public ScenarioGenerator scenarioGenerator;

    public override void Awake()
    {
        rootObjectName = "Land";
        base.Awake();
    }
    public override void Generate()
    {
        base.Generate();

        float maxX = scenarioGenerator.maxX;
        float maxZ = scenarioGenerator.maxZ;
        bankHeight = canalDepth;
        bankWidth = (float) Random.RandomRange(canalDepth, canalDepth + 2);
        canalWidth = scenarioGenerator.bridgeGenerator.GetLength();
        MakeLand(maxX, maxZ);
        MakeBanks(maxX, maxZ);
    }

    private void MakeLand(float maxX, float maxZ)
    {
        GameObject go = Instantiate(land);
        go.transform.position = new Vector3(0, -bankHeight, 0);
        go.transform.localScale = new Vector3(maxX, maxZ, 1);
        go.transform.parent = rootObject.transform;

        float landSizeX = (maxX - canalWidth) / 2.0f;
        go = Instantiate(land);
        go.transform.position = new Vector3(.5f * maxX - .5f * landSizeX, 0, 0);
        go.transform.localScale = new Vector3(landSizeX, maxZ, 1);
        go.transform.parent = rootObject.transform;

        go = Instantiate(land);
        go.transform.position = new Vector3(-.5f * maxX + .5f * landSizeX, 0, 0);
        go.transform.localScale = new Vector3(landSizeX, maxZ, 1);
        go.transform.parent = rootObject.transform;
    }

    private void MakeBanks(float maxX, float maxZ)
    {
        GameObject go = Instantiate(bank);
        go.transform.position = new Vector3((0.5f * canalWidth) - (0.5f * bankWidth), 0, 0);
        go.transform.localScale = new Vector3(bankWidth, maxZ, bankHeight);
        go.transform.parent = rootObject.transform;

        go = Instantiate(bank);
        go.transform.position = new Vector3(-(0.5f * canalWidth) + (0.5f * bankWidth), 0, 0);
        go.transform.localScale = new Vector3(bankWidth, maxZ, bankHeight);
        go.transform.localEulerAngles = new Vector3(-90, 0, 180);
        go.transform.parent = rootObject.transform;
    }
}


