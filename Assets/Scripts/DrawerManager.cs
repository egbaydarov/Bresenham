using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DrawerManager : MonoBehaviour
{
    private readonly Color32 _initColor = Color.white;

    public readonly ConcurrentDictionary<string, DragAndDropDrawer> Tools =
        new ConcurrentDictionary<string, DragAndDropDrawer>();
    public Color32[] ColorsBuffer { get; private set; }
    public Color32[] StagedColors { get; private set; }

    private void Update()
    {
        var cache = new List<DragAndDropDrawer>();
        var selected = false;
        foreach (var tool in Tools)
        {
            if (Input.GetKeyDown((KeyCode)Enum.Parse(typeof(KeyCode), tool.Key)) && !selected)
            {
                selected = true;
                tool.Value.OnToolEnabled();
                tool.Value.enabled = true;
                
                foreach (var cached in cache)
                {
                    cached.enabled = false;
                }
            }
            else if (selected)
            {
                tool.Value.enabled = false;
            }
            else
            {
                cache.Add(tool.Value);
            }
        }
    }

    private void Awake()
    {
        var image = GetComponent<Image>();
        
        var imageSize = image.transform.parent.GetComponent<RectTransform>().sizeDelta;
        var texture = new Texture2D((int) imageSize.x, (int) imageSize.y, TextureFormat.ARGB32, false);

        var rect = new Rect(0, 0, texture.width, texture.height);
        var pivot = Vector2.zero;
        ColorsBuffer = texture.GetPixels32();

        image.sprite = Sprite.Create(texture, rect, pivot);

        for (int i = 0; i < ColorsBuffer.Length; ++i)
        {
            ColorsBuffer[i] = _initColor;
        }
        
        StagedColors = new Color32[ColorsBuffer.Length];
        Array.Copy(ColorsBuffer, StagedColors, ColorsBuffer.Length);

        texture.SetPixels32(0, 0, texture.width, texture.height, ColorsBuffer);
        texture.Apply();
    }
}