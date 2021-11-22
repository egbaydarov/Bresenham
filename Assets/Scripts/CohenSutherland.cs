using System;
using UnityEngine;

[RequireComponent(typeof(DrawLine))]
public class CohenSutherland : DragAndDropDrawer
{
    [Flags]
    private enum OutCode
    {
        Inside = 0,
        Left = 1,
        Right = 2,
        Bottom = 4,
        Top = 8
    }
    
    protected override void DrawFigure(Vector3 start, Vector3 end, bool fill = false)
    {
        var lineDrawer = GetComponent<DrawLine>();

        var left = (int) Math.Min(start.x, end.x);
        var right = (int) Math.Max(start.x, end.x);
        var up = (int) Math.Min(start.y, end.y);
        var down = (int) Math.Max(start.y, end.y);
        for (int i = left; i < right; ++i)
        {
            this.SetPixel(i, up);
            this.SetPixel(i, down);
        }
        for (int i = up; i < down; ++i)
        {
            this.SetPixel(left, i);
            this.SetPixel(right, i);
        }

        var rect = new Rect(left, up, right - left, down - up);

        foreach (var lines in lineDrawer.Cache)
        {
            var segment = ClipSegment(rect, lines.Item1, lines.Item2);
            if (segment != null)
            {
                DrawLine(segment.Item2, segment.Item1);
            }
        }
    }
    
    private OutCode ComputeOutCode(float x, float y, Rect r)
    {
        var code = OutCode.Inside;

        if (x < r.xMin) code |= OutCode.Left;
        if (x > r.xMax) code |= OutCode.Right;
        if (y < r.yMin) code |= OutCode.Top;
        if (y > r.yMax) code |= OutCode.Bottom;

        return code;
    }

    private OutCode ComputeOutCode(Vector2 p, Rect r)
    {
        return ComputeOutCode(p.x, p.y, r);
    }

    private static Vector2 CalculateIntersection(Rect r, Vector2 p1, Vector2 p2, OutCode clipTo)
    {
        var dx = (p2.x - p1.x);
        var dy = (p2.y - p1.y);

        var slopeY = dx / dy;
        var slopeX = dy / dx;

        if (clipTo.HasFlag(OutCode.Top))
        {
            return new Vector2(
                p1.x + slopeY * (r.yMin - p1.y),
                r.yMin
            );
        }
        if (clipTo.HasFlag(OutCode.Bottom))
        {
            return new Vector2(
                p1.x + slopeY * (r.yMax - p1.y),
                r.yMax
            );
        }
        if (clipTo.HasFlag(OutCode.Right))
        {
            return new Vector2(
                r.xMax,
                p1.y + slopeX * (r.xMax - p1.x)
            );
        }
        if (clipTo.HasFlag(OutCode.Left))
        {
            return new Vector2(
                r.xMin,
                p1.y + slopeX * (r.xMin - p1.x)
            );
        }

        throw new ArgumentOutOfRangeException("clipTo = " + clipTo);
    }

    private Tuple<Vector2, Vector2> ClipSegment(Rect r, Vector2 p1, Vector2 p2)
    {
        var outCodeP1 = ComputeOutCode(p1, r);
        var outCodeP2 = ComputeOutCode(p2, r);
        var accept = false;

        while (true)
        {
            if ((outCodeP1 | outCodeP2) == OutCode.Inside)
            {
                accept = true;
                break;
            }
            if ((outCodeP1 & outCodeP2) != 0)
            {
                break;
            }
            var outCode = outCodeP1 != OutCode.Inside ? outCodeP1 : outCodeP2;
            var p = CalculateIntersection(r, p1, p2, outCode);
            if (outCode == outCodeP1)
            {
                p1 = p;
                outCodeP1 = ComputeOutCode(p1, r);
            }
            else
            {
                p2 = p;
                outCodeP2 = ComputeOutCode(p2, r);
            }
        }
        if (accept)
        {
            return new Tuple<Vector2, Vector2>(p1, p2);
        }
        return null;
    }

    private void DrawLine(Vector3 p1, Vector3 p2)
    {
        var start = new Vector2Int((int)p1.x, (int)p1.y);
        var end = new Vector2Int((int)p2.x, (int)p2.y);
        var size = new Vector2(end.x - start.x, end.y - start.y);
        var delta1 = Vector2Int.zero;
        var delta2 = Vector2Int.zero;

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

        var counter = (int) l >> 1;
        for (var i = 0; i <= l; ++i)
        {
            this.SetPixel(start.x, start.y);
            if ((counter += (int) s) >= l)
            {
                counter -= (int) l;
                start += delta1;
            }
            else
            {
                start += delta2;
            }
        }
    }
}