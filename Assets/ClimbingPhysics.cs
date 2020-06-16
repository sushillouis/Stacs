using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingPhysics : MonoBehaviour
{
    public StacsEntity entity;
    public float magneticForce;
    public float gravityForce;
    public float groundCheckDistance;

    Vector3 groundNormal;
    float force;

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
        //speed
        if (Utils.ApproximatelyEqual(entity.speed, entity.desiredSpeed))
        {
            ;
        }
        else if (entity.speed < entity.desiredSpeed)
        {
            entity.speed = entity.speed + entity.acceleration * Time.deltaTime;
        }
        else if (entity.speed > entity.desiredSpeed)
        {
            entity.speed = entity.speed - entity.acceleration * Time.deltaTime;
        }
        entity.speed = Utils.Clamp(entity.speed, entity.minSpeed, entity.maxSpeed);

        //altitude
        entity.altitude = entity.desiredAltitude = 0;
        //heading
        if (Utils.ApproximatelyEqual(entity.heading, entity.desiredHeading))
        {
            ;
        }
        else if (Utils.AngleDiffPosNeg(entity.desiredHeading, entity.heading) > 0)
        {
            entity.heading += entity.turnRate * Time.deltaTime;
        }
        else if (Utils.AngleDiffPosNeg(entity.desiredHeading, entity.heading) < 0)
        {
            entity.heading -= entity.turnRate * Time.deltaTime;
        }
        entity.heading = Utils.Degrees360(entity.heading);
        //

        transform.rotation = Quaternion.LookRotation(groundNormal, -transform.forward);
        transform.Rotate(Vector3.right, 90f);
        eulerRotation = transform.localEulerAngles;
        eulerRotation.y = entity.heading;
        transform.localEulerAngles = eulerRotation;


        ApplyPhysics();
    }

    public void ApplyPhysics()
    {
        CheckGroundStatus();
        entity.velocity.z = entity.speed;
        entity.velocity.y = transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity).y + ((force / entity.mass) * Time.deltaTime);
        Debug.Log("local velocity: " + transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity));
        GetComponent<Rigidbody>().velocity = transform.TransformDirection(entity.velocity);
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
            groundNormal = Vector3.up;
            force = gravityForce;
        }
    }
}