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
    [SerializeField] private bool altFlag = false;
    [SerializeField] private int order = 4;

    public int Order
    {
        get => order;
        set
        {
            if (value < 1)
            {
                order = 1;
            }
            else if (value > Points.Count)
            {
                order = Points.Count - 1;
            }
            else
            {
                order = value;
            }
        }
    }


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
        if (!altFlag)
        {
            return result;
        }

        var p = task == Task.BSplinesComplex || task == Task.OrderN ? Core.GetSplinesDeBoor(Points, order) : Core.GetBezier(Points);
        for (var i = 1; i < p.Count; ++i)
        {
            result.AddRange(Core.GetLine(p[i - 1], p[i]));
        }
        return result;
    }

    private List<Vector2> GetCurvePoints()
    {
        var count = Points.Count;
        List<Vector2> p = new List<Vector2>();
        switch (task)
        {
            case Task.OrderN:
                p = Core.GetBezier(Points);
                break;
            case Task.Order3:
                if (count > 3)
                {
                    DotTypes[count - 4] = 2;
                    DotTypes[count - 3] = 2;
                    DotTypes[count - 2] = 2;
                    DotTypes[count - 1] = 2;
                    p = Core.GetBezier(
                        Points[count - 4], 
                        Points[count - 3], 
                        Points[count - 2],
                        Points[count - 1]);
                }

                while (Points.Count > 4)
                {
                    Points.RemoveAt(0);
                    DotTypes.RemoveAt(0);
                }
                break;
            case Task.Complex:
                p = Core.GetComplexBezier(Points, DotTypes, loop);
                break;
            case Task.BSplines3:
                if (count > 3)
                {
                    DotTypes[count - 4] = 2;
                    DotTypes[count - 3] = 2;
                    DotTypes[count - 2] = 2;
                    DotTypes[count - 1] = 2;
                    p = Core.BSplines(
                        Points[count - 4], 
                        Points[count - 3], 
                        Points[count - 2],
                        Points[count - 1]);
                    while (Points.Count > 4)
                    {
                        Points.RemoveAt(0);
                        DotTypes.RemoveAt(0);
                    }
                }
                break;
            case Task.BSplinesComplex:
                p = Core.GetComplexBSpline(Points, loop);
                break;
            case Task.BSplinesDeBoor:
                p = Core.GetSplinesDeBoor(Points, order);
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
            altFlag = !altFlag;
        }
        
        if (Input.GetKeyUp(KeyCode.L))
        {
            loop = !loop;
        }
        
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            ++Order;
        }
        
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            --Order;
        }
        
        base.Update();
    }
}