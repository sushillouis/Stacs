using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// The SandboxMgr handles all functionality specific to sandbox mode
/// </summary>
public class SandboxMgr : MonoBehaviour
{
    public GameObject climbingRobot;
    public GameObject ParrotDrone;
    private GameObject robotFolder;
    public List<GameObject> robots;
    public GameObject player;
    public float spawnOffset = 1;
    public static SandboxMgr instance;
    public GameObject bridgeObject;
    /// <summary>
    /// Awake called once upon application initialization
    /// </summary>
    private void Awake()
    {

        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(this);
        }
        robotFolder = new GameObject();
        robotFolder.name = "Robots";
        robots = new List<GameObject>();
    }
    /// <summary>
    /// adds a climbing robot to the sandbox scene
    /// </summary>
    public void SpawnClimbing()
    {
        robots.Add(Instantiate(climbingRobot, GetNextSpawnLocation(), Quaternion.identity, robotFolder.transform));
    }
    /// <summary>
    /// adds a flying drone to the sandbox scene
    /// </summary>
    public void SpawnDrone()
    {
        robots.Add(Instantiate(ParrotDrone, GetNextSpawnLocation(), Quaternion.identity, robotFolder.transform));
    }
    /// <summary>
    /// removes either kind of robot from the sandbox scene given a robot is selected
    /// </summary>
    public void DespawnRobot()
    {
        StacsEntity robot = SelectionMgr.inst.selectedEntity;
        //robots.Remove(robot);
        Destroy(robot.gameObject); 
    }
    /// <summary>
    /// uses the user's location to spawn a new robot
    /// </summary>
    /// <returns> returns a vector3 of the user's position in the scene </returns>
    private Vector3 GetNextSpawnLocation()
    {
        return player.transform.position + player.transform.forward * spawnOffset;  
    }
    
}
