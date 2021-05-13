using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathVisualization : MonoBehaviour
{
    public int resolution;
    LineRenderer lr;
    StacsEntity entity;
    ClimbingPhysics physics;

    // Start is called before the first frame update
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = resolution;
        lr.enabled = false;
        entity = GetComponent<StacsEntity>();
        physics = GetComponent<ClimbingPhysics>();
    }

    // Update is called once per frame
    void Update()
    {
        if (entity.heading != entity.desiredHeading || entity.speed != 0)
        {
            lr.enabled = true;
            Vector3 start = transform.position + (transform.up * 0.06f);
            float length = Mathf.Clamp(((entity.speed * entity.speed / entity.acceleration) / 2), 1.0f, Mathf.Infinity);
            Debug.Log(physics.localYawNode.transform.forward);
            Vector3 direction = Quaternion.AngleAxis(entity.desiredHeading - entity.heading, transform.up) * physics.localYawNode.transform.forward;
            Debug.Log("Direction: " + direction);
            Vector3 line = direction * length;
            lr.SetPosition(0, start);
            lr.SetPosition(1, start + line);
        }
        else
            lr.enabled = false;
    }
}
