using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(DrawerManager))]
public abstract class DragAndDropDrawer : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    [SerializeField] public KeyCode toggleButton;
    [SerializeField] private bool innerFilling;
    [SerializeField] protected Color32 brushColor = Color.black;

    private Image _image;
    private DrawerManager _drawerManager;
    private Vector3 _shift;

    private Vector3 _srcPoint;
    private Vector3 _currentPoint;
    private bool _pointerDown;

    private Color32[] _colorsBuffer;
    private Color32[] _stagedColors;
    private readonly HashSet<int> _pointsCache = new HashSet<int>();

    private Texture2D _texture;
    private PointerEventData _pData;

    public void OnPointerUp(PointerEventData eventData)
    {
        _pointerDown = false;
        var src = transform.InverseTransformPoint(_srcPoint + _shift);
        var dest = transform.InverseTransformPoint(_currentPoint + _shift);

        ApplyCache();
        ApplyToTexture();
        StageCache();
        OnFigureDrawn(src, dest);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _srcPoint = eventData.pointerCurrentRaycast.screenPosition;
        _pData = eventData;
        _pointerDown = true;
    }

    protected void SetPixel(float x, float y)
    {
        var index = (int) (x + y * _texture.width);
        if (index > 0 && index < _colorsBuffer.Length)
        {
            _pointsCache.Add(index);
        }
    }

    protected bool TryGetPixel(float x, float y, out Color32 pixel)
    {
        var index = (int) (x + y * _texture.width);
        if (index > 0 && index < _colorsBuffer.Length)
        {
            pixel = _colorsBuffer[index];
            return true;
        }

        pixel = default;
        return false;
    }

    protected bool TryGetPixelFilled(float x, float y, out Color32 pixel)
    {
        pixel = default;

        var index = (int) (x + y * _texture.width);
        if (index > 0 && index < _colorsBuffer.Length)
        {
            pixel = _colorsBuffer[index];
            return !_pointsCache.Contains(index);
        }

        return false;
    }

    protected abstract void DrawFigure(Vector3 start, Vector3 end, bool fill = false);

    public virtual void OnToolEnabled()
    {
    }

    protected virtual void OnFigureDrawn(Vector2 src, Vector2 end)
    {
    }

    private void Draw()
    {
        var src = transform.InverseTransformPoint(_srcPoint + _shift);
        var dest = transform.InverseTransformPoint(_currentPoint + _shift);

        this.DrawFigure(src, dest, innerFilling);
    }

    private void ApplyCache()
    {
        foreach (var point in _pointsCache)
        {
            _stagedColors[point] = _colorsBuffer[point];
            _colorsBuffer[point] = brushColor;
        }
    }

    private void StageCache()
    {
        foreach (var point in _pointsCache)
        {
            _stagedColors[point] = _colorsBuffer[point];
        }

        _pointsCache.Clear();
    }

    private void DiscardCache()
    {
        foreach (var point in _pointsCache)
        {
            _colorsBuffer[point] = _stagedColors[point];
        }

        _pointsCache.Clear();
    }

    private void ApplyToTexture()
    {
        _texture.SetPixels32(0, 0, _texture.width, _texture.height, _colorsBuffer);
        _texture.Apply();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            innerFilling = !innerFilling;
        }

        if (!_pointerDown)
        {
            return;
        }
        var newPoint = _pData.pointerCurrentRaycast.screenPosition;
        if (_currentPoint == (Vector3) newPoint)
        {
            return;
        }

        _currentPoint = newPoint;

        DiscardCache();
        ApplyToTexture();

        this.Draw();

        ApplyCache();
        ApplyToTexture();
    }

    private void Start()
    {
        _image = GetComponent<Image>();
        var pivot = _image.GetComponent<RectTransform>().pivot;
        _texture = (Texture2D) _image.mainTexture;
        _shift = new Vector3(_texture.width * pivot.x, _texture.height * pivot.y);
        _drawerManager = GetComponent<DrawerManager>();
        _colorsBuffer = _drawerManager.ColorsBuffer;
        _stagedColors = _drawerManager.StagedColors;

        if (!_drawerManager.Tools.TryAdd(toggleButton.ToString(), this))
        {
            Debug.LogError($"Duplicate {nameof(toggleButton)} parameter in scene");
        }

        enabled = false;
    }
}