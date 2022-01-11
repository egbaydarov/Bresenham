using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class ClickPointDrawer : Drawer, IPointerClickHandler, IPointerDownHandler//, IPointerUpHandler
{
    protected List<Vector2> points = new List<Vector2>();
    protected List<int> dotTypes = new List<int>();
    private int pointWidth = 15;
    private PointerEventData eData;

    [SerializeField] private Color32 zeroThird = Color.green;
    [SerializeField] private Color32 firstSecond = Color.red;
    [SerializeField] private Color32 brushColor = Color.black;
    [SerializeField] private Color32 brushColorAlt = Color.blue;
    
    private int[,] pointsLocator;
    private bool OnPoint = false;
    private int PointIndex;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnPoint)
        {
            OnPoint = false;
            return;
        }
        
        var srcPoint = (Vector3)eventData.pointerCurrentRaycast.screenPosition;
        var src = transform.InverseTransformPoint(srcPoint + Shift);
        points.Add(new Vector2(src.x, src.y));
        dotTypes.Add(2);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        eData = eventData;
        var srcPoint = (Vector3)eventData.pointerCurrentRaycast.screenPosition;
        var src = transform.InverseTransformPoint(srcPoint + Shift);
        if (src.x > pointsLocator.GetLength(0) || src.x < 0 || src.y >= pointsLocator.GetLength(1) || src.y < 0)
        {
            return;
        }
        
        if (pointsLocator[(int) src.x, (int) src.y] == -1)
        {
            return;
        }
        OnPoint = true;
        PointIndex = pointsLocator[(int) src.x, (int) src.y];
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!OnPoint)
        {
            return;
        }

        var destPoint = (Vector3)eventData.pointerCurrentRaycast.screenPosition;
        var dest = transform.InverseTransformPoint(destPoint + Shift);
        if (dest.x > pointsLocator.GetLength(0) || dest.x < 0 || dest.y >= pointsLocator.GetLength(1) || dest.y < 0)
        {
            return;
        }

        var point = points[PointIndex];
        var pointType = dotTypes[PointIndex];
        switch (pointType)
        {
            case 0:
                var line = Core.GetLine(points[PointIndex - 1], points[PointIndex + 1]).ToList();
                var closest = float.MaxValue;
                var closestPt = line[0];
                foreach (var linePoint in line)
                {
                    var newDistance = Vector2.Distance(linePoint, dest);
                    if (newDistance < closest)
                    {
                        closest = newDistance;
                        closestPt = linePoint;
                    }
                }

                dest = closestPt;
                break;
            case 1:
                points[PointIndex - 1] = Core.Midpoint(points[PointIndex - 2], dest);
                break;
            case -1:
                points[PointIndex + 1] = Core.Midpoint(points[PointIndex + 2], dest);
                break;
            case 2:
                break;
            case 3:
                break;
        }
        points[PointIndex] = dest;

        for (int x = (int) point.x - pointWidth; x < (point.x) + pointWidth + 1; ++x)
        {
            for (int y = (int) point.y - pointWidth; y < (point.y) + pointWidth + 1; ++y)
            {
                try
                {
                    pointsLocator[x, y] = -1;

                }
                catch (Exception e)
                {
                    Debug.Log(e);
                }
            }
        }
    }

    protected abstract List<Vector2> Draw();
    protected abstract List<Vector2> DrawAlt();

    private void SetPixels(List<Vector2> pts)
    {
        foreach (var pixels in pts)
        {
            this.SetPixel(pixels);
        }
    }
    
    protected virtual void Update()
    {
        OnPointerUp(eData);
        DiscardCache();

        if (Input.GetKeyUp(KeyCode.C))
        {
            points.Clear();
        }

        var pts = Draw();
        var ptsAlt = DrawAlt();
        this.SetPixels(pts);
        this.SetPixels(ptsAlt);
        CacheBuffer(this.brushColor, pts);
        CacheBuffer(this.brushColorAlt, ptsAlt);
        Highlight();
        ApplyToTexture();
    }

    protected override void Start()
    {
        base.Start();
        pointsLocator = new int[Texture.width, Texture.height];
        for (int i = 0; i < Texture.width; ++i)
        {
            for (int j = 0; j < Texture.height; ++j)
            {
                pointsLocator[i, j] = -1;
            }
        }
    }
    
    private void Highlight()
    {
        for (var i = 0; i < points.Count; ++i)
        {
            var buffer = Highlight(points[i], i);

            if (i % 3 == 0)
            {
                CacheBuffer(this.zeroThird, buffer.Item1);
            }
            else
            {
                CacheBuffer(this.firstSecond, buffer.Item1);
            }

            points = points.Where((x, ind) => !buffer.Item2.Contains(ind)).ToList();
            dotTypes = dotTypes.Where((x, ind) => !buffer.Item2.Contains(ind)).ToList();
        }
    }

    private (List<Vector2>, List<int>) Highlight(Vector2 point, int index)
    {
        var border = new List<Vector2>();
        for (int x = (int) point.x - pointWidth; x < (point.x) + pointWidth + 1; ++x)
        {
            var p1 = new Vector2(x, point.y - pointWidth);
            var p2 = new Vector2(x, point.y + pointWidth);
            border.Add(p1);
            border.Add(p2);
        }
        
        for (int y = (int) point.y - pointWidth; y < (point.y) + pointWidth + 1; ++y)
        {
            var p1 = new Vector2(point.x - pointWidth, y);
            var p2 = new Vector2(point.x + pointWidth, y);
            border.Add(p1);
            border.Add(p2);
        }

        var remove = new List<int>();
        for (int x = (int) point.x - pointWidth; x < (point.x) + pointWidth + 1; ++x)
        {
            for (int y = (int) point.y - pointWidth; y < (point.y) + pointWidth + 1; ++y)
            {
                try
                {
                    pointsLocator[x, y] = index;
                }
                catch
                {
                    remove.Add(index);
                }
            }
        }

        return (border,remove);
    }
}
