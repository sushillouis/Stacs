using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandBuilder : MonoBehaviour
{
    public GameObject land;
    public GameObject bank;
    private float bankHeight = 4;
    private float bankWidth = 4;
    private GameObject primaryObject;

    void Awake()
    {
        MakeRootLandObject();
    }

    public void MakeRootLandObject()
    {
        primaryObject = new GameObject();
        primaryObject.name = "Land";
    }

    public void Generate(float maxX, float maxZ)
    {
        Clear();
        MakeLand(maxX, maxZ);
        MakeBanks(maxX, maxZ);
    }

    public void Clear()
    {
        Destroy(primaryObject);
        MakeRootLandObject();
    }

    private void MakeLand(float maxX, float maxZ)
    {
        GameObject go = Instantiate(land);
        go.transform.position = new Vector3(0, -bankHeight, 0);
        go.transform.localScale = new Vector3(maxX, maxZ, 1);
        go.transform.parent = primaryObject.transform;

        float landSizeX = (maxZ - maxX) / 2.0f;
        go = Instantiate(land);
        go.transform.position = new Vector3(.5f * maxX + .5f * landSizeX, 0, 0);
        go.transform.localScale = new Vector3(landSizeX, maxZ, 1);
        go.transform.parent = primaryObject.transform;

        go = Instantiate(land);
        go.transform.position = new Vector3(-.5f * maxX + -.5f * landSizeX, 0, 0);
        go.transform.localScale = new Vector3(landSizeX, maxZ, 1);
        go.transform.parent = primaryObject.transform;
    }

    private void MakeBanks(float maxX, float maxZ)
    {
        GameObject go = Instantiate(bank);
        go.transform.position = new Vector3((maxX / 2.0f) - (0.5f * bankWidth), 0, 0);
        go.transform.localScale = new Vector3(bankWidth, maxZ, bankHeight);
        go.transform.parent = primaryObject.transform;

        go = Instantiate(bank);
        go.transform.position = new Vector3(-(maxX / 2.0f) + (0.5f * bankWidth), 0, 0);
        go.transform.localScale = new Vector3(bankWidth, maxZ, bankHeight);
        go.transform.localEulerAngles = new Vector3(-90, 0, 180);
        go.transform.parent = primaryObject.transform;
    }
}
