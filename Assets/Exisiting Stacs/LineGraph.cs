using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class DynamicLine
{
    public LineRenderer lr;
    public List<Vector3> points;
    public List<Vector3> plottedPoints;
    public float minx;
    public float maxx;
    public float diffx;
    public float miny;
    public float maxy;
    public float diffy;
    public float minz;
    public float maxz;
    public float diffz;

    public DynamicLine(LineRenderer ilr)
    {
        lr = ilr;
        Init();
    }

    public void Init()
    {
        points = new List<Vector3>();
        plottedPoints = new List<Vector3>();
        SetMinMax();
    }

    public void SetMinMax()
    {
        if(points.Count > 0)
        {
            minx = points.Min(point => point.x);
            maxx = points.Max(point => point.x);
            diffx = maxx - minx +1;

            miny = points.Min(point => point.y);
            maxy = points.Max(point => point.y);
            diffy = maxy - miny +1;

            miny = points.Min(point => point.z);
            maxy = points.Max(point => point.z);
            diffz = maxz - minz + 1;
        }
    }

    public void Add(Vector3 vec)
    {
        points.Add(vec);
        Redraw();
    }

    public void Redraw()
    {
        SetMinMax();
        lr.positionCount = points.Count();
        plottedPoints.Clear();
        foreach(Vector3 point in points)
        {
            Vector3 p = ScaleOffsetPoint(point);
            plottedPoints.Add(p);
        }
        lr.SetPositions(plottedPoints.ToArray());

    }

    public float factor;
    public Vector3 offset = Vector3.zero;
    public Vector3 ScaleOffsetPoint(Vector3 point)
    {
        Vector3 scaledPoint = new Vector3(0, 0, 0);
        scaledPoint.x = (point.x / diffx) * factor;
        scaledPoint.y = (point.y / diffy) * factor;
        scaledPoint.z = (point.z / diffz) * factor;

        scaledPoint += offset;

        return scaledPoint;
    }

}

public class LineGraph : MonoBehaviour
{

    public LineRenderer SampleLR;

    public DynamicLine SampleLine;

    // Start is called before the first frame update
    void Start()
    {
        SampleLine = new DynamicLine(SampleLR);
        SampleLine.offset = new Vector3(-50, -50, -2);
        SampleLine.factor = 100f;
        SampleLine.Add(Vector3.zero);
        

    }

    // Update is called once per frame
    void Update()    {
        if(Time.frameCount % 2 == 0 && entity != null)        {
            //Vector3 point = new Vector3(Time.frameCount, Random.Range(0, 50), 0);
            //SampleLine.Add(point);
            SampleLine.points = entity.SensorData.readings;
            SampleLine.Redraw();
        }
        
    }

    StacsEntity entity = null;
    public void ConnectToEntity(StacsEntity ent)
    {
        entity = ent;
        SampleLine = new DynamicLine(SampleLR);
        SampleLine.offset = new Vector3(-50, 0, -2);
        SampleLine.factor = 100f;
        SampleLine.points = ent.SensorData.readings;
        SampleLine.Redraw();
    }
}
