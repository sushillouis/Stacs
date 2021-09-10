using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Line
{
    public Vector3 point;
    public Vector3 direction;
    public Line(Vector3 p, Vector3 d) {point = p; direction = d;}
}

public class EdgeDetection : MonoBehaviour
{
    public bool seeCircles = false;
    public bool seeEdgePoints = false;
    public bool seeEdges = false;
    public bool seeTrusses = false;
    public float radius = 5.0f;
    [Range(0.0f, 0.125f)]
    public float step = 0.1f;
    public float overlapSphereRadius;
    public Transform targetObject;
    public bool button = false;
    public float detailMultiplier = 1.0f;
    public float duration = 0.01f;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            float time = Time.realtimeSinceStartup;
            FindTrusses(targetObject.position);
            Debug.Log("Time: " + (Time.realtimeSinceStartup - time));
        }
        else if(!button)
        {
            FindTrusses(targetObject.position);
        }
    }

    void FindTrusses(Vector3 p)
    {
        List<Line> edges = FindEdges(targetObject.position);
        for(int i = 0; i < edges.Count; i++)
        {
            for(int j = i + 1; j < edges.Count; j++)
            {
                if(i == j) continue;
                if(IsTruss(edges[i], edges[j]))
                {
                    if(seeTrusses)
                    {
                        Vector3 start = edges[i].point + ((edges[j].point - edges[i].point) / 2.0f);
                        Vector3 dir = edges[i].direction;
                        Line truss = new Line(start, dir);
                        Debug.DrawLine(truss.point, truss.point + (truss.direction * 1000.0f), Color.yellow, duration);
                        Debug.DrawLine(truss.point, truss.point - (truss.direction * 1000.0f), Color.yellow, duration);
                    }
                }
            }
        }
    }

    bool IsTruss(Line l1, Line l2)
    {
        if(!Parallel(l1, l2))
        {
            return false;
        }
        if(SameLine(l1, l2))
        {
            return false;
        }
        if(DistanceBetweenParallel(l1, l2) > 3.5)
        {
            return false;
        }
        
        return true;
    }

    bool Parallel(Line l1, Line l2)
    {
        return Vector3.Cross(l1.direction, l2.direction).magnitude < 0.1f;
    }

    bool SameLine(Line l1, Line l2)
    {
        if(!Parallel(l1, l2))
        {
            return false;
        }
        return DistanceBetweenParallel(l1, l2) < 0.1f;
    }

    float DistanceBetweenParallel(Line l1, Line l2)
    {
        return Vector3.Cross(l1.direction, (l1.point - l2.point)).magnitude;
    }

    List<Line> FindEdges(Vector3 p)
    {
        List<Vector3> points = GetEdgePoints(p, radius);
        List<Line> edges = new List<Line>();
        foreach(Vector3 v in points)
        {
            List<Vector3> subPoints = GetEdgePoints(v, 0.2f, detailMultiplier);
            if(subPoints.Count == 2)
            {
                Vector3 start = subPoints[0];
                Vector3 dir = subPoints[1] - subPoints[0];
                dir.Normalize();
                if(seeEdges)
                {
                    Debug.DrawLine(start, start + (dir * 1000.0f), Color.magenta, duration);
                    Debug.DrawLine(start, start - (dir * 1000.0f), Color.magenta, duration);
                }
                Line l = new Line(start, dir);
                if(!CheckExists(edges, l))
                edges.Add(l);
            }
            if(seeEdgePoints)
            {
                Debug.DrawLine(p, v, Color.green, duration);
                foreach(Vector3 subV in subPoints)
                {
                    Debug.DrawLine(v, subV, Color.green, duration);
                }
            }
        }
        return edges;
    }

    bool CheckExists(List<Line> lines, Line l)
    {
        foreach(Line line in lines)
        {
            if(SameLine(line, l))
                return true;
        }
        return false;
    }

    List<Vector3> GetEdgePoints(Vector3 p, float rad, float detail = 1.0f)
    {
        List<Vector3> points = new List<Vector3>();

        if(step <= 0)
        {return null;}

        bool lastVal = true;
        Vector3 lastPoint = Vector3.zero;
        for(float i = 0; i < 1; i += (step / detail))
        {
            Vector3 point  = GetPointOnCircle(p, i, rad);
            int mask = (1 << LayerMask.NameToLayer("Truss"));
            bool val = Physics.Raycast(point, -transform.up, 0.5f, mask);
            Color col = (val) ? Color.red : Color.blue;
            if(seeCircles)
            {
                Debug.DrawLine(p, point, col, duration);
            }
            if(val != lastVal && lastPoint != Vector3.zero)
            {
                points.Add(lastPoint + ((point - lastPoint) / 2.0f));
            }
            lastPoint = point;
            lastVal = val;
        }

        return points;
    }

    Vector3 GetPointOnCircle(Vector3 p, float distAlongCirc, float rad)
    {
        Vector3 firstPoint = new Vector3(0, 0, 1);
        float angle = 360.0f * distAlongCirc * Mathf.Deg2Rad;
        float x = Mathf.Cos(angle + Mathf.Acos(firstPoint.x));
        float z = Mathf.Sin(angle + Mathf.Asin(firstPoint.z));
        Vector3 point = new Vector3(x, 0, z);
        point *= rad;
        point = transform.TransformPoint(point) + (p - transform.position);
        return point;
    }
}