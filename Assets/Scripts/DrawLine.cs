using System;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : DragAndDropDrawer
{
    public List<Tuple<Vector2, Vector2>> Cache = new List<Tuple<Vector2, Vector2>>();
    
    protected override void DrawFigure(Vector3 start, Vector3 end, bool fill = false)
    {
        var size = new Vector2(end.x - start.x, end.y - start.y);
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
            this.SetPixel(start.x, start.y);
            if ((counter += (int)s) >= l)
            {
                counter -= (int) l;
                start.x += delta1.x;
                start.y += delta1.y;
            }
            else
            {
                start.x += delta2.x;
                start.y += delta2.y;
            }
        }
    }

    public override void OnToolEnabled()
    {
        Cache.Clear();
    }

    protected override void OnFigureDrawn(Vector2 src, Vector2 end)
    {
        Cache.Add(new Tuple<Vector2, Vector2>(src, end));
    }
}