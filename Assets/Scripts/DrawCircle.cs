using UnityEngine;

public class DrawCircle : BresenhamDrawer
{
    protected override void Bresenham(Vector3 src, Vector3 dest)
    {
        var radius = (int) Vector3.Distance(src, dest);
        var param = (5 - radius * 4) / 4;
        var dot = new Vector2Int(0, radius);
        do
        {
            this.SetPixel(src.x + dot.x, src.y + dot.y);
            this.SetPixel(src.x + dot.x, src.y - dot.y);
            this.SetPixel(src.x - dot.x, src.y + dot.y);
            this.SetPixel(src.x - dot.x, src.y - dot.y);
            this.SetPixel(src.x + dot.y, src.y + dot.x);
            this.SetPixel(src.x + dot.y, src.y - dot.x);
            this.SetPixel(src.x - dot.y, src.y + dot.x);
            this.SetPixel(src.x - dot.y, src.y - dot.x);

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

    protected override void Standard(Vector3 src, Vector3 dest)
    {
        //Unity have no standard method for draw on texture
    }
}