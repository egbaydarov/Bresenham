using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class ClickPointDrawer : Drawer, IPointerClickHandler, IPointerDownHandler//, IPointerUpHandler
{
    protected List<Vector2> Points = new List<Vector2>();
    protected List<int> DotTypes = new List<int>();
    private const int PointWidth = 10;
    private PointerEventData _eData;

    [SerializeField] private Color32 zeroThird = new Color32(168, 127, 50, Byte.MaxValue);
    [SerializeField] private Color32 firstSecond = new Color32(50, 168, 129, Byte.MaxValue);
    [SerializeField] private Color32 brushColor = Color.black;
    [SerializeField] private Color32 brushColorAlt = Color.blue;
    
    private int[,] _pointsLocator;
    private bool _onPoint = false;
    private int _pointIndex;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (_onPoint)
        {
            _onPoint = false;
            return;
        }
        
        var srcPoint = (Vector3)eventData.pointerCurrentRaycast.screenPosition;
        var src = transform.InverseTransformPoint(srcPoint + Shift);
        Points.Add(new Vector2(src.x, src.y));
        DotTypes.Add(2);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _eData = eventData;
        var srcPoint = (Vector3)eventData.pointerCurrentRaycast.screenPosition;
        var src = transform.InverseTransformPoint(srcPoint + Shift);
        if (src.x > _pointsLocator.GetLength(0) || src.x < 0 || src.y >= _pointsLocator.GetLength(1) || src.y < 0)
        {
            return;
        }
        
        if (_pointsLocator[(int) src.x, (int) src.y] == -1)
        {
            return;
        }
        _onPoint = true;
        _pointIndex = _pointsLocator[(int) src.x, (int) src.y];
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!_onPoint)
        {
            return;
        }

        var destPoint = (Vector3)eventData.pointerCurrentRaycast.screenPosition;
        var dest = transform.InverseTransformPoint(destPoint + Shift);
        if (dest.x > _pointsLocator.GetLength(0) || dest.x < 0 || dest.y >= _pointsLocator.GetLength(1) || dest.y < 0)
        {
            return;
        }

        var point = Points[_pointIndex];
        var pointType = DotTypes[_pointIndex];
        switch (pointType)
        {
            case 0:
                var line = Core.GetLine(Points[_pointIndex - 1], Points[_pointIndex + 1]).ToList();
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
                Points[_pointIndex - 1] = Core.Midpoint(Points[_pointIndex - 2], dest);
                break;
            case -1:
                Points[_pointIndex + 1] = Core.Midpoint(Points[_pointIndex + 2], dest);
                break;
            case 2:
                break;
            case 3:
                break;
        }
        Points[_pointIndex] = dest;

        for (int x = (int) point.x - PointWidth; x < (point.x) + PointWidth + 1; ++x)
        {
            for (int y = (int) point.y - PointWidth; y < (point.y) + PointWidth + 1; ++y)
            {
                try
                {
                    _pointsLocator[x, y] = -1;

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
        OnPointerUp(_eData);
        DiscardCache();

        if (Input.GetKeyUp(KeyCode.C))
        {
            Points.Clear();
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
        _pointsLocator = new int[Texture.width, Texture.height];
        for (int i = 0; i < Texture.width; ++i)
        {
            for (int j = 0; j < Texture.height; ++j)
            {
                _pointsLocator[i, j] = -1;
            }
        }
    }
    
    private void Highlight()
    {
        for (var i = 0; i < Points.Count; ++i)
        {
            var buffer = Highlight(Points[i], i);

            if (i % 3 == 0)
            {
                CacheBuffer(this.zeroThird, buffer.Item1);
            }
            else
            {
                CacheBuffer(this.firstSecond, buffer.Item1);
            }

            Points = Points.Where((x, ind) => !buffer.Item2.Contains(ind)).ToList();
            DotTypes = DotTypes.Where((x, ind) => !buffer.Item2.Contains(ind)).ToList();
        }
    }

    private (List<Vector2>, List<int>) Highlight(Vector2 point, int index)
    {
        var border = new List<Vector2>();
        for (int x = (int) point.x - PointWidth; x < (point.x) + PointWidth + 1; ++x)
        {
            var p1 = new Vector2(x, point.y - PointWidth);
            var p2 = new Vector2(x, point.y + PointWidth);
            border.Add(p1);
            border.Add(p2);
        }
        
        for (int y = (int) point.y - PointWidth; y < (point.y) + PointWidth + 1; ++y)
        {
            var p1 = new Vector2(point.x - PointWidth, y);
            var p2 = new Vector2(point.x + PointWidth, y);
            border.Add(p1);
            border.Add(p2);
        }

        var remove = new List<int>();
        for (int x = (int) point.x - PointWidth; x < (point.x) + PointWidth + 1; ++x)
        {
            for (int y = (int) point.y - PointWidth; y < (point.y) + PointWidth + 1; ++y)
            {
                try
                {
                    _pointsLocator[x, y] = index;
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
