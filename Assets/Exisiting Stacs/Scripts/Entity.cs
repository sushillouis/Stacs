using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public float movementSpeed;
    public float rotationSpeed;
    public Transform cameraArm;
    [HideInInspector]
    public bool selected = false;

    private void Update()
    {
        if(selected)
        {
            Vector3 vel = Vector3.zero;
            if (!Input.GetKey(KeyCode.LeftShift))
            {
                vel += transform.right * Input.GetAxis("Horizontal");
                vel += transform.forward * Input.GetAxis("Vertical");
            }
            vel.y += Input.GetAxis("Up") * movementSpeed * Time.deltaTime;
            vel.y -= Input.GetAxis("Down") * movementSpeed * Time.deltaTime;
            transform.position += vel * movementSpeed * Time.deltaTime;

            Vector3 rot = transform.eulerAngles;
            rot.y += Input.GetAxis("LookX") * rotationSpeed * Time.deltaTime;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                rot.y += Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;
            }
            transform.eulerAngles = rot;
        }
    }

    public void Select()
    {
        selected = true;
        Camera.main.transform.SetParent(cameraArm, false);
    }
}
