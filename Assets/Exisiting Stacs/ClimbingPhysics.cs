using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingPhysics : MonoBehaviour
{
    public StacsEntity entity;
    public float magneticForce;
    public float gravityForce;
    public float groundCheckDistance;

    public Vector3 groundNormal;
    public Vector3 oldGroundNormal;
    public float force;
    public bool grounded = true;

    public Rigidbody entityRigidBody;
    public GameObject localYawNode;

    private void Awake()
    {
        entity = GetComponentInParent<StacsEntity>();
        entity.position = transform.localPosition;
        entity.desiredHeading = entity.heading = transform.localRotation.eulerAngles.y;
        entityRigidBody = GetComponent<Rigidbody>();

        localYawNode = transform.Find("LocalYawNode").gameObject;
        usableAcceleration = entity.acceleration;
        usableTurnRate = entity.turnRate;
    }
    // Start is called before the first frame update
    void Start()
    {
        entityRigidBody.velocity = Vector3.zero;
        oldGroundNormal = groundNormal = transform.up;
    }

    public Vector3 eulerRotation = Vector3.zero;
    public float usableTurnRate;
    public float usableAcceleration;
    public float fineTuneFactor = 4.0f;
    // Update is called once per frame
    void Update()
    {
        //if(Utils.ApproximatelyEqualAngle(entity.heading, entity.desiredHeading, 2 * Utils.ANGLE_EPSILON))
        //{
        //    usableTurnRate = entity.turnRate / fineTuneFactor;
        //} else
        //{
        //    usableTurnRate = entity.turnRate;
        //}
        //Indicates forward and up direction for debugging
        if(entity.batteryState <= 0) {
            entity.speed = 0; entityRigidBody.velocity = Vector3.zero;
        } else {
            //heading
            if(Utils.ApproximatelyEqualAngle(entity.heading, entity.desiredHeading))   {
                entity.heading = entity.desiredHeading;
                UpdateSpeed();
            } else if(Utils.AngleDiffPosNeg(entity.desiredHeading, entity.heading) > 0) {
                entity.heading += usableTurnRate * Time.deltaTime;
            } else if(Utils.AngleDiffPosNeg(entity.desiredHeading, entity.heading) < 0) {
                entity.heading -= usableTurnRate * Time.deltaTime;
            }
            entity.heading = Utils.Degrees360(entity.heading);
            //altitude
            entity.altitude = entity.desiredAltitude = 0;
            SetRotation();
            ApplyPhysics();
        }
    }

    public void UpdateSpeed() {

        //if(Utils.ApproximatelyEqual(entity.speed, entity.desiredSpeed, 2 * Utils.EPSILON))
        //{
        //    usableAcceleration = entity.acceleration / fineTuneFactor;
        //} else
        //{
        //    usableAcceleration = entity.acceleration;
        //}

        if (Utils.ApproximatelyEqual(entity.speed, entity.desiredSpeed)) {
            entity.speed = entity.desiredSpeed;
        }
        else if (entity.speed < entity.desiredSpeed)
        {
            entity.speed = entity.speed + usableAcceleration * Time.deltaTime;
        }
        else if (entity.speed > entity.desiredSpeed)
        {
            entity.speed = entity.speed - usableAcceleration * Time.deltaTime;
        }
        entity.speed = Utils.Clamp(entity.speed, entity.minSpeed, entity.maxSpeed);
    }

    public void SetRotation()
    {
        eulerRotation = localYawNode.transform.localEulerAngles;
        eulerRotation.y = entity.heading;
        localYawNode.transform.localEulerAngles = eulerRotation;
    }

    public void ApplyPhysics()
    {
        CheckGroundStatus();

        entity.velocity = transform.InverseTransformDirection(localYawNode.transform.forward * entity.speed);
        entity.velocity.y = transform.InverseTransformDirection(entityRigidBody.velocity).y 
            + ((force/entity.mass) * Time.deltaTime);
        entityRigidBody.velocity = transform.TransformDirection(entity.velocity);
    }

    public RaycastHit hitInfo;
    public float threshold;

    public Transform cubeT;
    public void CheckGroundStatus()
    {
        Vector3 rayPos = transform.position + (transform.up * 0.1f);
        Vector3 rayPos2 = transform.position + (transform.forward * 0.1f);
        Debug.DrawRay(rayPos, entityRigidBody.velocity, Color.red);

        if(Physics.Raycast(rayPos, -transform.up, out hitInfo, groundCheckDistance)) {
            oldGroundNormal = groundNormal;
            groundNormal = hitInfo.normal;
            Debug.DrawRay(rayPos2, groundNormal, Color.yellow);
            if(!grounded) {
                transform.up = groundNormal;
                grounded = true;
            }
            SetRotation();
            transform.position = hitInfo.point;
            force = (hitInfo.collider.gameObject.tag == "Truss" ? magneticForce : gravityForce);

            //if(oldGroundNormal != groundNormal) // for down ramps
            //    RotateToNewSurface();
            //Next if raycast forward shows ramp, rotate to go up ramp
        } else { //cliff edge
            if(Physics.Raycast(transform.position, -localYawNode.transform.up - localYawNode.transform.forward, out hitInfo, groundCheckDistance)) {
                oldGroundNormal = groundNormal;
                groundNormal = hitInfo.normal;
                Debug.Log("Went over a cliff");
                SetRotation();
                transform.position = hitInfo.point;
                transform.RotateAround(transform.position, Vector3.Cross(oldGroundNormal, groundNormal),
                    Vector3.Angle(oldGroundNormal, groundNormal));
                Debug.Log("Rotation: " + transform.localEulerAngles);
            } else  {
                groundNormal = Vector3.up;
                force = gravityForce;
                grounded = false;
            }
        }
    }

    void RotateToNewSurface()
    {
        SetRotation();
        transform.position = hitInfo.point;
        transform.RotateAround(transform.position, Vector3.Cross(oldGroundNormal, groundNormal),
            Vector3.Angle(oldGroundNormal, groundNormal));
    }


}
