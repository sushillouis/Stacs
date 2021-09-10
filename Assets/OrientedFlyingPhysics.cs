using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientedFlyingPhysics : MonoBehaviour
{
    private void Awake()
    {
        entity = GetComponentInParent<StacsEntity>();
        entity.position = transform.localPosition;
        entity.altitude = entity.desiredAltitude = transform.localPosition.y;

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public StacsEntity entity;
    public Vector3 eulerRotation = Vector3.zero;
    public Vector3 tmp;

    public float lerpTime = 0.01f;
    // Update is called once per frame
    void Update()
    {
        //speed
        if (Utils.ApproximatelyEqual(entity.speed, entity.desiredSpeed)) {
            ;
        } else if (entity.speed < entity.desiredSpeed) {
            entity.speed = entity.speed + entity.acceleration * Time.deltaTime;
        } else if (entity.speed > entity.desiredSpeed) {
            entity.speed = entity.speed - entity.acceleration * Time.deltaTime;
        }
        entity.speed = Utils.Clamp(entity.speed, entity.minSpeed, entity.maxSpeed);

        //altitude
        if(Utils.ApproximatelyEqual(entity.altitude, entity.desiredAltitude)) {
            ;
        } else if (entity.altitude < entity.desiredAltitude) {
            entity.altitude += entity.climbRate * Time.deltaTime;
        } else if (entity.altitude > entity.desiredAltitude) {
            entity.altitude -= entity.climbRate * Time.deltaTime;
        }
        entity.altitude = Utils.Clamp(entity.altitude, entity.minAltitude, entity.maxAltitude);

        //heading
        if (Utils.ApproximatelyEqualAngle(entity.heading, entity.desiredHeading)) {
            ;
        } else if (Utils.AngleDiffPosNeg(entity.desiredHeading, entity.heading) > Utils.ANGLE_EPSILON) {
            entity.heading += entity.turnRate * Time.deltaTime;
        } else if (Utils.AngleDiffPosNeg(entity.desiredHeading, entity.heading) < Utils.ANGLE_EPSILON) {
            entity.heading -= entity.turnRate * Time.deltaTime;
        }
        entity.heading = Utils.Degrees360(entity.heading);

        //
        entity.velocity.x = Mathf.Sin(entity.heading * Mathf.Deg2Rad) * entity.speed;
        entity.velocity.y = 0;
        entity.velocity.z = Mathf.Cos(entity.heading * Mathf.Deg2Rad) * entity.speed;

        entity.position = entity.position + entity.velocity * Time.deltaTime;
        entity.position.y = entity.altitude;
        //transform.localPosition = Vector3.Lerp(transform.position, entity.position, lerpTime);
        transform.position = entity.position;

        eulerRotation.x = eulerRotation.z = 0;
        eulerRotation.y = entity.heading;

        //transform.localEulerAngles = eulerRotation;

        tmp = transform.localEulerAngles;
        tmp.x = tmp.z = 0;
        transform.localEulerAngles = tmp;
        Quaternion rot = Quaternion.Euler(eulerRotation);
        //transform.localEulerAngles = Vector3.Lerp(transform.localEulerAngles, eulerRotation, lerpTime);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, rot, lerpTime);
        //transform.localEulerAngles = eulerRotation;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody rb = entity.GetComponent<Rigidbody>();
        //rb.useGravity = true;
        entity.speed = entity.desiredSpeed = 0;
        entity.altitude = entity.desiredAltitude = collision.GetContact(0).point.y;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.constraints = RigidbodyConstraints.FreezePositionX;
        rb.constraints = RigidbodyConstraints.FreezePositionZ;

    }

}
