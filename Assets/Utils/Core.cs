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

    private static float Bernstein(int n, int i, float t)
    {
        float t_i = Mathf.Pow(t, i);
        float t_n_minus_i = Mathf.Pow((1 - t), (n - i));

        float basis = BinomCoefficient(n, i) * t_i * t_n_minus_i;
        return basis;
    }

    public static long BinomCoefficient(long n, long k)
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

    public static List<Vector2> GetBezier(List<Vector2> controlPoints, float interval = 0.01f)
    {
        int N = controlPoints.Count - 1;

        List<Vector2> points = new List<Vector2>();
        for (float t = 0.0f; t <= 1.0f; t += interval)
        {
            Vector2 p = new Vector2();
            for (int i = 0; i < controlPoints.Count; ++i)
            {
                Vector2 bn = Bernstein(N, i, t) * controlPoints[i];
                p += bn;
            }
            p = new Vector2((int)p.x, (int)p.y);
            points.Add(p);
        }

        return points;
    }

    public static IEnumerable<Vector2> GetBezier(Vector2 pt0, Vector2 pt1, Vector2 pt2, Vector2 pt3, float dt = 0.05f)
    {
        var points = new List<Vector2>();
        for (float t = 0.0f; t <= 1.0; t += dt)
        {
            points.Add(new Vector2(X(t, pt0.x, pt1.x, pt2.x, pt3.x), Y(t, pt0.y, pt1.y, pt2.y, pt3.y)));
        }

        return points;
    }

    private static int X(float t, float x0, float x1, float x2, float x3)
    {
        return (int)(
            x0 * Math.Pow((1 - t), 3) +
            x1 * 3 * t * Math.Pow((1 - t), 2) +
            x2 * 3 * Math.Pow(t, 2) * (1 - t) +
            x3 * Math.Pow(t, 3)
        );
    }
    private static int Y(float t, float y0, float y1, float y2, float y3)
    {
        return (int)(
            y0 * Math.Pow((1 - t), 3) +
            y1 * 3 * t * Math.Pow((1 - t), 2) +
            y2 * 3 * Math.Pow(t, 2) * (1 - t) +
            y3 * Math.Pow(t, 3)
        );
    }
}
