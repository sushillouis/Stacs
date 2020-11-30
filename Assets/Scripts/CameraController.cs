using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject yawNode;
    public GameObject pitchNode;
    public GameObject rollNode;
    public GameObject cameraObject;
    bool isSelected = true; //always
    public float speed = 10;
    public float keyboardSpeed = 5;
    public float keyboardSpeedMultiplier = 1.5f;
    public float turnRate = 10;
    public float yawRate = 10;
    public float pitchRate = 10;



    void Update()
    {
        UpdateCamera();
        if (SceneMgr.inst.isInspecting && CameraMgr.inst.cameraEntity.entityType == EntityType.ParrotDrone)
            cameraObject.transform.LookAt(SelectionMgr.inst.selectedEntity.LookAtBridgeObject.transform, Vector3.up);
    }



    void UpdateCamera() { 

        if(isSelected)
        {
            Vector3 pos = transform.position;

            //Check for left shift, if so increase movement speed
            float speedMultiplier = (Input.GetKey(KeyCode.LeftShift)) ? keyboardSpeedMultiplier : 1;

            pos += cameraObject.transform.right * Input.GetAxis("DPadHorizontal") * speed * Time.deltaTime;
            pos -= cameraObject.transform.forward * Input.GetAxis("DPadVertical") * speed * Time.deltaTime;
            //Keyboard arrow keys
            if (Input.GetKey(KeyCode.W))
                pos += cameraObject.transform.forward * keyboardSpeed * speedMultiplier * Time.deltaTime;
            if (Input.GetKey(KeyCode.S))
                pos -= cameraObject.transform.forward * keyboardSpeed * speedMultiplier * Time.deltaTime;
            if (Input.GetKey(KeyCode.D))
                pos += cameraObject.transform.right * keyboardSpeed * speedMultiplier * Time.deltaTime;
            if (Input.GetKey(KeyCode.A))
                pos -= cameraObject.transform.right * keyboardSpeed * speedMultiplier * Time.deltaTime;

            //GameController, hold rightTrigger and use second joystick
            if (Input.GetAxis("CameraUp") > 0.5f) {
                pos.y += Input.GetAxis("Up") * speed * Time.deltaTime;
                pos.y -= Input.GetAxis("Down") * speed * Time.deltaTime;
            }
            //Keyboard
            if (Input.GetKey(KeyCode.R))
                pos.y += keyboardSpeed * speedMultiplier * Time.deltaTime;
            if (Input.GetKey(KeyCode.F))
                pos.y -= keyboardSpeed * speedMultiplier * Time.deltaTime;

            transform.position = pos;
            //-------Rotation Game Controller YAW-------------------------------------
            Vector3 yaw = yawNode.transform.eulerAngles;
            yaw.y += Input.GetAxis("LookX") * turnRate * Time.deltaTime;

            //These lines make it impossible to move camera laterally, seems better without
            //if (Input.GetKey(KeyCode.LeftShift))
                //yaw.y += Input.GetAxis("Horizontal") * turnRate * Time.deltaTime;

            //---Keyboard

            if (Input.GetKey(KeyCode.Q))
                yaw.y -= yawRate + Time.deltaTime;
            if (Input.GetKey(KeyCode.E))
                yaw.y += yawRate + Time.deltaTime;


            yawNode.transform.eulerAngles = yaw;

            //---------------Rotation pitch--------------------------------
            Vector3 pitch = pitchNode.transform.eulerAngles;
            pitch.x += Input.GetAxis("LookY") * turnRate * Time.deltaTime;
            //if (Input.GetKey(KeyCode.LeftShift))
                //pitch.x += Input.GetAxis("Vertical") * turnRate * Time.deltaTime;
            //Keyboard
            if (Input.GetKey(KeyCode.Z))
                pitch.x += pitchRate * Time.deltaTime;
            if (Input.GetKey(KeyCode.X))
                pitch.x -= pitchRate * Time.deltaTime;

            pitchNode.transform.eulerAngles = pitch;
        }
    }


}
