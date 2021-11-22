using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DrawCurve : DragAndDropDrawer
{
    private List<Vector2> _nodes = new List<Vector2>();
    private List<Vector2> _cache = new List<Vector2>();

    public override void OnToolEnabled()
    {
        _nodes.Clear();
        _cache.Clear();
    }

    protected override void DrawFigure(Vector3 start, Vector3 end, bool fill = false)
    {
        if(_nodes.Count == 0)
        {
            _nodes.Add(start);
        }

        //var points = (List<Vector2>)Core.GetBezier(0.05f, _nodes[count - 3], _nodes[count - 2], _nodes[count - 1], end);
        var temp = _nodes.ToList();
        temp.Add(end);
        var points = Core.GetBezier(temp);
        for (var i = 1; i < points.Count; ++i)
        {
            foreach (var point in Core.GetLine(points[i - 1], points[i]))
            {
                _cache.Add(point);
                this.SetPixel(point.x, point.y);
            }
        }

        //debug lines
        if (fill)
        {
            var last = _nodes.Last();
            foreach (var point in Core.GetLine(last, end))
            {
                _cache.Add(point);
                this.SetPixel(point.x, point.y);
            }
        }
    }

    protected override void OnFigureDrawn(Vector2 src, Vector2 end)
    {
        _nodes.Add(end);
    }
}
