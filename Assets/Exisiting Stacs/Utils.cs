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



}
