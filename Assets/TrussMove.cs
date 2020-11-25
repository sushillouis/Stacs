using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TrussMove : Command
{
    //target position
    public Vector3 movePosition;
    //public LineRenderer potentialLine;

    //distance needed for robot to come to a complete stop
    public float doneDistanceSq;

    //CONSTRUCTOR
    public TrussMove(StacsEntity ent, Vector3 pos) : base(ent)
    {
        //Get position in the plane of movement of the robot. So vertex placement doesn't have to be exact.
        //This will need updating when vertices may be on different planes
        Plane plane = new Plane(entity.transform.up, entity.transform.position);
        movePosition = plane.ClosestPointOnPlane(pos);
        //Create line but leave deactive until command begins
        line = LineMgr.inst.CreateMoveLine(entity.position, movePosition);
        line.gameObject.SetActive(false);
    }

    //true if robot is slowing down to stop
    public bool stopping = false;
    //true if robot has aligned heading and began moving
    public bool going = false;
    public override void Tick()
    {
        //Get desired speed and heading at current time
        DHDS dhds;
        dhds = ComputeDHDS();
        //Get distance needed to come to a complete stop at current speed
        doneDistanceSq = ComputeDoneDistanceSq();

        //Check if time needed to stop is greater than remaining distance, if so begin stopping.
        //This method means the robot will overshoot, but it is only noticable when acceleration is very high.
        if((entity.transform.position - movePosition).sqrMagnitude <= doneDistanceSq)
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
        line.SetPosition(1, movePosition);
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

    public Vector3 diff = Vector3.positiveInfinity;
    public float dhRadians;
    public float dhDegrees;

    //Calculate the desired speed and desired heading
    public DHDS ComputeDHDS()
    {
        //distance to target point
        diff = movePosition - entity.transform.position;
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
