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
        if (UIMgr.inst.State == EGameState.Monitoring && SelectionMgr.inst.selectedEntity != null ) {
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetAxis("EntitySpeed") > 0 )//|| Input.GetKeyUp(KeyCode.Joystick1Button3))
                    SelectionMgr.inst.selectedEntity.desiredSpeed += deltaSpeed * Time.deltaTime;
            if (Input.GetKey(KeyCode.DownArrow) || Input.GetAxis("EntitySpeed") < 0)// || Input.GetKeyUp(KeyCode.Joystick1Button1))
                    SelectionMgr.inst.selectedEntity.desiredSpeed -= deltaSpeed * Time.deltaTime;
            SelectionMgr.inst.selectedEntity.desiredSpeed =
                Utils.Clamp(SelectionMgr.inst.selectedEntity.desiredSpeed, SelectionMgr.inst.selectedEntity.minSpeed, SelectionMgr.inst.selectedEntity.maxSpeed);

            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetAxis("SteerEntity") < 0)
                SelectionMgr.inst.selectedEntity.desiredHeading -= deltaHeading * Time.deltaTime;
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetAxis("SteerEntity") > 0)
                SelectionMgr.inst.selectedEntity.desiredHeading += deltaHeading * Time.deltaTime;
            SelectionMgr.inst.selectedEntity.desiredHeading = 
                Utils.Degrees360(SelectionMgr.inst.selectedEntity.desiredHeading);

            if (Input.GetKey(KeyCode.PageUp) || Input.GetAxis("UpEntity") > 0)
                SelectionMgr.inst.selectedEntity.desiredAltitude += deltaAltitude * Time.deltaTime;
            if (Input.GetKey(KeyCode.PageDown) || Input.GetAxis("DownEntity") > 0)
                SelectionMgr.inst.selectedEntity.desiredAltitude -= deltaAltitude * Time.deltaTime;
            SelectionMgr.inst.selectedEntity.desiredAltitude = 
                Utils.Clamp(SelectionMgr.inst.selectedEntity.desiredAltitude, 
                SelectionMgr.inst.selectedEntity.minAltitude, 
                SelectionMgr.inst.selectedEntity.maxAltitude);

            //---------------Automated inspection by UAV

            if (Input.GetKey(KeyCode.P))
            {
                SceneMgr.inst.RunRoute();
                SceneMgr.inst.RunClimbingRobotRoutes();
            }



        }
    }



}
