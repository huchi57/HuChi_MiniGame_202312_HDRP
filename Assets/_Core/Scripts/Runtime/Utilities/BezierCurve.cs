using System;
using UnityEngine;

namespace UrbanFox
{
    [Serializable]
    public enum BezierCurveType
    {
        Quadratic,
        Cubic
    }

    public static class BezierCurve
    {
        public static Vector3 GetPointOnQuadraticCurve(Vector3 p0, Vector3 p1, Vector3 p2, float time, bool drawCurve = false)
        {
            float tt = time * time;
            float u = 1f - time;
            float uu = u * u;

            Vector3 result = uu * p0;
            result += 2f * u * time * p1;
            result += tt * p2;

            if (drawCurve)
            {
                DrawQuadraticCurve(p0, p1, p2, time);
            }

            return result;
        }

        public static Vector3 GetPointOnCubicCurve(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float time, bool drawCurve = false)
        {
            float tt= time * time;
            float ttt = time * tt;

            float u = 1f - time;
            float uu = u * u;
            float uuu = u * uu;

            Vector3 result = uuu * p0;
            result += 3f * uu * time * p1;
            result += 3f * u * tt * p2;
            result += ttt * p3;

            if (drawCurve)
            {
                DrawCubicCurve(p0, p1, p2, p3, time);
            }

            return result;
        }

        public static Vector3 GetPointOnCurve(Vector3[] points, float time, BezierCurveType curveType, bool drawCurve = false)
        {
            if (points.IsNullOrEmpty())
            {
                return Vector3.zero;
            }
            if (points.Length == 1)
            {
                return points[0];
            }
            if (points.Length == 2)
            {
                return Vector3.Lerp(points[0], points[1], time);
            }
            if (points.Length == 3)
            {
                return GetPointOnQuadraticCurve(points[0], points[1], points[2], time, drawCurve);
            }

            switch (curveType)
            {
                case BezierCurveType.Quadratic:
                    int numberOfCurves = points.Length / (3 - 1);
                    float subTotal = 1f / numberOfCurves;

                    float remainder = time % subTotal;
                    float localPercentage = remainder / subTotal;
                    int localStartIndex = (int)(time / subTotal) * (3 - 1);

                    int index0 = localStartIndex;
                    int index1 = localStartIndex + 1;
                    int index2 = localStartIndex + 2;

                    if (index0 < points.Length - 2)
                    {
                        var p0 = points[index0];
                        var p1 = points[index1];
                        var p2 = points[index2];
                        return GetPointOnQuadraticCurve(p0, p1, p2, localPercentage, drawCurve);
                    }

                    var start = points[index0];
                    var end = points[points.Length - 1];
                    return Vector3.Lerp(start, end, localPercentage);

                case BezierCurveType.Cubic:
                    numberOfCurves = points.Length / (4 - 1);
                    subTotal = 1f / numberOfCurves;

                    remainder = time % subTotal;
                    localPercentage = remainder / subTotal;
                    localStartIndex = (int)(time / subTotal) * (4 - 1);

                    index0 = localStartIndex;
                    index1 = localStartIndex + 1;
                    index2 = localStartIndex + 2;
                    var index3 = localStartIndex + 3;

                    if (index0 < points.Length - 3)
                    {
                        var p0 = points[index0];
                        var p1 = points[index1];
                        var p2 = points[index2];
                        var p3 = points[index3];
                        return GetPointOnCubicCurve(p0, p1, p2, p3, localPercentage, drawCurve);
                    }

                    start = points[index0];
                    end = points[points.Length - 1];
                    return Vector3.Lerp(start, end, localPercentage);

                default:
                    break;
            }
            return GetPointOnQuadraticCurve(points[0], points[1], points[2], time, drawCurve);
        }

        public static void DrawQuadraticCurve(Vector3 p0, Vector3 p1, Vector3 p2, float time)
        {
            Debug.DrawLine(p0, p1, Color.green);
            Debug.DrawLine(p1, p2, Color.green);

            var a = Vector3.Lerp(p0, p1, time);
            var b = Vector3.Lerp(p1, p2, time);
            Debug.DrawLine(a, b, Color.yellow);
        }

        public static void DrawCubicCurve(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float time)
        {
            Debug.DrawLine(p0, p1, Color.green);
            Debug.DrawLine(p1, p2, Color.green);
            Debug.DrawLine(p2, p3, Color.green);

            var a = Vector3.Lerp(p0, p1, time);
            var b = Vector3.Lerp(p1, p2, time);
            var c = Vector3.Lerp(p2, p3, time);
            Debug.DrawLine(a, b, Color.yellow);
            Debug.DrawLine(b, c, Color.yellow);

            var aa = Vector3.Lerp(a, b, time);
            var bb = Vector3.Lerp(b, c, time);
            Debug.DrawLine(aa, bb, Color.red);
        }

        public static void DrawCurve(Vector3[] points, float time, BezierCurveType curveType)
        {
            if (points.IsNullOrEmpty() || points.Length == 1)
            {
                return;
            }
            for (int i = 0; i < points.Length - 1; i++)
            {
                Debug.DrawLine(points[i], points[i + 1], Color.white);
            }
            Debug.DrawLine(points[points.Length - 1], points[0], Color.white);
            
            if (points.Length == 2)
            {
                var midPoint = Vector3.Lerp(points[0], points[1], time);
                Debug.DrawLine(points[0], midPoint, Color.green);
            }

            switch (curveType)
            {
                case BezierCurveType.Quadratic:
                    int numberOfCurves = points.Length / (3 - 1);
                    float subTotal = 1f / numberOfCurves;

                    float remainder = time % subTotal;
                    float localPercentage = remainder / subTotal;
                    int localStartIndex = (int)(time / subTotal) * (3 - 1);

                    int index0 = localStartIndex;
                    int index1 = localStartIndex + 1;
                    int index2 = localStartIndex + 2;

                    if (index0 < points.Length - 2)
                    {
                        var p0 = points[index0];
                        var p1 = points[index1];
                        var p2 = points[index2];
                        DrawQuadraticCurve(p0, p1, p2, localPercentage);
                        break;
                    }

                    //var start = points[index0];
                    //var end = points[points.Length - 1];
                    //return Vector3.Lerp(start, end, localPercentage);
                    break;

                case BezierCurveType.Cubic:
                    numberOfCurves = points.Length / (4 - 1);
                    subTotal = 1f / numberOfCurves;

                    remainder = time % subTotal;
                    localPercentage = remainder / subTotal;
                    localStartIndex = (int)(time / subTotal) * (4 - 1);

                    index0 = localStartIndex;
                    index1 = localStartIndex + 1;
                    index2 = localStartIndex + 2;
                    var index3 = localStartIndex + 3;

                    if (index0 < points.Length - 3)
                    {
                        var p0 = points[index0];
                        var p1 = points[index1];
                        var p2 = points[index2];
                        var p3 = points[index3];
                        DrawCubicCurve(p0, p1, p2, p3, localPercentage);
                        break;
                    }

                    //start = points[index0];
                    //end = points[points.Length - 1];
                    //return Vector3.Lerp(start, end, localPercentage);
                    break;

                default:
                    break;
            }
        }
    }
}
