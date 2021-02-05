using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TrussMove : Command
{
    //target position
    public Waypoint destination;
    //public LineRenderer potentialLine;

    //distance needed for robot to come to a complete stop
    public float doneDistanceSq;
    public Vector3 edgePoint;

    //CONSTRUCTOR
    public TrussMove(StacsEntity ent, Waypoint w) : base(ent)
    {
        //Get position in the plane of movement of the robot. So vertex placement doesn't have to be exact.
        //This will need updating when vertices may be on different planes
        destination = w;
        //Create line but leave deactive until command begins
        edgePoint = Utils.GetEdgePoint(entity.transform.position, destination.position, entity.transform.up, destination.transform.up);
        if(edgePoint == Vector3.zero)
            line = LineMgr.inst.CreateTrussMoveLine(entity.position, destination.position);
        else
            line = LineMgr.inst.CreateTrussMoveLine(entity.position, edgePoint, destination.position);
        line.gameObject.SetActive(false);
    }

    //true if robot is slowing down to stop
    public bool stopping = false;
    //true if robot has aligned heading and began moving
    public bool going = false;
    public override void Tick()
    {
        edgePoint = Utils.GetEdgePoint(entity.transform.position, destination.position, entity.transform.up, destination.transform.up);
        line.positionCount = (edgePoint == Vector3.zero) ? 2 : 3;
        //Get desired speed and heading at current time
        DHDS dhds;
        dhds = ComputeDHDS();
        //Get distance needed to come to a complete stop at current speed
        doneDistanceSq = ComputeDoneDistanceSq();

        //Check if time needed to stop is greater than remaining distance, if so begin stopping.
        //This method means the robot will overshoot, but it is only noticable when acceleration is very high.
        if(ComputeRealDistanceSq() <= doneDistanceSq)
        {
            stopping = true;
        }
        //Check if heading has been aligned, if so begin moving
        else if(Utils.ApproximatelyEqualAngle(dhds.dh, entity.heading))
        {
            going = true;
        }

        //If stopping easy, just set ds to zero
        if(stopping) 
        {
            entity.desiredSpeed = 0.0f;
        }
        //If going ds will always be max speed
        //If the heading is slightly off at the beginning the robot can miss the point by a lot. To fix this set dh in every tick like below
        else if(going)
        {
            entity.desiredSpeed = dhds.ds;
            entity.desiredHeading = dhds.dh;
        }
        //If not going easy, just set dh
        else
        {
            entity.desiredHeading = dhds.dh;
        }
        //Update line position
        line.SetPosition(1, destination.position);
    }

    //Get distance needed to come to a complete stop
    public float ComputeDoneDistanceSq()
    {
        //Get time needed for robot to come to a complete stop
        float timeToZeroSpeed = entity.speed / entity.acceleration;
        //To find distance needed to stop we to multiply time by average speed.
        //We already have time, and since velocity changes linearly the average speed is just (current speed + zero) / 2
        float distance = ((entity.speed * timeToZeroSpeed) / 2);// + (0.5f * entity.acceleration * (timeToZeroSpeed * timeToZeroSpeed)));
        return distance * distance;
    }

    public float ComputeRealDistanceSq()
    {
        float distance;

        if(entity.transform.up == destination.transform.up)
        {
            distance = (destination.position - entity.transform.position).magnitude;
            return distance * distance;
        }

        Vector3 diff1 = destination.position - edgePoint;
        Vector3 diff2 = edgePoint - entity.transform.position;
        distance = diff1.magnitude + diff2.magnitude;
        return distance * distance;
    }

    public Vector3 diff = Vector3.positiveInfinity;
    public float dhRadians;
    public float dhDegrees;

    //Calculate the desired speed and desired heading
    public DHDS ComputeDHDS()
    {
        //distance to target point
        if(entity.transform.up == destination.transform.up)
        {
            diff = destination.position - entity.transform.position;
        }
        else
        {
            diff = edgePoint - entity.transform.position;
        }
        //Convert diff from world space to local space
        diff = entity.transform.InverseTransformDirection(diff);
        //Calculate target heading in radians
        dhRadians = Mathf.Atan2(diff.x, diff.z);
        //Calculate target heading in degrees
        dhDegrees = Utils.Degrees360(Mathf.Rad2Deg * dhRadians);
        return new DHDS(dhDegrees, entity.maxSpeed);
    }

    //check if command is done executing
    public override bool IsDone()
    {
        //done if robot is stopping and speed has reached zero
        return (stopping && entity.speed < Utils.EPSILON);
    }

    //called before command is destroyed
    public override void Stop()
    {
        //clear desired speed and heading
        entity.desiredHeading = entity.heading;
        entity.speed = entity.desiredSpeed = 0;
        //destroy line renderer
        LineMgr.inst.DestroyLR(line);
    }

}
