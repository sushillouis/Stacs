using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public GameObject yawNode;
    public GameObject pitchNode;
    public GameObject rollNode;
    public GameObject cameraObject;
    public float speed = 10;
    public float keyboardSpeed = 5;
    public float keyboardSpeedMultiplier = 1.5f;
    public float turnRate = 10;
    public float yawRate = 10;
    public float pitchRate = 10;

    float speedMultiplier = 1.0f;
    Vector3 moveVec = Vector3.zero;
    Vector3 rotVec = Vector3.zero;

    void Update()
    {
        if (SceneMgr.inst.isInspecting && CameraMgr.inst.cameraEntity.entityType == EntityType.ParrotDrone)
            cameraObject.transform.LookAt(SelectionMgr.inst.selectedEntity.LookAtBridgeObject.transform, Vector3.up);
        Move();
        Yaw();
        Pitch();
    }

    public void MoveCamera(InputAction.CallbackContext context)
    {
        moveVec = context.ReadValue<Vector3>();
    }

    public void RotateCamera(InputAction.CallbackContext context)
    {
        rotVec = context.ReadValue<Vector2>();
    }

    public void  SpeedMultiplier(InputAction.CallbackContext context)
    {
        if(context.started)
            speedMultiplier = keyboardSpeedMultiplier;
        else if(context.canceled)
            speedMultiplier = 1;
    }

    void Move()
    {
        Vector3 move = new Vector3(moveVec.x, 0, moveVec.z);
        move = cameraObject.transform.TransformDirection(move);
        move.y = Mathf.Clamp(move.y + moveVec.y, -1, 1);
        transform.position += move * keyboardSpeed * speedMultiplier * Time.deltaTime;
    }

    void Yaw()
    {
        if(rotVec.x != 0)
        {
            Vector3 yaw = yawNode.transform.eulerAngles;
            yaw.y += rotVec.x * yawRate + Time.deltaTime;
            yawNode.transform.eulerAngles = yaw;
        }
    }

    void Pitch()
    {
        if(rotVec.y != 0)
        {
            Vector3 pitch = pitchNode.transform.eulerAngles;
            pitch.x -= rotVec.y * pitchRate + Time.deltaTime;
            pitchNode.transform.eulerAngles = pitch;
        }
    }

}
