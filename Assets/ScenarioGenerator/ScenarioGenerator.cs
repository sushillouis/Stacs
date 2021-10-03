using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioGenerator : MonoBehaviour
{
    public GameObject builderObject;

    private SurfaceBuilder surfaceBuilder;
    private BridgeBuilder bridgeBuilder;
    private LandBuilder landBuilder;

    private float maxZ = 1000;
    private float maxX = 1000;

    private void Awake()
    {
        surfaceBuilder = builderObject.GetComponent<SurfaceBuilder>();
        bridgeBuilder = builderObject.GetComponent<BridgeBuilder>();
        landBuilder = builderObject.GetComponent<LandBuilder>();
    }

    public void Start()
    {
        Generate();
    }

    public void Generate()
    {
        surfaceBuilder.Generate(maxX);
        bridgeBuilder.Generate(surfaceBuilder.GetWidth());
        landBuilder.Generate(bridgeBuilder.GetLength(), maxZ);
    }
}
