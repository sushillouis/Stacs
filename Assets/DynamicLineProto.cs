using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class DynamicLineProto 
{
    public LineRenderer lr;
    public float height;
    public float width;
    public float border;

    public float minx, miny, maxx, maxy, minz, maxz;
    public float diffx, diffy, diffz;
    public float xscale;
    public float yscale;
    public Vector3 offset;

    public List<Vector3> points;
    public List<Vector3> plottedPoints;

    public DynamicLineProto(LineRenderer l, Vector3 offst)
    {
        lr = l;
        height = 100;
        width = 100;
        border = 5;
        points = new List<Vector3>();
        plottedPoints = new List<Vector3>();
        offset = offst;

    }

    public void SetMinMax()
    {
        if(points.Count > 0) {
            minx = points.Min(point => point.x);
            maxx = points.Max(point => point.x);

            miny = points.Min(point => point.y);
            maxy = points.Max(point => point.y);

            minz = points.Min(point => point.z);
            maxz = points.Max(point => point.z);
        } else {
            minx = 0; miny = 0;  minz = 0;
            maxx = 100; maxy = 100;  maxz = 0;
        }
        diffx = maxx - minx;
        diffy = maxy - miny;
        diffz = 1;
    }

    public void SetXYScales()
    {
        if(diffx > 0)
            xscale = width / diffx;
        else
            xscale = 1;

        if(diffy > 0)
            yscale = height / diffy;
        else
            yscale = 1;
    }

    public void Add(Vector3 point)
    {
        points.Add(point);
        Redraw();
    }
    
    public Vector3 ScaleOffsetPoint(Vector3 point)
    {
        Vector3 p = new Vector3((point.x - minx) * xscale, 
            (point.y - miny) * yscale, point.z);
        p += offset;
        return p;
    }

    public void Redraw()
    {
        SetMinMax();
        SetXYScales();
        lr.positionCount = points.Count();
        plottedPoints.Clear();
        foreach(Vector3 point in points) {
            Vector3 p = ScaleOffsetPoint(point);
            plottedPoints.Add(p);
        }
        lr.SetPositions(plottedPoints.ToArray());
    }
}
