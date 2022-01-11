using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(DrawerManager))]
public abstract class Drawer : MonoBehaviour
{
    [SerializeField] public KeyCode toggleButton;
    
    private DrawerManager drawerManager;
    private Image image;
    
    protected Texture2D Texture;
    protected Vector3 Shift;
    protected Color32[] PointsCache;
    protected Color32[] PointsStash;
    private readonly HashSet<int> pointsBuffer = new HashSet<int>();


    public virtual void OnToolEnabled()
    {
    }
    
    protected virtual void OnFigureDrawn(Vector2 src, Vector2 end)
    {
    }
    
    protected virtual void OnClear()
    {
    }
    
    protected bool TryGetPixelFilled(float x, float y, out Color32 pixel)
    {
        pixel = default;

        var index = (int) (x + y * Texture.width);
        if (index > 0 && index < PointsCache.Length)
        {
            pixel = PointsCache[index];
            return !pointsBuffer.Contains(index);
        }

        return false;
    }
    
    protected void SetPixel(float x, float y)
    {
        var index = (int)x + (int)y * Texture.width;
        if (index > 0 && index < PointsCache.Length)
        {
            pointsBuffer.Add(index);
        }
    }
    
    protected void SetPixel(Vector2 point)
    {
        this.SetPixel(point.x, point.y);
    }

    protected void CacheBuffer(Color brushColor)
    {
        foreach (var point in pointsBuffer)
        {
            PointsStash[point] = PointsCache[point];
            PointsCache[point] = brushColor;
        }
    }
    
    protected void CacheBuffer(Color brushColor, IEnumerable<Vector2> pointBuffer)
    {
        foreach (var p in pointBuffer)
        {
            var point = (int) p.x + (int)p.y * Texture.width;
            pointsBuffer.Add(point);
            if (point > 0 && point < PointsCache.Length)
            {
                PointsCache[point] = brushColor;
            }
        }
    }
    
    protected void ApplyCache()
    {
        foreach (var point in pointsBuffer)
        {
            PointsStash[point] = PointsCache[point];
        }

        pointsBuffer.Clear();
    }

    protected void ApplyStash()
    {
        foreach (var point in pointsBuffer)
        {
            PointsCache[point] = PointsStash[point];
        }

        pointsBuffer.Clear();
    }

    protected void DiscardCache()
    {
        foreach (var point in pointsBuffer)
        {
            if (point > 0 && point < PointsCache.Length)
            {
                PointsCache[point] = Color.white;
            }
        }

        pointsBuffer.Clear();
    }
    
    protected void ApplyToTexture()
    {
        Texture.SetPixels32(0, 0, Texture.width, Texture.height, PointsCache);
        Texture.Apply();
    }

    protected virtual void Start()
    {
        drawerManager = GetComponent<DrawerManager>();
        
        image = GetComponent<Image>();
        var pivot = image.GetComponent<RectTransform>().pivot;
        Texture = (Texture2D) image.mainTexture;
        Shift = new Vector3(Texture.width * pivot.x, Texture.height * pivot.y);
        PointsCache = drawerManager.ColorsBuffer;
        PointsStash = drawerManager.StagedColors;
        enabled = false;

        if (!drawerManager.Tools.TryAdd(toggleButton.ToString(), this))
        {
            Debug.LogError($"Duplicate {nameof(toggleButton)} parameter in scene");
        }
    }
}
