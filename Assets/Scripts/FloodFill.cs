using System;
using System.Collections.Generic;
using UnityEngine;

public class FloodFill : DragAndDropDrawer
{
    protected override void DrawFigure(Vector3 start, Vector3 end, bool fill = false)
    {
        Stack<Vector3> pixels = new Stack<Vector3>();
        pixels.Push(end);
        TryGetPixel(start.x, start.y, out var initPixel);

        while (pixels.Count > 0)
        {
            var point = pixels.Pop();
            if (TryGetPixelFilled(point.x, point.y, out var pixel) && pixel.Equals(initPixel))
            {
                this.SetPixel(point.x, point.y);
                var up = new Vector3(point.x, point.y + 1);
                var left = new Vector3(point.x - 1, point.y);
                var right = new Vector3(point.x + 1, point.y);
                var down = new Vector3(point.x, point.y - 1);
                pixels.Push(left);
                pixels.Push(right);
                pixels.Push(up);
                pixels.Push(down);
            }
        }
    }
}