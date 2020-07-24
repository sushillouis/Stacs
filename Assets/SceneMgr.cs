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

    public List<Route> Routes = new List<Route>();

    [ContextMenu("MakeRoutes")]
    public void MakeRoutes()
    {
        Routes.Clear();
        Route r = new Route();
        r.Waypoints = new List<GameObject>();

        foreach (Transform t in DroneWaypoints.GetComponentsInChildren<Transform>())
        {
            if (t.name.StartsWith("Cube"))
                r.Waypoints.Add(t.gameObject);
        }
        Routes.Add(r);
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
        int i = 0;
        Vector3 returnPos = entity.position;
        UnitAI uai = entity.GetComponent<UnitAI>();
        Move m = null;
        foreach (GameObject go in Routes[0].Waypoints)
        {
            m = new Move(entity, go.transform.position);
            if(i == 0)
                uai.SetCommand(m);
            else
                uai.AddCommand(m);
            i++;
        }
        m = new Move(entity, returnPos);
        uai.AddCommand(m);
        isInspecting = true;
    }

}
