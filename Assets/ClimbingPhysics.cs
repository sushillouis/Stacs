using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingPhysics : MonoBehaviour
{
    public StacsEntity entity;
    public Transform body;
    public float magneticForce;
    public float gravityForce;
    public float groundCheckDistance;

    Vector3 groundNormal;
    float force;
    bool grounded;

    private void Awake()
    {
        entity = GetComponentInParent<StacsEntity>();
        entity.position = transform.localPosition;
        
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    public Vector3 eulerRotation = Vector3.zero;

    // Update is called once per frame
    void Update()
    {
        //Indicates forward and up direction for debugging
        Debug.DrawRay(transform.position + (0.5f * transform.up), body.transform.forward, Color.red);
        Debug.DrawRay(transform.position + (0.5f * transform.up), transform.up, Color.green);

        //speed
        if (Utils.ApproximatelyEqual(entity.speed, entity.desiredSpeed)) {
            entity.speed = entity.desiredSpeed;
        } else if (entity.speed < entity.desiredSpeed) {
            entity.speed = entity.speed + entity.acceleration * Time.deltaTime;
        } else if (entity.speed > entity.desiredSpeed) {
            entity.speed = entity.speed - entity.acceleration * Time.deltaTime;
        }
        entity.speed = Utils.Clamp(entity.speed, entity.minSpeed, entity.maxSpeed);

        //altitude
        entity.altitude = entity.desiredAltitude = 0;
        //heading
        if (Utils.ApproximatelyEqualAngle(entity.heading, entity.desiredHeading)) {
            entity.heading = entity.desiredHeading;
        } else if (Utils.AngleDiffPosNeg(entity.desiredHeading, entity.heading) > 0) {
            entity.heading += entity.turnRate * Time.deltaTime;
        } else if (Utils.AngleDiffPosNeg(entity.desiredHeading, entity.heading) < 0) {
            entity.heading -= entity.turnRate * Time.deltaTime;
        }
        entity.heading = Utils.Degrees360(entity.heading);
        //

        SetRotation();
        ApplyPhysics();
    }

    private void ApplyPhysics()
    {
        CheckGroundStatus();
        entity.velocity = transform.InverseTransformDirection(body.forward * entity.speed);
        entity.velocity.y = transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity).y + ((force / entity.mass) * Time.deltaTime);
        GetComponent<Rigidbody>().velocity = transform.TransformDirection(entity.velocity);
    }

    private void SetRotation()
    {
        //transform.up = groundNormal;
        eulerRotation = body.localEulerAngles;
        eulerRotation.y = entity.heading;
        body.localEulerAngles = eulerRotation;
    }

    void CheckGroundStatus()
    {
        RaycastHit hitInfo;
#if UNITY_EDITOR
        // helper to visualise the ground check ray in the scene view
        Debug.DrawLine(transform.position + (transform.up * 0.1f), transform.position + (transform.up * 0.1f) + (-transform.up * groundCheckDistance));
#endif
        // 0.1f is a small offset to start the ray from inside the character
        // it is also good to note that the transform position in the sample assets is at the base of the character
        if (Physics.Raycast(transform.position + (transform.up * 0.1f), -transform.up, out hitInfo, groundCheckDistance))
        {
            groundNormal = hitInfo.normal;
            //If the robot falls and strikes an angled surface, this block ensures it sticks at the correct orientation
            if (!grounded)
            {
                transform.up = groundNormal;
                grounded = true;
            }
            SetRotation();
            transform.position = hitInfo.point;
            if (hitInfo.collider.gameObject.tag == "Truss")
            {
                force = magneticForce;
            }
            else
            {
                force = gravityForce;
            }
        }
        else
        {
            if (Physics.Raycast(transform.position, -body.up - body.forward, out hitInfo, groundCheckDistance))
            {
                Vector3 old_groundNormal = groundNormal;
                groundNormal = hitInfo.normal;
                SetRotation();
                transform.position = hitInfo.point;
                transform.RotateAround(transform.position, Vector3.Cross(old_groundNormal, groundNormal), Vector3.Angle(old_groundNormal, groundNormal));
            }
            else
            {
                groundNormal = Vector3.up;
                force = gravityForce;
                grounded = false;
            }
        }
    }
}