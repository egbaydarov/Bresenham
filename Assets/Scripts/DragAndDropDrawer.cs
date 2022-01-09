using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class DragAndDropDrawer : Drawer, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField] private bool innerFilling;
    [SerializeField] protected Color32 brushColor = Color.black;
    
    private Vector3 srcPoint;
    private Vector3 currentPoint;
    private bool pointerDown;
    
    private PointerEventData pData;

    public void OnPointerUp(PointerEventData eventData)
    {
        pointerDown = false;
        var src = transform.InverseTransformPoint(srcPoint + Shift);
        var dest = transform.InverseTransformPoint(currentPoint + Shift);
        CacheBuffer(this.brushColor);
        ApplyToTexture();
        ApplyCache();
        OnFigureDrawn(src, dest);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        srcPoint = eventData.pointerCurrentRaycast.screenPosition;
        pData = eventData;
        pointerDown = true;
    }

    protected void EraseStage(IEnumerable<Vector2> points, Color32 eraserColor)
    {
        foreach (var pt in points)
        {
            var point =  (int)pt.x + (int)pt.y * Texture.width;
            PointsCache[point] = eraserColor;
            PointsStash[point] = eraserColor;
        }
        ApplyToTexture();
    }

    protected bool TryGetPixel(float x, float y, out Color32 pixel)
    {
        var index = (int) (x + y * Texture.width);
        if (index > 0 && index < PointsCache.Length)
        {
            pixel = PointsCache[index];
            return true;
        }

        pixel = default;
        return false;
    }

    protected abstract void DrawFigure(Vector3 start, Vector3 end, bool fill = false);

    private void Draw()
    {
        var src = transform.InverseTransformPoint(srcPoint + Shift);
        var dest = transform.InverseTransformPoint(currentPoint + Shift);

        this.DrawFigure(src, dest, innerFilling);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            innerFilling = !innerFilling;
        }
        
        if (Input.GetKeyDown(KeyCode.Z))
        {
            OnClear();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            brushColor = Random.ColorHSV(0f, 1f,
                0.9f, 1f,
                0.9f, 1f, 
                1, 1);
        }

        if (!pointerDown)
        {
            return;
        }
        var newPoint = pData.pointerCurrentRaycast.screenPosition;
        if (currentPoint == (Vector3) newPoint)
        {
            return;
        }

        currentPoint = newPoint;

        ApplyStash();
        ApplyToTexture();

        this.Draw();

        CacheBuffer(this.brushColor);
        ApplyToTexture();
    }
}