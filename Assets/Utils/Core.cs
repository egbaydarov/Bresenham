using System;
using System.Collections.Generic;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Core : MonoBehaviour
{
    public static IEnumerable<Vector2> GetLine(Vector3 start, Vector3 end)
    {
        var size = new Vector2(end.x - start.x, end.y - start.y);
        var delta1 = Vector3.zero;
        var delta2 = Vector3.zero;
        var points = new List<Vector2>();

        if (size.x < 0)
        {
            delta1.x = -1;
        }
        else if (size.x > 0)
        {
            delta1.x = 1;
        }

        if (size.y < 0)
        {
            delta1.y = -1;
        }
        else if (size.y > 0)
        {
            delta1.y = 1;
        }

        if (size.x < 0)
        {
            delta2.x = -1;
        }
        else if (size.x > 0)
        {
            delta2.x = 1;
        }

        var l = Math.Abs(size.x);
        var s = Math.Abs(size.y);
        if (l <= s)
        {
            l = Math.Abs(size.y);
            s = Math.Abs(size.x);
            if (size.y < 0) delta2.y = -1;
            else if (size.y > 0) delta2.y = 1;
            delta2.x = 0;
        }

        var counter = (int)l >> 1;
        for (var i = 0; i <= l; ++i)
        {
            points.Add(new Vector2(start.x, start.y));
            if ((counter += (int)s) >= l)
            {
                counter -= (int)l;
                start.x += delta1.x;
                start.y += delta1.y;
            }
            else
            {
                start.x += delta2.x;
                start.y += delta2.y;
            }
        }
        return points;
    }

    public static List<Vector2> GetBezier(List<Vector2> controlPoints, float interval = 0.01f)
    {
        int n = controlPoints.Count - 1;
        List<Vector2> points = new List<Vector2>();
        for (float t = 0.0f; t <= 1.0f; t += interval)
        {
            Vector2 p = new Vector2();
            for (int i = 0; i < controlPoints.Count; ++i)
            {
                p += Bernstein(n, i, t) * controlPoints[i];
            }
            points.Add(Vector2Int.RoundToInt(p));
        }
        return points;
    }

    public static List<Vector2> GetBezier(Vector2 pt0, Vector2 pt1, Vector2 pt2, Vector2 pt3, float dt = 0.01f)
    {
        var points = new List<Vector2>();
        for (float t = 0.0f; t <= 1.0; t += dt)
        {
            points.Add(Bezier(t, pt0, pt1, pt2, pt3));
        }

        return points;
    }
    
    public static List<Vector2> GetComplexBezier(List<Vector2> allNodes, out Vector2 midpoint, bool fill)
    {
        midpoint = Vector2.zero;
        var points = new List<Vector2>();
        var count = allNodes.Count;
        if (count > 0 && count % 4 == 0)
        {
            midpoint = Midpoint(allNodes[count - 2], allNodes[count - 1]);
            var temp = allNodes[count - 1];
            allNodes[count - 1] = midpoint;
            allNodes.Add(midpoint);
            allNodes.Add(temp);
            if (fill)
            {
                var first = allNodes[0];
                var second = allNodes[1];
                allNodes.Add(2 * first - second);
                allNodes.Add(first);
            }

            count = allNodes.Count;
            for (var i = 0; i + 3 < count; i += 4)
            {
                points.AddRange(GetBezier(allNodes[i], allNodes[i + 1], allNodes[i + 2], allNodes[i + 3]));
            }
        }
        
        return points;
    }

    private static Vector2 Midpoint(Vector2 a, Vector2 b)
    {
        return (a + b) / 2;
    }

    private static float Bernstein(int n, int i, float t)
    {
        var ti = Mathf.Pow(t, i);
        var tNmi = Mathf.Pow((1 - t), (n - i));
        return BinCoefficient(n, i) * ti * tNmi;
    }

    private static long BinCoefficient(long n, long k)
    {
        if (k > n) { return 0; }
        if (n == k) { return 1; }
        if (k > n - k) { k = n - k; }
        long c = 1;
        for (long i = 1; i <= k; i++)
        {
            c *= n--;
            c /= i;
        }
        return c;
    }

    private static Vector2 Bezier(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
    {
         var point = p0 * Mathf.Pow(1 - t, 3) +
                p1 * 3 * t * Mathf.Pow(1 - t, 2) +
                p2 * 3 * Mathf.Pow(t, 2) * (1 - t) +
                p3 * Mathf.Pow(t, 3);
         return Vector2Int.RoundToInt(point);
    }
    
    public static List<Vector2> BSplines(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, float dt = 0.01f)
    {
        List<Vector2> points = new List<Vector2>();
        Vector2[] spl = new Vector2[5];
        spl[0] = (-p1 + 3 * p2 - 3 * p3 + p4) / 6.0f;
        spl[1] = (3 * p1 - 6 * p2 + 3 * p3) / 6.0f;
        spl[2] = (-3 * p1 + 3 * p3) / 6.0f;
        spl[3] = (p1 + 4 * p2 + p3) / 6.0f;
        
        for (float t = 0; t <= 1; t += dt)
        {
            points.Add(Vector2Int.RoundToInt((spl[2] + t * (spl[1] + t * spl[0])) * t + spl[3]));
        }

        return points;
    }
    
    public static List<Vector2> GetComplexBSpline(List<Vector2> allNodes, out Vector2 midpoint, bool fill)
    {
        midpoint = Vector2.zero;
        var points = new List<Vector2>();
        var count = allNodes.Count;
        if (count > 0 && count % 4 == 0)
        {
            midpoint = Midpoint(allNodes[count - 2], allNodes[count - 1]);
            var temp = allNodes[count - 1];
            allNodes[count - 1] = midpoint;
            allNodes.Add(midpoint);
            allNodes.Add(temp);
            if (fill)
            {
                var first = allNodes[0];
                var second = allNodes[1];
                allNodes.Add(2 * first - second);
                allNodes.Add(first);
            }

            count = allNodes.Count;
            for (var i = 0; i + 3 < count; i += 4)
            {
                points.AddRange(BSplines(allNodes[i], allNodes[i + 1], allNodes[i + 2], allNodes[i + 3]));
            }
        }
        
        return points;
    }
}
