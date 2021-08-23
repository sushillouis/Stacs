using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SBCameraController : MonoBehaviour
{
    public static SBCameraController instance;
    private Transform rig;
    public bool testbool;
    public float deltaX = 1;
    public float deltaY = 1;
    public float camSpeed = 20.0f;
    public float rotationSpeed = 12.0f;
    public float zoomAmount = 5f;
    private bool rotationLocked = true;
    private float offset = 20;
    private Vector3 orbitStart;
    private Camera camera;
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
    }
    // Start is called before the first frame update
    void Start()
    {
        rig = transform.parent.parent;
    }
    private void OnMouseDown()
    {
        if (testbool)
        {
            deltaX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x - transform.position.x;
            deltaY = Camera.main.ScreenToWorldPoint(Input.mousePosition).y - transform.position.y;
        }
    }

    private void OnMouseDrag()
    {
        if (testbool)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mousePosition.x - deltaX, mousePosition.y - deltaY, mousePosition.z);
        }
    }

    private void ZoomIn()
    {
        Camera.main.transform.position += Camera.main.transform.forward * zoomAmount;
    }

    private void ZoomOut()
    {
        Camera.main.transform.position -= Camera.main.transform.forward * zoomAmount;
    }

    private void StartMouseOrbit()
    {
        /*        yaw = Camera.main.transform.parent.parent.transform.localEulerAngles.y;
                pitch = Camera.main.transform.parent.transform.localEulerAngles.z;*/
        orbitStart = Input.mousePosition;
    }

    private void SetNewAngles(float newPitch, float newYaw)
    {
        transform.parent.localEulerAngles = new Vector3(newPitch, newYaw, 0);
    }

    private void ContinueOrbit()
    {
        Vector2 rotationOffset = Input.mousePosition - orbitStart;
        float rotationYawDegrees = (rotationOffset.x / ((float)Screen.width)) * 180;
        float rotationPitchDegrees = (rotationOffset.y / ((float)Screen.height)) * 180;

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
