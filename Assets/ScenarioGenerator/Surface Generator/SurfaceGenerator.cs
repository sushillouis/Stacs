using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceGenerator : Generator
{
    public GameObject surface;
    public GameObject sidewalk;
    public GameObject railing;

    private float surfaceWidth;

    private float roadWidth = 3.7f; // meters
    private float sidewalkWidth = 1.2192f; // meters

    public ScenarioGenerator scenarioGenerator;


    public override void Awake()
    {
        rootObjectName = "Surface";
        base.Awake();
    }

    public float GetWidth()
    {
        return surfaceWidth;
    }

    public override void Generate()
    {
        base.Generate();
        float maxLength = scenarioGenerator.maxX;
        MakeSurface(maxLength);
    }

    private void MakeSurface(float maxLength)
    {
        // surface types
        string type = "Single Lane";
        switch (type)
        {
            case "Single Lane":
                MakeRoad(2, maxLength);
                break;
        }

        MakeSidewalk(maxLength);
    }

    private void MakeRoad(int numLanes, float maxLength)
    {
        surfaceWidth = roadWidth * numLanes;
        float startZPos = (roadWidth * numLanes);
        GameObject go = Instantiate(surface);
        go.transform.position = new Vector3(0, 0, 0);
        go.transform.localScale = new Vector3(maxLength, 1, roadWidth * numLanes);
        go.transform.parent = rootObject.transform;
    }

    private void MakeSidewalk(float maxLength)
    {
        GameObject go = Instantiate(sidewalk);
        go.transform.position = new Vector3(0, 0, (surfaceWidth / 2) + (sidewalkWidth / 2));
        go.transform.localScale = new Vector3(maxLength, sidewalkWidth, 1);
        go.transform.parent = rootObject.transform;

        go = Instantiate(sidewalk);
        go.transform.position = new Vector3(0, 0, -(surfaceWidth / 2) - (sidewalkWidth / 2));
        go.transform.localScale = new Vector3(maxLength, sidewalkWidth, 1);
        go.transform.parent = rootObject.transform;

        surfaceWidth += 2 * sidewalkWidth;
    }
}
