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

    float speedMultiplier;

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
            speedMultiplier = (Input.GetKey(KeyCode.LeftShift)) ? keyboardSpeedMultiplier : 1;

            //pos += cameraObject.transform.right * Input.GetAxis("DPadHorizontal") * speed * Time.deltaTime;
            //pos -= cameraObject.transform.forward * Input.GetAxis("DPadVertical") * speed * Time.deltaTime;
            //Keyboard arrow keys
            if (Input.GetKey(KeyCode.W))
                Move(cameraObject.transform.forward);
            if (Input.GetKey(KeyCode.S))
                Move(-cameraObject.transform.forward);
            if (Input.GetKey(KeyCode.D))
                Move(cameraObject.transform.right);
            if (Input.GetKey(KeyCode.A))
                Move(-cameraObject.transform.right);
            /*
            //GameController, hold rightTrigger and use second joystick
            if (Input.GetAxis("CameraUp") > 0.5f) {
                pos.y += Input.GetAxis("Up") * speed * Time.deltaTime;
                pos.y -= Input.GetAxis("Down") * speed * Time.deltaTime;
            }
            */
            //Keyboard
            if (Input.GetKey(KeyCode.R))
                Move(Vector3.up);// pos.y += keyboardSpeed * speedMultiplier * Time.deltaTime;
            if (Input.GetKey(KeyCode.F))
                Move(-Vector3.up);// pos.y -= keyboardSpeed * speedMultiplier * Time.deltaTime;

            //transform.position = pos;
            //-------Rotation Game Controller YAW-------------------------------------
            //Vector3 yaw = yawNode.transform.eulerAngles;
            //yaw.y += Input.GetAxis("LookX") * turnRate * Time.deltaTime;

            //These lines make it impossible to move camera laterally, seems better without
            //if (Input.GetKey(KeyCode.LeftShift))
            //yaw.y += Input.GetAxis("Horizontal") * turnRate * Time.deltaTime;

            //---Keyboard

            if (Input.GetKey(KeyCode.Q))
                Yaw(-1);
            if (Input.GetKey(KeyCode.E))
                Yaw(1);

            //---------------Rotation pitch--------------------------------
            //Vector3 pitch = pitchNode.transform.eulerAngles;
            //pitch.x += Input.GetAxis("LookY") * turnRate * Time.deltaTime;
            //if (Input.GetKey(KeyCode.LeftShift))
            //pitch.x += Input.GetAxis("Vertical") * turnRate * Time.deltaTime;
            //Keyboard
            if (Input.GetKey(KeyCode.Z))
                Pitch(1);
            if (Input.GetKey(KeyCode.X))
                Pitch(-1);

        }
    }

    void Move(Vector3 moveVec)
    {
        transform.position += moveVec * keyboardSpeed * speedMultiplier * Time.deltaTime;
    }

    void Yaw(float direction)
    {
        Vector3 yaw = yawNode.transform.eulerAngles;
        yaw.y += direction * yawRate + Time.deltaTime;
        yawNode.transform.eulerAngles = yaw;
    }

    void Pitch(float direction)
    {
        Vector3 pitch = pitchNode.transform.eulerAngles;
        pitch.x += direction * pitchRate + Time.deltaTime;
        pitchNode.transform.eulerAngles = pitch;
    }

}
