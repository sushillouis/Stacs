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

    //Normal of current plane
    Vector3 groundNormal;
    //Either gravitational or magnetic, currently only gravitational when falling off bridge
    float force;

    private void Awake()
    {
        entity = GetComponentInParent<StacsEntity>();
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

        SetRotation();
        ApplyPhysics();
    }

    //Updates rigidbody velocity
    //There is a problem with the gameobject not maintaining it's local y rotation when assigning transform.up. When trying to find a solution I tried applying the yaw rotation to the robot body but applying velocity to the parent gameObject. (More explanation at line 113)
    //This helped the problem I was facing at the time but now it may not be the best solution.
    private void ApplyPhysics()
    {
        CheckGroundStatus();
        //Forward velocity is forward direction of robot body in local space
        entity.velocity = transform.InverseTransformDirection(body.forward * entity.speed);
        //Downward velocity is current rigidbody.velocity.y + acceleration due to force
        entity.velocity.y = transform.InverseTransformDirection(GetComponent<Rigidbody>().velocity).y + ((force / entity.mass) * Time.deltaTime);
        //Converts to global space to set rigidbody velocity
        GetComponent<Rigidbody>().velocity = transform.TransformDirection(entity.velocity);
    }

    private void SetRotation()
    {
        //Does this still need to be assigned every frame? It used to be necessary but now it may only be needed when the groundNormal is changed.
        transform.up = groundNormal;
        //Assigns body rotation to entity.heading
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
            //If the raycast downward from the center hits nothing it means that the robot has gone over an edge.
            //So cast raypoint back at 45 degrees from center to find new ground plane.
            if (Physics.Raycast(transform.position, -body.up - body.forward, out hitInfo, groundCheckDistance))
            {
                Vector3 old_groundNormal = groundNormal;
                groundNormal = hitInfo.normal;
                SetRotation();
                transform.position = hitInfo.point;
                Debug.Log("old_groundNormal: " + old_groundNormal + " groundNormal: " + groundNormal);
                //Problem: 
                //local y rotation is not maintained when assigning transfrom.up to new groundNormal
                //Temporary solution (current solution):
                //Compare old_groundNormal to the new groundNormal, and add either 90 or 180 degrees to heading to correct the unwanted rotation
                //Permanent solution (needs to be written):
                //Find a way to rotate the robot from the old ground normal to the new ground normal without any unwanted rotation on the y axis.
                //This would simplify the code a lot and until it's written the code doesn't work if the truss us rotated to any other angle.
                if (old_groundNormal == Vector3.right && groundNormal == Vector3.down ||
                    old_groundNormal == Vector3.down && groundNormal == Vector3.right ||
                    old_groundNormal == Vector3.left && groundNormal == Vector3.down)
                {
                    //flipped = true;
                    entity.desiredHeading += 180.0f;
                    entity.heading += 180.0f;
                    SetRotation();
                    transform.position += body.forward * 0.25f;
                }
                else if (old_groundNormal == Vector3.forward && groundNormal == Vector3.right ||
                    old_groundNormal == Vector3.right && groundNormal == Vector3.back ||
                    old_groundNormal == Vector3.back && groundNormal == Vector3.left ||
                    old_groundNormal == Vector3.left && groundNormal == Vector3.forward)
                {
                    entity.desiredHeading += 90.0f;
                    entity.heading += 90.0f;
                    SetRotation();
                    transform.position += body.forward * 0.25f;
                }
                else
                {
                    SetRotation();
                }
            }
            //If neither raycast hits then gravitational force is applied and the robot falls
            else
            {
                groundNormal = Vector3.up;
                force = gravityForce;
            }
        }
    }
}