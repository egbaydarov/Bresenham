using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DrawCurve : DragAndDropDrawer
{
    private enum Task
    {
        OrderN,
        Order3,
        Complex,
    }
    
    private List<Vector2> _nodes = new List<Vector2>();
    private List<Vector2> _cache = new List<Vector2>();
    private HashSet<Vector2> _oldCache = new HashSet<Vector2>();
    private Color _eraserColor = Color.white;

    public override void OnToolEnabled()
    {
    }

    protected override void DrawFigure(Vector3 start, Vector3 end, bool fill = false)
    {
        if(_nodes.Count == 0)
        {
            _nodes.Add(start);
        }
        var allNodes = _nodes.ToList();
        allNodes.Add(end);
        var points = GetCurvePoints(allNodes, Task.OrderN);
        for (var i = 1; i < points.Count; ++i)
        {
            foreach (var point in Core.GetLine(points[i - 1], points[i]))
            {
                _cache.Add(point);
                this.SetPixel(point.x, point.y);
            }
        }
        EraseStage(_oldCache, _eraserColor);
    }

    protected override void OnFigureDrawn(Vector2 src, Vector2 end)
    {
        _nodes.Add(end);
        // var points = GetCurvePoints(_nodes, Task.Order3);
        // for (var i = 1; i < points.Count; ++i)
        // {
        //     foreach (var point in Core.GetLine(points[i - 1], points[i]))
        //     {
        //         _cache.Add(point);
        //         _oldCache.Remove(point);
        //         this.SetPixel(point.x, point.y);
        //     }
        // }
        // EraseStage(_oldCache, _eraserColor);
        _oldCache = new HashSet<Vector2>(_cache);
        _cache = new List<Vector2>();
    }

    private List<Vector2> GetCurvePoints(List<Vector2> input, Task task)
    {
        var count = input.Count;
        List<Vector2> points = new List<Vector2>();
        switch (task)
        {
            case Task.OrderN:
                points = Core.GetBezier(input);
                break;
            case Task.Order3:
                if (count > 3)     
                {
                    points = Core.GetBezier(
                        input[count - 4], 
                        input[count - 3], 
                        input[count - 2],
                        input[count - 1]);
                }
                break;
            case Task.Complex:
                points = Core.GetComplexBezier(input);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(task), task, null);
        }
        
        return points;
    }
}
