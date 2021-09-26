using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class BresenhamDrawer : MonoBehaviour
{
    [SerializeField] private Image image;
    private Vector3? _srcPoint;
    private Vector3? _destPoint;
    private const int LineWidth = 3;
    private Color32[] _colors;
    private bool[] _hardColors;
    private readonly Color32 _backgroundColor = Color.white;
    private readonly Color32 _brushColor = Color.black;
    private Texture2D _texture;

    private PointerEventData _pData;
    private readonly List<int> _pointsCache = new List<int>();

    private bool _pointerFlag;

    public void PointerDown(BaseEventData eventData)
    {
        var pData = eventData as PointerEventData;
        _srcPoint = pData?.pointerCurrentRaycast.screenPosition;
        _pData = pData;
        _pointerFlag = true;
    }

    public void PointerUp(BaseEventData eventData)
    {
        _pointerFlag = false;

        var pData = eventData as PointerEventData;
        _destPoint = pData?.pointerCurrentRaycast.screenPosition;
        ApplyCache(true);

        this.RenderFigure();
    }

    protected void SetPixel(float x, float y)
    {
        var index = (int) (x + y * _texture.width);
        if (index > 0 && index < _colors.Length)
        {
            _pointsCache.Add(index);
        }
    }

    private void Apply()
    {
        _texture.SetPixels32(0, 0, _texture.width, _texture.height, _colors);
        _texture.Apply();
    }

    protected abstract void Bresenham(Vector3 src, Vector3 dest);

    protected abstract void Standard(Vector3 src, Vector3 dest);

    private void RenderFigure()
    {
        if (_srcPoint.HasValue && _destPoint.HasValue)
        {
            var pivot = image.GetComponent<RectTransform>().pivot;
            var shift = new Vector3(_texture.width * pivot.x, _texture.height * pivot.y);
            var src = transform.InverseTransformPoint(_srcPoint.Value);
            var dest = transform.InverseTransformPoint(_destPoint.Value);

            this.Bresenham(src, dest);
            this.Standard(src, dest);
        }
    }

    private void ApplyCache(bool hard = false)
    {
        foreach (var point in _pointsCache)
        {
            _colors[point] = _brushColor;
        }
        Apply();
        
        if (hard)
        {
            foreach (var point in _pointsCache)
            {
                _hardColors[point] = true;
            }
            _pointsCache.Clear();
        }
    }

    private void DiscardCache()
    {
        foreach (var point in _pointsCache)
        {
            if (!_hardColors[point])
            {
                _colors[point] = _backgroundColor;
            }
        }
        _pointsCache.Clear();
        Apply();
    }

    private void Awake()
    {
        if (image == null)
        {
            enabled = false;
            Debug.Log("Null image component. Disabling");
        }
    }

    private void Start()
    {
        var imageSize = image.transform.parent.GetComponent<RectTransform>().sizeDelta;
        _texture = new Texture2D((int) imageSize.x, (int) imageSize.y, TextureFormat.ARGB32, false);
        var rect = new Rect(0, 0, _texture.width, _texture.height);
        var pivot = Vector2.zero;
        _colors = new Color32[(int) imageSize.x * (int) imageSize.y];
        _hardColors = new bool[(int) imageSize.x * (int) imageSize.y];

        image.sprite = Sprite.Create(_texture, rect, pivot);

        for (int i = 0; i < _colors.Length; ++i)
        {
            _colors[i] = _backgroundColor;
        }

        this.Apply();
    }

    private void Update()
    {
        if (!_pointerFlag) return;
        
        DiscardCache();
        _destPoint = _pData?.pointerCurrentRaycast.screenPosition;
        this.RenderFigure();
        ApplyCache();
    }
}