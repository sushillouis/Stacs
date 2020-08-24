using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefectManager : MonoBehaviour
{
    public Transform defectLocationsRoot;
    public List<Transform> defectLocations;
    public GameObject defect;
    public int numDefects;

    void Start()
    {
        foreach(Transform t in defectLocationsRoot.GetComponentsInChildren<Transform>())
        {
            if(t == defectLocationsRoot)
                continue;
            defectLocations.Add(t);
        }

        bool[] occupiedSpaces = new bool[defectLocations.Count];
        for(int i = 0; i < occupiedSpaces.Length; i++)
        {
            occupiedSpaces[i] = false;
        }

        for(int i = 0; i < numDefects; i++)
        {
            int loc;
            do
            {
                loc = Random.Range(0, defectLocations.Count);
            } while(occupiedSpaces[loc] == true);

            Instantiate(defect, defectLocations[loc]);
            occupiedSpaces[loc] = true;
        }
    }
}
