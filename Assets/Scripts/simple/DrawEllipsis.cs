using System;
using System.Collections.Generic;
using UnityEngine;

public class DrawEllipsis : DragAndDropDrawer
{
    protected override void DrawFigure(Vector3 start, Vector3 end, bool fill = false)
    {
        var center = (start + end) / 2;
        var diff = (end - start) / 2;
        var radius = new Vector3(Math.Abs(diff.x), Math.Abs(diff.y));
        if (radius.y == 0 || radius.x == 0)
        {
            return;
        }

        var dot = new Vector2(0, radius.y);
        var coeff = 2 * Vector2.Scale(radius, radius);
        var delta = new Vector2(dot.x * coeff.y, dot.y * coeff.x);
        var param1 = radius.y * radius.y
                     - radius.x * radius.x * radius.y
                     + 0.25f * radius.x * radius.x;
        while (delta.x < delta.y)
        {
            SetPixels(dot, center, fill);
            if (param1 < 0)
            {
                dot.x++;
                delta.x += 2 * radius.y * radius.y;
                param1 += delta.x + radius.y * radius.y;
            }
            else
            {
                dot.x++;
                dot.y--;
                delta.x += 2 * radius.y * radius.y;
                delta.y -= 2 * radius.x * radius.x;
                param1 += delta.x - delta.y + radius.y * radius.y;
            }
        }

        var param2 = radius.y * radius.y * (dot.x + 0.5f) * (dot.x + 0.5f)
                     + radius.x * radius.x * (dot.y - 1) * (dot.y - 1)
                     - radius.x * radius.x * radius.y * radius.y;
        while (dot.y >= 0)
        {
            SetPixels(dot, center, fill);
            if (param2 > 0)
            {
                dot.y--;
                delta.y -= 2 * radius.x * radius.x;
                param2 += radius.x * radius.x - delta.y;
            }
            else
            {
                dot.y--;
                dot.x++;
                delta.x += 2 * radius.y * radius.y;
                delta.y -= 2 * radius.x * radius.x;
                param2 += delta.x - delta.y + radius.x * radius.x;
            }
        }
    }

    private void SetPixels(Vector2 dot, Vector2 center, bool fill)
    {
        this.SetPixel(dot.x + center.x, dot.y + center.y);
        this.SetPixel(-dot.x + center.x, dot.y + center.y);
        this.SetPixel(dot.x + center.x, -dot.y + center.y);
        this.SetPixel(-dot.x + center.x, -dot.y + center.y);

        if (fill)
        {
            var y1 = dot.y + center.y;
            var y2 = -dot.y + center.y;
            var x2 = (int)dot.x + (int)center.x;
            var x1 = (int)-dot.x + (int)center.x;
            for (var i = x1; i <= x2; ++i)
            {
                this.SetPixel(i, y1);
                this.SetPixel(i, y2);
            }
        }
    }
}