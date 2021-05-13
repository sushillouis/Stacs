using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlMgr : MonoBehaviour
{

    public static ControlMgr inst;
    private void Awake()
    {
        inst = this;
    }

    public float deltaSpeed = 5f;
    public float deltaHeading = 5f;
    public float deltaAltitude = 5f ;
    public VRPointer leftHand;
    public VRPointer rightHand;

    public void RunRoutes(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            SceneMgr.inst.RunClimbingRobotRoutes();
            SceneMgr.inst.RunDroneRoutes();
        }
    }

    public void ChangeSpeed(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            if (UIMgr.inst.State == EGameState.Monitoring)
            {
                if (SelectionMgr.inst.selectedEntity != null)
                {
                    SelectionMgr.inst.selectedEntity.desiredSpeed += deltaSpeed * context.ReadValue<float>();// * Time.deltaTime;
                }
            }
            SelectionMgr.inst.selectedEntity.desiredSpeed =
                Utils.Clamp(SelectionMgr.inst.selectedEntity.desiredSpeed, SelectionMgr.inst.selectedEntity.minSpeed, SelectionMgr.inst.selectedEntity.maxSpeed);
        }
    }

    public void ChangeHeading(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (UIMgr.inst.State == EGameState.Monitoring)
            {
                if (SelectionMgr.inst.selectedEntity != null)
                {
                    SelectionMgr.inst.selectedEntity.desiredHeading += deltaHeading * context.ReadValue<float>();// * Time.deltaTime;
                }
            }
            SelectionMgr.inst.selectedEntity.desiredHeading =
                Utils.Degrees360(SelectionMgr.inst.selectedEntity.desiredHeading);
        }
    }

    public void ChangeAltitude(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            if (UIMgr.inst.State == EGameState.Monitoring)
            {
                if (SelectionMgr.inst.selectedEntity != null)
                {
                    SelectionMgr.inst.selectedEntity.desiredAltitude += deltaAltitude * context.ReadValue<float>();
                }
            }
            SelectionMgr.inst.selectedEntity.desiredAltitude =
                Utils.Clamp(SelectionMgr.inst.selectedEntity.desiredAltitude,
                SelectionMgr.inst.selectedEntity.minAltitude,
                SelectionMgr.inst.selectedEntity.maxAltitude);
        }
    }

    public void DrawLasers(InputAction.CallbackContext context)
    {
        Debug.Log("Pressed");
        if(context.started)
        {
            leftHand.SetLaser(true);
            rightHand.SetLaser(true);
        }
        if(context.canceled)
        {
            leftHand.SetLaser(false);
            rightHand.SetLaser(false);
        }
    }
}
