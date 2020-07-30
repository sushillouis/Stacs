using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookObject : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.parent.position;
        pos.z += 50;
        transform.position = pos;
    }
}
