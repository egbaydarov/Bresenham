using System.Collections.Generic;
using UnityEngine;

public class FillPolygonScanline : DragAndDropDrawer
{
    protected override void DrawFigure(Vector3 start, Vector3 end, bool fill = false)
    {
        Stack<Vector3> pixels = new Stack<Vector3>();
        pixels.Push(end);
        TryGetPixel(end.x, end.y, out var initPixel);
        bool spanAbove, spanBelow;
        while (pixels.Count != 0)
        {
            var dot = pixels.Pop();
            var x = (int) dot.x;
            var y = (int) dot.y;
            while (TryGetPixel(x - 1, y, out var pixel) && pixel.Equals(initPixel))
            {
                --x;
            }

            spanAbove = spanBelow = false;
            while (TryGetPixel(x, y, out var pixel) && pixel.Equals(initPixel))
            {
                this.SetPixel(x, y);
                if (TryGetPixelFilled(x, y - 1, out var pixel1))
                {
                    if (!spanAbove && pixel1.Equals(initPixel))
                    {
                        pixels.Push(new Vector3(x, y - 1));
                        spanAbove = true;
                    }
                    else if (spanAbove && !pixel1.Equals(initPixel))
                    {
                        spanAbove = false;
                    }
                }

                if (TryGetPixelFilled(x, y + 1, out var pixel2))
                {
                    if (!spanBelow && pixel2.Equals(initPixel))
                    {
                        pixels.Push(new Vector3(x, y + 1));

                        spanBelow = true;
                    }
                    else if (spanBelow && !pixel2.Equals(initPixel))
                    {
                        spanBelow = false;
                    }
                }
                
                x++;
            }
        }
    }
}