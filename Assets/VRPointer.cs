using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRPointer : MonoBehaviour
{
    public LineRenderer lr;

    // Start is called before the first frame update
    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = 2;
    }

    // Update is called once per frame
    void Update()
    {
        if (OVRInput.Get(OVRInput.Button.One))
        {
            GiveCommand();
        }
    }

    void GiveCommand()
    {
        if (OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger) != 0.0f)
        {
            if (!lr.enabled)
            {
                lr.enabled = true;
            }
            lr.SetPosition(0, transform.position);
            Debug.Log("ButtonPressed");
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, 1000.0f))
            {
                lr.SetPosition(1, hit.point);
                AIMgr.inst.HandleCommand(hit.point, hit.normal, hit.point, Vector3.zero, SelectionMgr.inst.selectedEntity);
            }
            else
            {
                lr.SetPosition(1, transform.position + (transform.forward * 100.0f));
            }
        }
        else if (lr.enabled)
        {
            lr.enabled = false;
        }
    }
}
