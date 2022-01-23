using System;
using System.Collections.Generic;
using UnityEngine;

public class DrawCurveV2 : ClickPointDrawer
{
    private enum Task
    {
        OrderN,
        Order3,
        Complex,
        BSplines3,
        BSplinesComplex,
        BSplinesDeBoor
    }
    
    [SerializeField]
    private Task task = Task.OrderN;

    [SerializeField] private bool loop = false;
    [SerializeField] private bool AltFlag = false;
    [SerializeField] private int Order = 4;

    
    protected override List<Vector2> Draw()
    {
        var result = new List<Vector2>();
        var p = GetCurvePoints();
        for (var i = 1; i < p.Count; ++i)
        {
            result.AddRange(Core.GetLine(p[i - 1], p[i]));
        }

        return result;
    }

    protected override List<Vector2> DrawAlt()
    {
        var result = new List<Vector2>();
        if (AltFlag)
        {
            var p = Core.GetBezier(points);
            for (var i = 1; i < p.Count; ++i)
            {
                result.AddRange(Core.GetLine(p[i - 1], p[i]));
            }
        }
        return result;
    }

    private List<Vector2> GetCurvePoints()
    {
        var count = points.Count;
        List<Vector2> p = new List<Vector2>();
        switch (task)
        {
            case Task.OrderN:
                p = Core.GetBezier(points);
                break;
            case Task.Order3:
                if (count > 3)
                {
                    dotTypes[count - 4] = 2;
                    dotTypes[count - 3] = 2;
                    dotTypes[count - 2] = 2;
                    dotTypes[count - 1] = 2;
                    p = Core.GetBezier(
                        points[count - 4], 
                        points[count - 3], 
                        points[count - 2],
                        points[count - 1]);
                }

                while (points.Count > 4)
                {
                    points.RemoveAt(0);
                    dotTypes.RemoveAt(0);
                }
                break;
            case Task.Complex:
                p = Core.GetComplexBezier(points, dotTypes, loop);
                break;
            case Task.BSplines3:
                if (count > 3)
                {
                    dotTypes[count - 4] = 2;
                    dotTypes[count - 3] = 2;
                    dotTypes[count - 2] = 2;
                    dotTypes[count - 1] = 2;
                    p = Core.BSplines(
                        points[count - 4], 
                        points[count - 3], 
                        points[count - 2],
                        points[count - 1]);
                    while (points.Count > 4)
                    {
                        points.RemoveAt(0);
                        dotTypes.RemoveAt(0);
                    }
                }
                break;
            case Task.BSplinesComplex:
                p = Core.GetComplexBSpline(points, loop);
                break;
            case Task.BSplinesDeBoor:
                p = Core.GetSplinesDeBoor(points, Order);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(task), task, null);
        }
        
        return p;
    }

    protected override void Update()
    {
        if (Input.GetKeyUp(KeyCode.A))
        {
            AltFlag = !AltFlag;
        }
        
        if (Input.GetKeyUp(KeyCode.L))
        {
            loop = !loop;
        }
        
        base.Update();
    }
}