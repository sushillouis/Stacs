using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefectsGenerator : MonoBehaviour
{
    public int numDefects;
    public GameObject defectObject;

    private BridgeBuilder bridgeBuilder;
    private GameObject primaryObject;

    // Start is called before the first frame update
    void Awake()
    {
        bridgeBuilder = transform.GetComponent<BridgeBuilder>();
        MakeRootLandObject();
    }

    public void MakeRootLandObject()
    {
        primaryObject = new GameObject();
        primaryObject.name = "Defects";
    }

    public void Generate()
    {
        Clear();
        for (int i = 0; i < numDefects; i++)
        {
            int ei = (int) Mathf.Floor(Random.Range(0, bridgeBuilder.edges.Count - 1));
            GameObject edge = bridgeBuilder.edges[ei].transform.gameObject;

            GameObject defect = Instantiate(defectObject);
            defect.transform.parent = primaryObject.transform;
            Vector3 offsetOnEdge = edge.transform.forward * Random.Range(-edge.transform.localScale.z * 0.5f, edge.transform.localScale.z * 0.5f);
            defect.transform.position = edge.transform.position + offsetOnEdge;
        }
    }

    void Clear()
    {
        Destroy(primaryObject);
        MakeRootLandObject();
    }

}
