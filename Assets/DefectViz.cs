using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefectViz : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {


    }

    public int detected = 0;
    public void Detect()
    {
        detected += 1;
        Vector3 scale = transform.localScale;
        scale.z = 3;
        transform.localScale = scale;
    }

}
