using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMgr : MonoBehaviour
{
    public static EntityMgr inst;
    private void Awake()
    {
        inst = this;
    }

    public List<StacsEntity> entities;
    private StacsEntity selected;
    private int selectedIndex = 0;

    bool selectPressed = false;

    private void Start()
    {
        selected = entities[selectedIndex];
    }

    public void PlaceEntities()
    {
        foreach(StacsEntity e in entities)
        {
            e.gameObject.SetActive(false);
        }

        StartingPoint camPoint = GameObject.Find("EnvironmentMgr").GetComponent<EnvironmentMgr>().current.camPoint;
        List<StartingPoint> climbingPositions = GameObject.Find("EnvironmentMgr").GetComponent<EnvironmentMgr>().current.climbingPositions;
        List<StartingPoint> dronePositions = GameObject.Find("EnvironmentMgr").GetComponent<EnvironmentMgr>().current.dronePositions;

        int climbingIndex = 0;
        int droneIndex = 0;

        foreach(StacsEntity e in entities)
        {
            e.gameObject.SetActive(true);
            switch (e.entityType)
            {
                case EntityType.Camera:
                    if (camPoint == null)
                    {
                        Debug.LogError("Camera Position Not Set in EnvironmentMgr!");
                        break;
                    }
                    e.transform.position = camPoint.position;
                    e.transform.transform.eulerAngles = camPoint.eulerAngles;
                    break;
                case EntityType.ClimbingRobot:
                    if (climbingIndex >= climbingPositions.Count)
                    {
                        Debug.LogError("Not Enough Climbing Positions Provided in EnvironmentMgr!");
                        break;
                    }
                    e.transform.position = climbingPositions[climbingIndex].position;
                    e.transform.transform.eulerAngles = climbingPositions[climbingIndex++].eulerAngles;
                    break;
                case EntityType.ParrotDrone:
                    if (droneIndex >= dronePositions.Count)
                    {
                        Debug.LogError("Not Enough Drone Positions Provided in EnvironmentMgr!");
                        break;
                    }
                    e.transform.position = e.position = dronePositions[droneIndex].position;
                    e.transform.transform.eulerAngles = dronePositions[droneIndex++].eulerAngles;
                    break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {/*
        if (selectPressed == false && Input.GetAxis("Select") != 0)
        {
            selected.isSelected = false;
            if(Input.GetAxis("Select") > 0)
            {
                selectedIndex = (selectedIndex < entities.Count - 1) ? selectedIndex + 1 : 0;
            }
            else
            {
                selectedIndex = (selectedIndex > 0) ? selectedIndex - 1 : entities.Count - 1;

            }
            selected = entities[selectedIndex];
            selected.isSelected = true; 
            //set camera
            selectPressed = true;
            Debug.Log("Pressed");
        }
        else if (Input.GetAxis("Select") == 0) 
        {
            selectPressed = false;
        }
        */
    }
}
