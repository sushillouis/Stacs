using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Route
{
    public List<GameObject> Waypoints;
}

public class SceneMgr : MonoBehaviour
{
    public static SceneMgr inst;
    private void Awake()
    {
        inst = this;
    }

    public GameObject DroneWaypoints;
    public GameObject ClimbingRobotWaypointsRoot;

    public List<Route> DroneRoutes = new List<Route>();

    public List<Route> ClimbingRobotRoutes = new List<Route>();

    [ContextMenu("MakeDroneRoutes")]
    public void MakeDroneRoutes()
    {
        DroneRoutes.Clear();
        Route r = new Route();
        r.Waypoints = new List<GameObject>();

        foreach (Transform t in DroneWaypoints.GetComponentsInChildren<Transform>())
        {
            if (t.name.StartsWith("Cube"))
                r.Waypoints.Add(t.gameObject);
        }
        DroneRoutes.Add(r);
    }

    [ContextMenu("MakeClimbinRobotRoutes")]
    public void MakeClimbingRobotRoutes()
    {
        ClimbingRobotRoutes.Clear();
        foreach(Transform t in ClimbingRobotWaypointsRoot.GetComponentsInChildren<Transform>())
        {
            if(t.name.StartsWith("Robot"))
            {
                Route r = new Route();
                r.Waypoints = new List<GameObject>();
                foreach(Transform tc in t.GetComponentsInChildren<Transform>())
                {
                    if(tc.name.StartsWith("Cube"))
                    {
                        r.Waypoints.Add(tc.gameObject);
                    }
                }
                ClimbingRobotRoutes.Add(r);
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {

    }
    
    // Update is called once per frame
    void Update()
    {
        
    }

    public bool isInspecting;
    public void RunRoute(StacsEntity entity)
    {
        Vector3 returnPos = entity.position;
        UnitAI uai = entity.GetComponent<UnitAI>();
        uai.StopAndRemoveAllCommands();
        Move m = null;
        foreach (GameObject go in DroneRoutes[0].Waypoints)
        {
            m = new Move(entity, go.transform.position);
            uai.AddCommand(m);
        }
        m = new Move(entity, returnPos);
        uai.AddCommand(m);
        isInspecting = true;
    }

    public List<StacsEntity> ClimbingRobots; // Set in scene manager, routes and waypoints are in order...
    public void RunClimbingRobotRoutes()
    {
        for(int i = 0; i < ClimbingRobots.Count; i++)
        {
            RunClimbingRobotRoute(ClimbingRobots[i], ClimbingRobotRoutes[i]);
        }
    }

    public void RunClimbingRobotRoute(StacsEntity ent, Route route)
    {
        UnitAI uai = ent.GetComponent<UnitAI>();
        uai.StopAndRemoveAllCommands();
        TrussMove tm = null;
        foreach(GameObject go in route.Waypoints)
        {
            tm = new TrussMove(ent, go.transform.position);
            uai.AddCommand(tm);
        }
        isInspecting = true;
    }

}
