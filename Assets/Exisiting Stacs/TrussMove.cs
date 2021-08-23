using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrussMove : Command
{
    public Vector3 movePosition;
    public LineRenderer potentialLine;

    public float doneDistanceSq;

    public TrussMove(StacsEntity ent, Vector3 pos) : base(ent)
    {
        movePosition = pos;
        doneDistanceSq = (ent.length * ent.length);
    }

    public override void Init()//Should be moved into Move constructor
    {
        //Debug.Log("Truss Move Init:\tMoving to: " + movePosition);
        line = LineMgr.inst.CreateMoveLine(entity.position, movePosition);
        line.gameObject.SetActive(false);
        potentialLine = LineMgr.inst.CreatePotentialLine(entity.position);
        potentialLine.gameObject.SetActive(false);
    }

    public bool stopping = false;
    public bool going = true;
    public override void Tick()
    {
        DHDS dhds;
        dhds = ComputeDHDS();
        doneDistanceSq = ComputeDoneDistanceSq();

        if((entity.transform.position - movePosition).sqrMagnitude <= doneDistanceSq)
            stopping = true;
        else if(Utils.ApproximatelyEqualAngle(dhds.dh, entity.heading))
            going = true;

        if(stopping)  {
            entity.desiredSpeed = 0.0f;
        } else if(going)   {
            entity.desiredSpeed = dhds.ds;
            entity.desiredHeading = dhds.dh;
        } else {
            entity.desiredHeading = dhds.dh;
        }
        line.SetPosition(1, movePosition);

    }

    public float ComputeDoneDistanceSq()
    {
        float timeToZeroSpeed = entity.speed / entity.acceleration;
        float distance = ((entity.speed * timeToZeroSpeed) / 2);// + (0.5f * entity.acceleration * (timeToZeroSpeed * timeToZeroSpeed)));
        return distance * distance;
    }

    public Vector3 diff = Vector3.positiveInfinity;
    public float dhRadians;
    public float dhDegrees;

    public DHDS ComputeDHDS()
    {
        diff = movePosition - entity.transform.position;
        diff = entity.transform.InverseTransformDirection(diff);
        dhRadians = Mathf.Atan2(diff.x, diff.z);
        dhDegrees = Utils.Degrees360(Mathf.Rad2Deg * dhRadians);
        return new DHDS(dhDegrees, entity.maxSpeed);
    }

    public override bool IsDone()
    {
        return (stopping && entity.speed < Utils.EPSILON);
    }

    public override void Stop()
    {
        entity.desiredHeading = entity.heading;
        entity.speed = entity.desiredSpeed = 0;
        
        LineMgr.inst.DestroyLR(line);
        LineMgr.inst.DestroyLR(potentialLine);
    }

}
