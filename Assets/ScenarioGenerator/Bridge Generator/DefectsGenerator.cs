using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefectsGenerator : Generator
{
    public int numDefects = 4;
    public GameObject defectObject;

    public ScenarioGenerator scenarioGenerator;
    public override void Awake()
    {
        rootObjectName = "Defects";
        base.Awake();
    }

    public override void Generate()
    {
        base.Generate();
        for (int i = 0; i < numDefects; i++)
        {
            int ei = (int) Mathf.Floor(Random.Range(0, scenarioGenerator.bridgeGenerator.edges.Count - 1));
            GameObject edge = scenarioGenerator.bridgeGenerator.edges[ei].transform.gameObject;

            GameObject defect = Instantiate(defectObject);
            defect.transform.parent = rootObject.transform;
            Vector3 offsetOnEdge = edge.transform.forward * Random.Range(-edge.transform.localScale.z * 0.5f, edge.transform.localScale.z * 0.5f);
            defect.transform.position = edge.transform.position + offsetOnEdge;
        }
    }
}
