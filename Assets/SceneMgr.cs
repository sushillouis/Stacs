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
    
    public Route(string route, List<Waypoint> allWaypoints)
    {
        Waypoints = new List<Waypoint>();
        string[] waypoints = route.Split(' ');
        for(int i = 1; i < waypoints.Length; i++)
        {
            Waypoints.Add(allWaypoints[int.Parse(waypoints[i])]);
            Debug.Log(waypoints[i]);
        }
    }
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
    public GameObject AllClimbingWaypointsRoot;
    public List<Route> DroneRoutes;
    public List<Route> ClimbingRobotRoutes;
    public List<Waypoint> AllClimbingWaypoints;
/*
    [ContextMenu("MakeDroneRoutes")]
    public void MakeDroneRoutes()
    {
        DroneRoutes.Clear();
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
*/
    [ContextMenu("GetAllClimbingWaypoints")]
    public void GetAllClimbingWaypoints()
    {
        AllClimbingWaypoints.Clear();

        for(int i = 0; i < AllClimbingWaypointsRoot.transform.childCount; i++)
        {
            Transform t = AllClimbingWaypointsRoot.transform.GetChild(i).transform;
            t.gameObject.GetComponent<Vertex>().UpdateInfo('v' + i.ToString());
            AllClimbingWaypoints.Add(new Waypoint(t.position, t.gameObject.name));
        }
    }

    [ContextMenu("ReadDroneRoutes")]
    public void ReadDroneRoutes()
    {
        string[] lines = File.ReadAllLines(Application.dataPath + "/Routes/droneRoutes.tsv");
        for(int i = 0; i < lines.Length; i++)
        {
            DroneRoutes.Add(new Route(lines[i], AllClimbingWaypoints));
        }
    }

    [ContextMenu("ReadClimbingRobotRoutes")]
    public void ReadClimbingRobotRoutes()
    {
        string[] lines = File.ReadAllLines(Application.dataPath + "/Routes/climbingRobotRoutes.tsv");
        for(int i = 0; i < lines.Length; i++)
        {
            Debug.Log(lines[i]);
            ClimbingRobotRoutes.Add(new Route(lines[i], AllClimbingWaypoints));
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //MakeClimbingRobotRoutes();
        ClimbingRobotWaypointsRoot.SetActive(false);
        DroneRoutes = new List<Route>();
        ClimbingRobotRoutes = new List<Route>();
        GetAllClimbingWaypoints();
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
        foreach (Waypoint w in DroneRoutes[0].Waypoints)
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
            RunClimbingRobotRoute(ClimbingRobots[i], ClimbingRobotRoutes[i]);
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
        File.WriteAllText(Application.dataPath + "/Routes/" + file, json);
    }

}

/****METHODS FOR READING FROM JSON, MAY NEED IN THE FUTURE****/
/*
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
*/
