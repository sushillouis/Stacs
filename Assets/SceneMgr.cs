using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


[System.Serializable]
public class Waypoint
{
    public Vector3 position;
    public string name;

    public Waypoint(Vector3 position, string name = null)
    {
        this.position = position;
        this.name = name;
    }
}


[System.Serializable]
public class Route
{
    public List<Waypoint> Waypoints;
}

[System.Serializable]
public class RouteCategory
{
    public List<Route> Routes;
}

public class SceneMgr : MonoBehaviour
{
    public static SceneMgr inst;
    private void Awake()
    {
        inst = this;
    }

    public bool isInspecting;
    public StacsEntity DefaultParrotDrone;


    public GameObject DroneWaypointsRoot;
    public GameObject ClimbingRobotWaypointsRoot;
    public RouteCategory DroneRoutes;
    public RouteCategory ClimbingRobotRoutes;

    [ContextMenu("MakeDroneRoutes")]
    public void MakeDroneRoutes()
    {
        DroneRoutes.Routes.Clear();
        foreach(Transform t in DroneWaypointsRoot.GetComponentsInChildren<Transform>())
        {
            if(t.name.StartsWith("Drone"))
            {
                Route r = new Route();
                r.Waypoints = new List<Waypoint>();
                foreach(Transform tc in t.GetComponentsInChildren<Transform>())
                {
                    if(tc.name.StartsWith("Cube"))
                    {
                        r.Waypoints.Add(new Waypoint(tc.position, tc.gameObject.name));
                    }
                }
                DroneRoutes.Routes.Add(r);
            }
        }
        SaveRoutes(DroneRoutes, "droneRoutes.json");
    }

    [ContextMenu("MakeClimbinRobotRoutes")]
    public void MakeClimbingRobotRoutes()
    {
        ClimbingRobotRoutes.Routes.Clear();
        foreach(Transform t in ClimbingRobotWaypointsRoot.GetComponentsInChildren<Transform>())
        {
            if(t.name.StartsWith("Robot"))
            {
                Route r = new Route();
                r.Waypoints = new List<Waypoint>();
                foreach(Transform tc in t.GetComponentsInChildren<Transform>())
                {
                    if(tc.name.StartsWith("Cube"))
                    {
                        r.Waypoints.Add(new Waypoint(tc.position, tc.gameObject.name));
                    }
                }
                ClimbingRobotRoutes.Routes.Add(r);
            }
        }
        SaveRoutes(ClimbingRobotRoutes, "climbingRobotRoutes.json");
    }

    [ContextMenu("ReadDroneRoutes")]
    public void ReadDroneRoutes()
    {
        string json = File.ReadAllText(Application.dataPath + "/Routes/droneRoutes.json");
        DroneRoutes = JsonUtility.FromJson<RouteCategory>(json);
    }

    [ContextMenu("ReadClimbingRobotRoutes")]
    public void ReadClimbingRobotRoutes()
    {
        string json = File.ReadAllText(Application.dataPath + "/Routes/climbingRobotRoutes.json");
        ClimbingRobotRoutes = JsonUtility.FromJson<RouteCategory>(json);
    }

    // Start is called before the first frame update
    void Start()
    {
        MakeClimbingRobotRoutes();
        ClimbingRobotWaypointsRoot.SetActive(false);
        DroneRoutes.Routes = new List<Route>();
        ClimbingRobotRoutes.Routes = new List<Route>();
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }


    public void RunRoute(StacsEntity entity = null)
    {
        if(entity == null)
        {
            entity = DefaultParrotDrone;
        } 

        Vector3 returnPos = entity.position;
        UnitAI uai = entity.GetComponent<UnitAI>();
        uai.StopAndRemoveAllCommands();
        Move m = null;
        foreach (Waypoint w in DroneRoutes.Routes[0].Waypoints)
        {
            m = new Move(entity, w.position);
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
            RunClimbingRobotRoute(ClimbingRobots[i], ClimbingRobotRoutes.Routes[i]);
        }
    }

    public void RunClimbingRobotRoute(StacsEntity ent, Route route)
    {
        UnitAI uai = ent.GetComponent<UnitAI>();
        uai.StopAndRemoveAllCommands();
        TrussMove tm = null;
        foreach(Waypoint w in route.Waypoints)
        {
            tm = new TrussMove(ent, w.position);
            uai.AddCommand(tm);
        }
        isInspecting = true;
    }

    public void SaveRoutes(RouteCategory routes, string file)
    {
        string json = JsonUtility.ToJson(routes, true);
        Debug.Log(json);
        File.WriteAllText(Application.dataPath + "/Routes/" + file, json);
    }

}
