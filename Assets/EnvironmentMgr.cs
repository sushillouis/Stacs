using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class StartingPoint
{
    public Vector3 position;
    public Vector3 eulerAngles;

    public StartingPoint(Vector3 pos, Vector3 eul)
    {
        position = pos;
        eulerAngles = eul;
    }
}

[System.Serializable]
public class Environment
{
    public GameObject envObject;
    public StartingPoint camPoint;
    public List<StartingPoint> climbingPositions;
    public List<StartingPoint> dronePositions;
}

public class EnvironmentMgr : MonoBehaviour
{
    public static EnvironmentMgr inst;

    private void Awake()
    {
        if(inst == null)
        {
            inst = this;
        }
        if(Time.realtimeSinceStartup > 5.0f)
        {
            environment = SettingsMgr.environment;
        }
    }

    public Environment current;
    public List<Environment> environments;

    public int environment;

    // Start is called before the first frame update
    void Start()
    {
        SetEnvironment();
    }

    void SetEnvironment()
    {
        foreach (Environment e in environments)
        {
            if (e == environments[environment])
            {
                e.envObject.SetActive(true);
                current = e;
            }
            else
                e.envObject.SetActive(false);
        }
        GameObject.Find("EntityMgr").GetComponent<EntityMgr>().PlaceEntities();
    }

    [ContextMenu("Set Starting Points")]
    public void SetStartingPoints()
    {
        Environment env = environments[environment];
        env.climbingPositions.Clear();
        env.dronePositions.Clear();

        foreach(StacsEntity e in GameObject.Find("EntityMgr").GetComponent<EntityMgr>().entities)
        {
            Vector3 pos;
            Vector3 eul;
            switch(e.entityType)
            {
                case EntityType.Camera:
                    pos = e.transform.position;
                    eul = e.transform.eulerAngles;
                    env.camPoint = new StartingPoint(pos, eul);
                    break;
                case EntityType.ClimbingRobot:
                    pos = e.transform.position;
                    eul = e.transform.eulerAngles;
                    env.climbingPositions.Add(new StartingPoint(pos, eul));
                    break;
                case EntityType.ParrotDrone:
                    pos = e.transform.position;
                    eul = e.transform.eulerAngles;
                    env.dronePositions.Add(new StartingPoint(pos, eul));
                    break;
            }
        }
    }

    [ContextMenu("Select Environment/UNR Bridge")]
    public void SelectEnvironment_UNRBridge()
    {
        environment = 0;
        SetEnvironment();
    }

    [ContextMenu("Select Environment/Highway")]
    public void SelectEnvironment_Highway()
    {
        environment = 1;
        SetEnvironment();
    }
}
