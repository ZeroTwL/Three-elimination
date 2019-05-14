using Assets.Scripts.Enum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearColorSweet : ClearSweet
{
    public ColorType ClearColor;
    public override void Clear()
    {
        base.Clear();
        sweet.gameManager.ClearColor(ClearColor);
    }
}
