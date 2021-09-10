using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefectManager : MonoBehaviour
{
    public Transform defectLocationsRoot;
    public List<Transform> defectLocations;
    public List<Material> defectMaterials;
    public GameObject defect;
    public int numDefects;

    void Start()
    {
        foreach (Transform t in defectLocationsRoot.GetComponentsInChildren<Transform>())
        {
            if (t == defectLocationsRoot)
                continue;
            defectLocations.Add(t);
        }

        bool[] occupiedSpaces = new bool[defectLocations.Count];
        for (int i = 0; i < occupiedSpaces.Length; i++)
        {
            occupiedSpaces[i] = false;
        }

        if (numDefects > defectLocations.Count)
        {
            numDefects = defectLocations.Count;
        }

        for (int i = 0; i < numDefects; i++)
        {
            int loc;
            do
            {
                loc = Random.Range(0, defectLocations.Count);
            } while (occupiedSpaces[loc] == true);

            GameObject d = Instantiate(defect, defectLocations[loc]);
            d.GetComponent<MeshRenderer>().material = defectMaterials[Random.Range(0, defectMaterials.Count)];
            float rot = Random.Range(-180.0f, 180.0f);
            d.transform.Rotate(0.0f, 0.0f, rot, Space.World);
            occupiedSpaces[loc] = true;
        }
    }
}