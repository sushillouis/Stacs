using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowParentWithOffset : MonoBehaviour
{

    public Transform TransformToTrack;


    // Start is called before the first frame update
    void Start()
    {
        TransformToTrack = transform.parent;
    }

    Vector3 pos = Vector3.zero;
    // Update is called once per frame
    void Update()
    {
        pos = TransformToTrack.position;
        pos.z += 50;
        transform.position = pos;
    }
}
