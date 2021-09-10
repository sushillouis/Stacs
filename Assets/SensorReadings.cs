using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorReadings : MonoBehaviour
{
    public StacsEntity entity;

    public List<Vector3> readings;
    public List<DefectViz> defects;

    void Awake()
    {
        entity = GetComponentInParent<StacsEntity>();
        readings = new List<Vector3>();
    }


    // Start is called before the first frame update
    void Start()
    {
        readings.Clear();
    }

    Vector3 diff;
    public float threshold = 3f;
    public float nextDataStep = 1;
    // Update is called once per frame
    void Update()
    {
        if(Time.realtimeSinceStartup > nextDataStep && entity.speed > Utils.EPSILON)
        {
            nextDataStep = Time.realtimeSinceStartup + 1f;
            foreach(DefectViz defect in defects)
            {
                diff = defect.transform.position - transform.position;
                if(diff.magnitude < threshold)
                    MakeReading(diff.magnitude, entity, defect);
                else
                    MakeReading(-1, entity, defect);
            }
        }

    }

    public void MakeReading(float distance, StacsEntity entity, DefectViz defect)
    {
        Vector3 reading = new Vector3(Time.frameCount, 0, 0);

        if(distance < 0)
        {
            reading.y = 0;
        } else
        {
            reading.y = 1 - distance / threshold;
            //Debug.Log("Reading " + readings.Count + " : " + reading.y);
            defect.Detect();

        }
        readings.Add(reading);

    }

    public GameObject DefectRoot;

    [ContextMenu("LoadDefects")]
    public void LoadDefects()
    {
        defects.Clear();
        foreach(Transform t in DefectRoot.GetComponentsInChildren<Transform>())
        {
            if(t.name.StartsWith("Cube"))
            {
                defects.Add(t.GetComponent<DefectViz>());
            }
        }
    }

}
