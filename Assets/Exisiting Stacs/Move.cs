using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Move : Command
{
    public Vector3 movePosition;
    public LineRenderer potentialLine;

    public Move(StacsEntity ent, Vector3 pos) : base(ent)
    {
        movePosition = pos;
        doneDistanceSq = (ent.length * ent.length);
        //entity.desiredAltitude = movePosition.y;
    }

    public override void Init()//Should be moved into Move constructor
    {
        //Debug.Log("MoveInit:\tMoving to: " + movePosition);
        line = LineMgr.inst.CreateMoveLine(entity.position, movePosition);
        line.gameObject.SetActive(false);
        potentialLine = LineMgr.inst.CreatePotentialLine(entity.position);
        potentialLine.gameObject.SetActive(false);
    }

    public override void Tick()
    {
        DHDS dhds;
        if (AIMgr.inst.isPotentialFieldsMovement)
            dhds = ComputePotentialDHDS();
        else
            dhds = ComputeDHDS();

        entity.desiredHeading = dhds.dh;
        entity.desiredSpeed = dhds.ds;
        entity.desiredAltitude = movePosition.y;
        line.SetPosition(0, entity.position);

    }

    public float ComputeDoneDistanceSq()
    {
        float timeToZeroSpeed = entity.speed / entity.acceleration;
        float distance = ((entity.speed * timeToZeroSpeed)/2);// + (0.5f * entity.acceleration * (timeToZeroSpeed * timeToZeroSpeed)));
        return distance * distance;
    }

    public Vector3 diff = Vector3.positiveInfinity;
    public float dhRadians;
    public float dhDegrees;
    public DHDS ComputeDHDS()
    {
        diff = movePosition - entity.position;
        dhRadians = Mathf.Atan2(diff.x, diff.z);
        dhDegrees = Utils.Degrees360(Mathf.Rad2Deg * dhRadians);
        return new DHDS(dhDegrees, entity.maxSpeed);
    }

    public DHDS ComputePotentialDHDS()
    {
        Potential p;
        repulsivePotential = Vector3.one; repulsivePotential.y = 0;
        foreach (StacsEntity ent in EntityMgr.inst.entities) {
            if (ent == entity) continue;
            p = DistanceMgr.inst.GetPotential(entity, ent);
            if (p.distance < AIMgr.inst.potentialDistanceThreshold && p.distance > Utils.EPSILON * 10) {
                repulsivePotential += p.direction * ent.mass *
                    AIMgr.inst.repulsiveCoefficient * Mathf.Pow(p.diff.magnitude, AIMgr.inst.repulsiveExponent);
                //repulsivePotential += p.diff;
            }
        }
        //repulsivePotential *= repulsiveCoefficient * Mathf.Pow(repulsivePotential.magnitude, repulsiveExponent);
        attractivePotential = movePosition - entity.position;
        Vector3 tmp = attractivePotential.normalized;
        attractivePotential = tmp * //EntityMgr.inst.entities.Count *
            AIMgr.inst.attractionCoefficient * Mathf.Pow(attractivePotential.magnitude, AIMgr.inst.attractiveExponent);
        potentialSum = attractivePotential - repulsivePotential;

        dh = Utils.Degrees360(Mathf.Rad2Deg * Mathf.Atan2(potentialSum.x, potentialSum.z));

        angleDiff = Utils.Degrees360(Utils.AngleDiffPosNeg(dh, entity.heading));
        cosValue = (Mathf.Cos(angleDiff * Mathf.Deg2Rad) + 1) / 2.0f; // makes it between 0 and 1
        ds = entity.maxSpeed * cosValue;

        return new DHDS(dh, ds);
    }
    public Vector3 attractivePotential = Vector3.zero;
    public Vector3 potentialSum = Vector3.zero;
    public Vector3 repulsivePotential = Vector3.zero;
    public float dh;
    public float angleDiff;
    public float cosValue;
    public float ds;



    public float doneDistanceSq;//computed in constructor based on entity length squared;

    public override bool IsDone()
    {

        return ((entity.position - movePosition).sqrMagnitude < doneDistanceSq);
    }

    public override void Stop()
    {
        entity.desiredSpeed = 0;
        LineMgr.inst.DestroyLR(line);
        LineMgr.inst.DestroyLR(potentialLine);
    }
}
