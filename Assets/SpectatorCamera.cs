using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorCamera : MonoBehaviour
{
    public Vector3 offset;
    public Transform position;
    public Transform rotation;

    // Update is called once per frame
    void Update()
    {
        transform.position = position.position + offset;
        transform.rotation = rotation.rotation;
    }
}
