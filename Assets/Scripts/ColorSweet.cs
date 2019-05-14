using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Enum;
using Sirenix.OdinInspector;

public class ColorSweet : SerializedMonoBehaviour
{
    /// <summary>
    /// 精灵字典
    /// </summary>
    public Dictionary<ColorType, Sprite> spriteDic = new Dictionary<ColorType, Sprite>();
    /// <summary>
    /// 精灵渲染器
    /// </summary>
    private SpriteRenderer sprite;
    [HideInInspector]
    public ColorType colorType;
    private void Awake()
    {
        sprite = gameObject.transform.Find("Sweet").GetComponent<SpriteRenderer>();
    }
    /// <summary>
    /// 设置精灵
    /// </summary>
    /// <param name="newColor">精灵类型</param>
    public void SetSprite(ColorType newColor)
    {
        colorType = newColor;
        if (spriteDic.ContainsKey(newColor))
        {
            sprite.sprite = spriteDic[newColor];
        }
    }
}
