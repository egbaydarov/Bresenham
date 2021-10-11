using UnityEngine;

public class DrawCircle : DragAndDropDrawer
{
    protected override void DrawFigure(Vector3 start, Vector3 end, bool fill = false)
    {
        var radius = (int) Vector3.Distance(start, end);
        var param = (5 - radius * 4) / 4;
        var dot = new Vector2Int(0, radius);
        do
        {
            SetPixels(start, dot, fill);

            if (param < 0)
            {
                param += 2 * dot.x + 1;
            }
            else
            {
                param += 2 * (dot.x - dot.y) + 1;
                dot.y--;
            }

            dot.x++;
        } while (dot.x <= dot.y);
    }

    private void SetPixels(Vector2 start, Vector2 dot, bool fill)
    {
        this.SetPixel(start.x + dot.x, start.y + dot.y);
        this.SetPixel(start.x + dot.x, start.y - dot.y);
        this.SetPixel(start.x - dot.x, start.y + dot.y);
        this.SetPixel(start.x - dot.x, start.y - dot.y);
        this.SetPixel(start.x + dot.y, start.y + dot.x);
        this.SetPixel(start.x + dot.y, start.y - dot.x);
        this.SetPixel(start.x - dot.y, start.y + dot.x);
        this.SetPixel(start.x - dot.y, start.y - dot.x);

        if (fill)
        {
            var x1 = (int)(start.x - dot.x);
            var x2 = (int)(start.x + dot.x);
            var y1 = (int)(start.y - dot.y);
            var y2 = (int)(start.y + dot.y);

            for (int i = x1; i < x2; ++i)
            {
                this.SetPixel(i, y1);
                this.SetPixel(i, y2);
            }
            
            x1 = (int)(start.x - dot.y);
            x2 = (int)(start.x + dot.y);
            y1 = (int)(start.y - dot.x);
            y2 = (int)(start.y + dot.x);
            
            for (int i = x1; i < x2; ++i)
            {
                this.SetPixel(i, y1);
                this.SetPixel(i, y2);
            }
        }
    }
}