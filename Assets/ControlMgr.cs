using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlMgr : MonoBehaviour
{

    public static ControlMgr inst;
    private void Awake()
    {
        inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    public float deltaSpeed = 5f;
    public float deltaHeading = 5f;
    public float deltaAltitude = 5f ;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetAxis("EntitySpeed") > 0)//|| Input.GetKeyUp(KeyCode.Joystick1Button3))
            IncreaseSpeed();
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetAxis("EntitySpeed") < 0)// || Input.GetKeyUp(KeyCode.Joystick1Button1))
            DecreaseSpeed();

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetAxis("SteerEntity") < 0)
            DecreaseHeading();
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetAxis("SteerEntity") > 0)
            IncreaseHeading();

        if (Input.GetKey(KeyCode.PageUp) || Input.GetAxis("UpEntity") > 0)
            IncreaseAltitude();
        if (Input.GetKey(KeyCode.PageDown) || Input.GetAxis("DownEntity") > 0)
            DecreaseAltitude();

        //---------------Automated inspection by UAV

        if (Input.GetKeyDown(KeyCode.P))
        {
            RunRoutes();
        }
    }

    void IncreaseSpeed()
    {
        if (UIMgr.inst.State == EGameState.Monitoring)
        {
            if (SelectionMgr.inst.selectedEntity != null)
            {
                SelectionMgr.inst.selectedEntity.desiredSpeed += deltaSpeed * Time.deltaTime;
            }
        }
        SelectionMgr.inst.selectedEntity.desiredSpeed =
            Utils.Clamp(SelectionMgr.inst.selectedEntity.desiredSpeed, SelectionMgr.inst.selectedEntity.minSpeed, SelectionMgr.inst.selectedEntity.maxSpeed);
    }

    void DecreaseSpeed()
    {
        if (UIMgr.inst.State == EGameState.Monitoring)
        {
            if (SelectionMgr.inst.selectedEntity != null)
            {
                SelectionMgr.inst.selectedEntity.desiredSpeed -= deltaSpeed * Time.deltaTime;
            }
        }
        SelectionMgr.inst.selectedEntity.desiredSpeed =
            Utils.Clamp(SelectionMgr.inst.selectedEntity.desiredSpeed, SelectionMgr.inst.selectedEntity.minSpeed, SelectionMgr.inst.selectedEntity.maxSpeed);
    }

    void IncreaseHeading()
    {
        if (UIMgr.inst.State == EGameState.Monitoring)
        {
            if (SelectionMgr.inst.selectedEntity != null)
            {
                SelectionMgr.inst.selectedEntity.desiredHeading += deltaHeading * Time.deltaTime;
            }
        }
        SelectionMgr.inst.selectedEntity.desiredHeading =
            Utils.Degrees360(SelectionMgr.inst.selectedEntity.desiredHeading);
    }

    void DecreaseHeading()
    {
        if (UIMgr.inst.State == EGameState.Monitoring)
        {
            if (SelectionMgr.inst.selectedEntity != null)
            {
                SelectionMgr.inst.selectedEntity.desiredHeading -= deltaHeading * Time.deltaTime;
            }
        }
        SelectionMgr.inst.selectedEntity.desiredHeading =
            Utils.Degrees360(SelectionMgr.inst.selectedEntity.desiredHeading);
    }

    void IncreaseAltitude()
    {
        if (UIMgr.inst.State == EGameState.Monitoring)
        {
            if (SelectionMgr.inst.selectedEntity != null)
            {
                SelectionMgr.inst.selectedEntity.desiredAltitude += deltaAltitude * Time.deltaTime;
            }
        }
        SelectionMgr.inst.selectedEntity.desiredAltitude =
            Utils.Clamp(SelectionMgr.inst.selectedEntity.desiredAltitude,
            SelectionMgr.inst.selectedEntity.minAltitude,
            SelectionMgr.inst.selectedEntity.maxAltitude);
    }

    void DecreaseAltitude()
    {
        if (UIMgr.inst.State == EGameState.Monitoring)
        {
            if (SelectionMgr.inst.selectedEntity != null)
            {
                SelectionMgr.inst.selectedEntity.desiredAltitude -= deltaAltitude * Time.deltaTime;
            }
        }
        SelectionMgr.inst.selectedEntity.desiredAltitude =
            Utils.Clamp(SelectionMgr.inst.selectedEntity.desiredAltitude,
            SelectionMgr.inst.selectedEntity.minAltitude,
            SelectionMgr.inst.selectedEntity.maxAltitude);
    }

    void RunRoutes()
    {
        SceneMgr.inst.RunClimbingRobotRoutes();
        SceneMgr.inst.RunDroneRoutes();
    }

}
