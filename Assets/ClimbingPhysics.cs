using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingPhysics : MonoBehaviour
{
    public StacsEntity entity;
    public float magneticForce;
    public float gravityForce;
    public float groundCheckDistance;
    public float wallCheckDistance;
    public float wallCheckWidth;
    public float robotLength;
    public float robotHeight;

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
        }
        ApplyPhysics();
    }

    public void UpdateSpeed() {

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
        CheckWall();
        CheckGroundStatus();

        if(grounded)
        {
            entity.velocity = transform.InverseTransformDirection(localYawNode.transform.forward * entity.speed);
            entityRigidBody.velocity = transform.TransformDirection(entity.velocity);
            entity.position = transform.position;
        }
        else
        {
            entityRigidBody.velocity += Vector3.up * (force / entity.mass) * Time.deltaTime;// = entity.velocity;
            Debug.Log(transform.InverseTransformDirection(Vector3.down));
        }
    }

    public RaycastHit hitInfo;
    public float threshold;

    public Transform cubeT;
    public void CheckGroundStatus()
    {
        //Origin of downward ray to check if still on surface
        Vector3 rayPos = transform.position + (transform.up * 0.1f);
        //Origin of ray to find new surface
        Vector3 rayPos2 = transform.position + (transform.forward * 0.1f);

        //Debug.DrawRay(rayPos, entityRigidBody.velocity, Color.red);
        //Debug.DrawRay(rayPos, -transform.up * groundCheckDistance, Color.green);

        //Raycast down to check if still on surface
        if(Physics.Raycast(rayPos, -transform.up, out hitInfo, groundCheckDistance)) {
            //Update ground normal in case of slight angle change
            oldGroundNormal = groundNormal;
            groundNormal = hitInfo.normal;

            if(groundNormal != Vector3.back)
            {
                Debug.Log(entity.gameObject.name + ": Wrong normal found: " + groundNormal);
            }

            //Debug.DrawRay(rayPos2, groundNormal, Color.yellow);

            //If newly grounded, adjust up direction
            if(!grounded) {
                transform.up = groundNormal;
                grounded = true;
            }

            //Correct the yaw rotation of the entity
            SetRotation();
            //Snap to surface if slightly above
            transform.position = hitInfo.point;
            //Stick if truss, otherwise fall
            force = (hitInfo.collider.gameObject.tag == "Truss" ? magneticForce : gravityForce);
            Debug.Log("Entity: " + entity.gameObject.name + " oldGroundNormal: " + oldGroundNormal + " groundNormal: " + groundNormal);
            
            if(oldGroundNormal != groundNormal) // for down ramps
                RotateToNewSurface();
            
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
        Debug.Log("Entity: " + entity.gameObject.name + " oldGroundNormal: " + oldGroundNormal + " groundNormal: " + groundNormal + " objectName: " + hitInfo.collider.name);
        SetRotation();
        transform.position = hitInfo.point;
        transform.RotateAround(transform.position, Vector3.Cross(oldGroundNormal, groundNormal),
            Vector3.Angle(oldGroundNormal, groundNormal));
    }

    public void CheckWall()
    {
        Vector3 center = transform.position + (transform.up * 0.1f);
        Vector3 left = center + localYawNode.transform.InverseTransformDirection(Vector3.left) * wallCheckWidth;
        Vector3 right = center - localYawNode.transform.InverseTransformDirection(Vector3.left) * wallCheckWidth;

        if (Physics.Raycast(center, localYawNode.transform.forward, out hitInfo, wallCheckDistance))
        {
            SnapToWall();
        }
        else if (Physics.Raycast(left, localYawNode.transform.forward, out hitInfo, wallCheckDistance) ||
            Physics.Raycast(right, localYawNode.transform.forward, out hitInfo, wallCheckDistance))
        {
            if (Physics.Raycast(center, localYawNode.transform.forward, out hitInfo, wallCheckDistance * 4))
            {
                SnapToWall();
            }
        }

        //Debug.DrawLine(center, center + (localYawNode.transform.forward * wallCheckDistance));
        //Debug.DrawLine(left, left + (localYawNode.transform.forward * wallCheckDistance));
        //Debug.DrawLine(right, right + (localYawNode.transform.forward * wallCheckDistance));
        //Debug.DrawLine(center, center - localYawNode.transform.forward * robotLength);
        //Debug.DrawLine(transform.position, transform.position - transform.up * robotHeight);
    }

    public void SnapToWall()
    {
        Debug.Log(entity.gameObject.name + ": Snapped to wall!");
        oldGroundNormal = groundNormal;
        groundNormal = hitInfo.normal;
        transform.RotateAround(transform.position, Vector3.Cross(oldGroundNormal, groundNormal),
            Vector3.Angle(oldGroundNormal, groundNormal));
        Vector3 wallOffsetDirection = Vector3.Cross(groundNormal, localYawNode.transform.TransformDirection(Vector3.right));
        //Debug.DrawLine(hitInfo.point, hitInfo.point - (wallOffsetDirection * (robotLength - 0.1f)), Color.green, 3.0f);
        transform.position = hitInfo.point - (wallOffsetDirection * (robotLength - 0.1f));// + (groundNormal * robotHeight);
    }
}
