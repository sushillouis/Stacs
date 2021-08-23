using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BCCameraController class establishes controls for camera movement within the Bridge Creator scene
/// </summary>
public class BCCameraController : MonoBehaviour
{
    public static BCCameraController instance; //bridge Creator instance is accessible to other scripts
    public enum CameraDirection
    {
        Left, Right, Top, Bottom
    }

    private Transform rig;

    public bool testbool;
    public float deltaX = 1;
    public float deltaY = 1;
    public float camSpeed = 20.0f;
    public float rotationSpeed = 12.0f;

    public float zoomAmount = 5f;

    private bool rotationLocked, leftViewControls, topViewControls, bottomViewControls;

    private float offset = 20;

    private Vector3 orbitStart;

    /// <summary>
    /// instance is established before the first frame is called so other scripts can access the BCCameraController
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
        testbool = true;
    }
    /// <summary>
    /// in the first frame, rig is set to the transform of the parent's parent GameObject
    /// </summary>
    private void Start()
    {
        rig = transform.parent.parent;
        //RightView();
    }
    /// <summary>
    /// establishes position and rotation for snap to top view
    /// </summary>
    public void TopView()
    {
        rig.transform.position = Vector3.zero;
        SetNewAngles(90, 0);
    }
    /// <summary>
    /// establishes position and rotation for snap to bottom view
    /// </summary>
    public void BottomView()
    {
        rig.transform.position = Vector3.zero;
        SetNewAngles(270, 0);
    }
    /// <summary>
    /// establishes position and rotation for snap to left view
    /// </summary>
    public void LeftView()
    {
        rig.transform.position = Vector3.zero;
        SetNewAngles(0, 180);
    }
    /// <summary>
    /// establishes position and rotation for snap to right view
    /// </summary>
    public void RightView()
    {
        rig.transform.position = Vector3.zero;
        SetNewAngles(0, 0);
    }
    /// <summary>
    /// calculates the change in position from the camera to a point selected with the mouse
    /// </summary>
    private void OnMouseDown()
    {
        if (testbool)
        {
            deltaX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x - transform.position.x;
            deltaY = Camera.main.ScreenToWorldPoint(Input.mousePosition).y - transform.position.y;
        }
    }
    /// <summary>
    /// uses delaX and deltaY to change the camera's transform by dragging the mouse
    /// </summary>
    private void OnMouseDrag()
    {
        if (testbool)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mousePosition.x - deltaX, mousePosition.y - deltaY, mousePosition.z);
        }
    }
    /// <summary>
    /// moves the camera closer to the bridge incrementally
    /// </summary>
    private void ZoomIn()
    {
        Camera.main.transform.position += Camera.main.transform.forward * zoomAmount;
    }
    /// <summary>
    /// moves the camera away from the bridge incrementally
    /// </summary>
    private void ZoomOut()
    {
        Camera.main.transform.position -= Camera.main.transform.forward * zoomAmount;
    }
    /// <summary>
    /// using the mouse's postion, the camera is orbitted around the bridge
    /// </summary>
    private void StartMouseOrbit()
    {
/*        yaw = Camera.main.transform.parent.parent.transform.localEulerAngles.y;
        pitch = Camera.main.transform.parent.transform.localEulerAngles.z;*/
        orbitStart = Input.mousePosition;
    }
    /// <summary>
    /// uses parameters to update the angles of a transform
    /// </summary>
    /// <param name="newPitch"></param>
    /// <param name="newYaw"></param>
    private void SetNewAngles(float newPitch, float newYaw)
    {
        transform.parent.localEulerAngles = new Vector3(newPitch, newYaw, 0);
    }
    /// <summary>
    /// the rotational orbit around the bridge is updated each time this function is called 
    /// </summary>
    private void ContinueOrbit()
    {
        Vector2 rotationOffset = Input.mousePosition - orbitStart;
        float rotationYawDegrees = (rotationOffset.x / ((float) Screen.width)) * 180;
        float rotationPitchDegrees = (rotationOffset.y / ((float) Screen.height)) * 180;

        SetNewAngles(transform.parent.localEulerAngles.x - rotationPitchDegrees, transform.parent.localEulerAngles.y + rotationYawDegrees);

        orbitStart = Input.mousePosition;
    }

    // Update is called once per frame
    void Update()
    {
            if (Input.GetMouseButtonDown(1)) // right mouse
            {
                StartMouseOrbit();
            }

            if (Input.GetMouseButton(1))
            {
                ContinueOrbit();
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                testbool = true;
            }

            if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
            {
                ZoomIn();
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
            {
                ZoomOut();
            }

            if (Input.GetKeyDown(KeyCode.Space))
                rotationLocked = false;
            if (Input.GetKeyUp(KeyCode.Space))
                rotationLocked = true;

            if (Input.GetKey(KeyCode.A))
            {
                if (!rotationLocked)
                    SetNewAngles(transform.parent.localEulerAngles.x, transform.parent.localEulerAngles.y + (rotationSpeed * Time.deltaTime));
                else
                {
                    rig.position += -transform.right * camSpeed * Time.deltaTime;
                }
            }
            if (Input.GetKey(KeyCode.D))
            {
                if (!rotationLocked)
                    SetNewAngles(transform.parent.localEulerAngles.x, transform.parent.localEulerAngles.y - (rotationSpeed * Time.deltaTime));

                else
                {
                    rig.position += transform.right * camSpeed * Time.deltaTime;
                }
            }
            if (Input.GetKey(KeyCode.W))
            {
                if (!rotationLocked)
                    SetNewAngles(transform.parent.localEulerAngles.x + (rotationSpeed * Time.deltaTime), transform.parent.localEulerAngles.y);
                else
                {
                    rig.position += transform.up * camSpeed * Time.deltaTime;
                }
            }
            if (Input.GetKey(KeyCode.S))
            {
                if (!rotationLocked)
                    SetNewAngles(transform.parent.localEulerAngles.x - (rotationSpeed * Time.deltaTime), transform.parent.localEulerAngles.y);
                else
                {
                    rig.position += -transform.up * camSpeed * Time.deltaTime;
                }
            }
        }
}
