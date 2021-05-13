using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


[System.Serializable]
public class Waypoint
{
    public GameObject go;
    public Transform transform;
    public Vector3 position
    {
        get
        {return transform.position;}
    }
    public string name;

    public Waypoint(Transform transform, string name = null)
    {
        this.transform = transform;
        this.name = name;
    }

    public Waypoint(Vector3 pos, Vector3 norm)
    {
        go = GameObject.Instantiate(new GameObject());
        go.transform.position = pos;
        go.transform.up = norm;
        this.transform = go.transform;
    }
}

[System.Serializable]
public class Route
{
    public List<Waypoint> Waypoints;
    
    public Route()
    {
        Waypoints = new List<Waypoint>();
    }

    public Route(string route, List<Waypoint> allWaypoints)
    {
        Waypoints = new List<Waypoint>();
        string[] waypoints = route.Split(' ');
        for(int i = 1; i < waypoints.Length; i++)
        {
            Debug.Log(allWaypoints[int.Parse(waypoints[i])].name);
            Waypoints.Add(allWaypoints[int.Parse(waypoints[i])]);
        }
    }
}

//QUESTION: Is this still necessary?
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
    public List<StacsEntity> ClimbingRobots; // Set in scene manager, routes and waypoints are in order...
    public List<StacsEntity> Drones;


    public GameObject DroneRoutesRoot;
    public GameObject ClimbingRobotWaypointsRoot;
    public GameObject AllClimbingWaypointsRoot;
    public List<Route> DroneRoutes = new List<Route>();
    public List<Route> ClimbingRobotRoutes = new List<Route>();
    public List<Waypoint> AllClimbingWaypoints = new List<Waypoint>();

    public string routeOutput;

    [ContextMenu("GetAllClimbingWaypoints")]
    public void GetAllClimbingWaypoints()
    {
        AllClimbingWaypoints.Clear();

        for(int i = 0; i < AllClimbingWaypointsRoot.transform.childCount; i++)
        {
            Transform t = AllClimbingWaypointsRoot.transform.GetChild(i).transform;
            t.gameObject.GetComponent<Vertex>().UpdateInfo('v' + i.ToString());
            AllClimbingWaypoints.Add(new Waypoint(t, t.gameObject.name));
        }
    }

    public void GetClimbingRobots()
    {
        ClimbingRobots.Clear();
        foreach(StacsEntity e in EntityMgr.inst.entities)
        {
            if(e.GetComponent<ClimbingPhysics>() != null)
            {
                ClimbingRobots.Add(e);
            }
        }
    }

    public void GetDrones()
    {
        Drones.Clear();
        foreach(StacsEntity e in EntityMgr.inst.entities)
        {
            if(e.GetComponent<OrientedFlyingPhysics>() != null)
            {
                Drones.Add(e);
            }
        }
    }

    [ContextMenu("ReadDroneRoutes")]
    public void ReadDroneRoutes()
    {
        DroneRoutes.Clear();
        string[] lines = File.ReadAllLines(Application.dataPath + "/Routing/droneRoutes.tsv");
        for(int i = 0; i < lines.Length; i++)
        {
            DroneRoutes.Add(new Route(lines[i], AllClimbingWaypoints));
        }
    }

    [ContextMenu("ReadClimbingRobotRoutes")]
    public void ReadClimbingRobotRoutes()
    {
        ClimbingRobotRoutes.Clear();
        string[] lines = routeOutput.Split('\n');//File.ReadAllLines(Application.dataPath + "/MMkCPP/" + climbingRouteFile);
        for(int i = 0; i < lines.Length; i++)
        {
            if(lines[i].Length > 0)
            {
                Debug.Log("line i: " + lines[i]);
                ClimbingRobotRoutes.Add(new Route(lines[i], AllClimbingWaypoints));
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //MakeClimbingRobotRoutes();
        ClimbingRobotWaypointsRoot.SetActive(false);
        GetAllClimbingWaypoints();
        GetClimbingRobots();
        GetDrones();
        MakeDroneRoutes();
        //This line may be annoying later, but it makes the video better
        AllClimbingWaypointsRoot.SetActive(false);
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

    public void RunClimbingRobotRoutes()
    {
        for(int i = 0; i < ClimbingRobotRoutes.Count; i++)
        {
            RunClimbingRobotRoute(ClimbingRobots[i], ClimbingRobotRoutes[i]);
        }
    }

    public void RunDroneRoutes()
    {
        for(int i = 0; i < DroneRoutes.Count; i++)
        {
            RunDroneRoute(Drones[i], DroneRoutes[i]);
        }
    }

    public void RunClimbingRobotRoute(StacsEntity ent, Route route)
    {
        UnitAI uai = ent.GetComponent<UnitAI>();
        uai.StopAndRemoveAllCommands();
        TrussMove tm = null;
        int i = 0;
        foreach(Waypoint w in route.Waypoints)
        {
            if(w == route.Waypoints[0])
            {
                uai.Teleport(w);
            }
            else
            {
                tm = new TrussMove(ent, w);
                uai.AddCommand(tm);
            }
            i++;
        }
        isInspecting = true;
    }

    public void RunDroneRoute(StacsEntity ent, Route route)
    {
        UnitAI uai = ent.GetComponent<UnitAI>();
        uai.StopAndRemoveAllCommands();
        Move m = null;
        int i = 0;
        foreach(Waypoint w in route.Waypoints)
        {
            if(w == route.Waypoints[0])
            {
                if(ent.gameObject.name.StartsWith("Parrot"))
                {
                    Debug.Log("Teleporting Drone");
                }
                uai.Teleport(w);
            }
            else
            {
                m = new Move(ent, w.position);
                uai.AddCommand(m);
            }
            i++;
        }
        isInspecting = true;
    }

    [ContextMenu("MakeDroneRoutes")]
    public void MakeDroneRoutes()
    {
        DroneRoutes.Clear();
        foreach (Transform r in DroneRoutesRoot.GetComponentsInChildren<Transform>())
        {
            if(r.name.StartsWith("Route"))
            {
                Route route = new Route();
                foreach (Transform w in r.GetComponentsInChildren<Transform>())
                {
                    if (w.name.StartsWith("Cube"))
                        route.Waypoints.Add(new Waypoint(w, w.gameObject.name));
                }
                DroneRoutes.Add(route);
            }
        }
    }
}
