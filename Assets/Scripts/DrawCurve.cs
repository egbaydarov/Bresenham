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
    
    private readonly List<Vector2> _nodes = new List<Vector2>();
    private static readonly Color EraserColor = Color.white;

    private List<Vector2> _cache = new List<Vector2>();
    private List<Vector2> _oldCache = new List<Vector2>();
    private Vector2 midpointCached;

    [SerializeField]
    private Task task = Task.OrderN;
    
    public override void OnToolEnabled()
    {
    }

    protected override void OnClearReceived()
    {
        midpointCached = Vector2.zero;
        _nodes.Clear();
    }

    protected override void DrawFigure(Vector3 start, Vector3 end, bool fill = false)
    {
        if(_nodes.Count == 0)
        {
            _nodes.Add(start);
        }
        var allNodes = _nodes.ToList();
        allNodes.Add(end);
        var points = GetCurvePoints(allNodes, fill);
        for (var i = 1; i < points.Count; ++i)
        {
            foreach (var point in Core.GetLine(points[i - 1], points[i]))
            {
                _cache.Add(point);
                this.SetPixel(point.x, point.y);
            }
        }
        EraseStage(_oldCache, EraserColor);
    }

    protected override void OnFigureDrawn(Vector2 src, Vector2 end)
    {
        if (midpointCached != Vector2.zero)
        {
            _nodes.Add(midpointCached);
            _nodes.Add(midpointCached);
        }
        _nodes.Add(end);
        
        _oldCache = _cache;
        _cache = new List<Vector2>();
    }

    private List<Vector2> GetCurvePoints(List<Vector2> input, bool fill)
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
                points = Core.GetComplexBezier(input, out midpointCached, fill);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(task), task, null);
        }
        
        return points;
    }
}
