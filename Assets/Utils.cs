using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{


    public static float EPSILON = 0.05f;
    public static float ANGLE_EPSILON = 0.05f;
    public static bool ApproximatelyEqual(float a, float b, float precision = 0.05f)
    {
        return (Mathf.Abs(a - b) < EPSILON);
    }
    public static bool ApproximatelyEqualAngle(float a, float b, float precision=0.05f)
    {
        return (Mathf.Abs(a - b) < ANGLE_EPSILON);
    }

    public static float Clamp(float val, float min, float max)
    {
        if (val < min)
            val = min;
        if (val > max)
            val = max;
        return val;
    }

    public static float TruncateToPrecision(float value, int precision)
    {
        float step = Mathf.Pow(10, precision);
        int tmp = Mathf.RoundToInt(step * value);
        return tmp / step;

    }

    public static float AngleDiffPosNeg(float a, float b)
    {
        float diff = a - b;
        if (diff > 180)
            return diff - 360;
        if (diff < -180)
            return diff + 360;
        return diff;
    }

    public static float Degrees360(float angleDegrees)
    {
        while (angleDegrees >= 360)
            angleDegrees -= 360;
        while (angleDegrees < 0)
            angleDegrees += 360;
        return angleDegrees;

    }

    public static float VectorToHeadingDegrees(Vector3 v)
    {
        return Mathf.Atan2(v.x, v.z) * Mathf.Rad2Deg;
    }

    public static void CPA(StacsEntity e1, StacsEntity e2)
    {


    }

    public static Vector3 GetEdgePoint(Vector3 p1, Vector3 p2, Vector3 norm1, Vector3 norm2)
    {
        if(norm1 == norm2)
        {
            return Vector3.zero;
        }

        //Vector between points
        Vector3 diff = p2 - p1;
        //Project difference onto the two planes
        Vector3 diff1 = Vector3.ProjectOnPlane(diff, norm1);
        Vector3 diff2 = Vector3.ProjectOnPlane(diff, norm2);

        Plane plane1 = new Plane(norm2, p2);
        Ray ray1 = new Ray(p1, diff1 * 1.1f);
        float hit1 = 0.0f;
        plane1.Raycast(ray1, out hit1);
        diff1 = ray1.GetPoint(hit1) - p1;

        Plane plane2 = new Plane(norm1, p1);
        Ray ray2 = new Ray(p2, -diff2 * 1.1f);
        float hit2 = 0.0f;
        plane2.Raycast(ray2, out hit2);
        diff2 = ray2.GetPoint(hit2) - p2;

        //Get the "y axis" on each plane
        Vector3 y1 = Vector3.ProjectOnPlane(norm2, norm1);
        Vector3 y2 = Vector3.ProjectOnPlane(norm1, norm2);

        //Get the "x axis", same for both planes
        Vector3 cross = Vector3.Cross(norm1, norm2);
        //Project difference onto this axis to get "x difference"
        Vector3 xDiff = Vector3.Project(diff1, cross);

        //Project each of the y differences onto their respective y axes
        Vector3 yDiff1 = Vector3.Project(diff1, y1);
        Vector3 yDiff2 = Vector3.Project(diff2, y2);

        //Get total x and y magnitudes to find the angle theta
        float x = xDiff.magnitude;
        float y = yDiff1.magnitude + yDiff2.magnitude;
        float theta = Mathf.Atan(y / x);

        //Get the vector from p1 to crossover point on x axis
        Vector3 xDiff1 = (xDiff / xDiff.magnitude) * (yDiff1.magnitude / Mathf.Tan(theta));

        return p1 + xDiff1 + yDiff1;
    }

}
