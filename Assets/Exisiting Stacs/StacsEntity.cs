using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public enum EntityType
{
    ClimbingRobot,
    ParrotDrone,
    OctocopterGeneric,
    Bird,
    Camera
}


public class StacsEntity : MonoBehaviour
{

    public bool isSelected;
    //----Dynamic Info------------------
    public Vector3 position = Vector3.zero;
    public Vector3 velocity = Vector3.zero;

    public float speed;
    public float desiredSpeed;
    public float heading; //degrees
    public float desiredHeading; //degrees
    public float altitude;
    public float desiredAltitude;
    public float batteryState; // percent

    //----Static Info------------------
    public float acceleration;
    public float turnRate;
    public float climbRate;
    public float maxSpeed;
    public float minSpeed;
    public float minAltitude;
    public float maxAltitude;
 
    public float waypointReachedThreshold;

    /// <summary>
    /// For potential fields scaling
    /// </summary>
    public float mass;
    /// <summary>
    /// For determining stopping distance in unitAi
    /// </summary>
    public float length;

    public EntityType entityType;

    public GameObject cameraRig;
    public GameObject selectionCircle;

    public Camera StacsCamera;
    public RenderTexture CameraRenderTexture;

    public GameObject LookAtBridgeObject;

    public SensorReadings SensorData;


    void Awake() 
    { 
    }

    // Start is called before the first frame update
    void Start()//Initialize StacsEntity
    {
        Transform cameraRigTransform = transform.Find("CameraRig");
        if(cameraRigTransform)
            cameraRig = cameraRigTransform.gameObject;
        else
            cameraRig = transform.Find("LocalYawNode/CameraRig").gameObject;

        StacsCamera = transform.GetComponentInChildren<Camera>();
        if(StacsCamera != null && entityType != EntityType.Camera)
        {
            CameraRenderTexture = new RenderTexture(CameraMgr.inst.RenderTextureToCopyForEntityCams);
            UIMgr.inst.MakeCameraView(CameraRenderTexture);
            StacsCamera.targetTexture = CameraRenderTexture;
        }

        selectionCircle = transform.Find("Decorations").Find("SelectionCylinder").gameObject;
        LookAtBridgeObject = transform.Find("LookAtBridgeObject").gameObject;
        SensorData = GetComponent<SensorReadings>();


    }

    // Update is called once per frame
    void Update()
    {
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log(name + " collided with" + collision.gameObject.name);
    //}

    //private void OnCollisionStay(Collision collision)
    //{
    //    ContactPoint[] contacts = new ContactPoint[10];
    //    int n = collision.GetContacts(contacts);
    //    string tmp = " :";
    //    foreach(ContactPoint c in contacts) {
    //        tmp += c.normal.ToString() + ", ";
    //    }
    //    Debug.Log(n + " points: " + tmp);
    //}
}
