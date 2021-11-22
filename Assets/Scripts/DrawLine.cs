using System;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : DragAndDropDrawer
{
    public List<Tuple<Vector2, Vector2>> Cache = new List<Tuple<Vector2, Vector2>>();
    
    protected override void DrawFigure(Vector3 start, Vector3 end, bool fill = false)
    {
        foreach(var point in Core.GetLine(start, end))
        {
            this.SetPixel(point.x, point.y);
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