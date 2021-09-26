using System;
using UnityEngine;

public class DrawLine : BresenhamDrawer
{
    protected override void Bresenham(Vector3 src, Vector3 dest)
    {
        var size = new Vector2(dest.x - src.x, dest.y - src.y);
        var delta1 = Vector3.zero;
        var delta2 = Vector3.zero;
        
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
            this.SetPixel(src.x, src.y);
            if ((counter += (int)s) >= l)
            {
                counter -= (int) l;
                src.x += delta1.x;
                src.y += delta1.y;
            }
            else
            {
                src.x += delta2.x;
                src.y += delta2.y;
            }
        }
    }

    protected override void Standard(Vector3 src, Vector3 dest)
    {
        //Unity have no standard method for draw on texture
    }
}