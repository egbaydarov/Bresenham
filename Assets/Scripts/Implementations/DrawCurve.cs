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
        BSplines3,
        BSplinesComplex
    }
    
    private readonly List<Vector2> nodes = new List<Vector2>();
    private static readonly Color EraserColor = Color.white;

    private List<Vector2> cache = new List<Vector2>();
    private List<Vector2> oldCache = new List<Vector2>();
    private Vector2 midpointCached;

    [SerializeField]
    private Task task = Task.OrderN;
    
    public override void OnToolEnabled()
    {
    }

    protected override void OnClear()
    {
        midpointCached = Vector2.zero;
        nodes.Clear();
    }

    protected override void DrawFigure(Vector3 start, Vector3 end, bool fill = false)
    {
        if(nodes.Count == 0)
        {
            nodes.Add(start);
        }
        var allNodes = nodes.ToList();
        allNodes.Add(end);
        var points = GetCurvePoints(allNodes, fill);
        for (var i = 1; i < points.Count; ++i)
        {
            foreach (var point in Core.GetLine(points[i - 1], points[i]))
            {
                cache.Add(point);
                this.SetPixel(point.x, point.y);
            }
        }
        EraseStage(oldCache, EraserColor);
    }
    
    protected override void OnFigureDrawn(Vector2 src, Vector2 end)
    {
        if (midpointCached != Vector2.zero)
        {
            nodes.Add(midpointCached);
            nodes.Add(midpointCached);
        }
        nodes.Add(end);
        
        oldCache = cache;
        cache = new List<Vector2>();
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
            case Task.BSplines3:
                if (count > 3)
                {
                    points = Core.BSplines(
                        input[count - 4],
                        input[count - 3],
                        input[count - 2],
                        input[count - 1]);
                }
                break;
            case Task.BSplinesComplex:
                points = Core.GetComplexBSpline(input, out midpointCached, fill);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(task), task, null);
        }
        
        return points;
    }
}
