using Assets.Scripts.Enum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 甜品基础脚本
/// </summary>
public class GameSweet : MonoBehaviour
{
    /// <summary>
    /// x坐标
    /// </summary>
    public int X;
    /// <summary>
    /// y坐标
    /// </summary>
    public int Y;
    /// <summary>
    /// 当前类型
    /// </summary>
    public SweetsType Type;
    [HideInInspector]
    public MoveSweet moveSweet;
    [HideInInspector]
    public ColorSweet colorSweet;
    [HideInInspector]
    public GameManager gameManager;
    [HideInInspector]
    public ClearSweet clearSweet;
    private void Awake()
    {
        moveSweet = GetComponent<MoveSweet>();
        colorSweet = GetComponent<ColorSweet>();
        clearSweet = GetComponent<ClearSweet>();
    }
    /// <summary>
    /// 初始化属性
    /// </summary>
    /// <param name="_x">x坐标</param>
    /// <param name="_y">y坐标</param>
    /// <param name="_gameManager"></param>
    /// <param name="_type">类型</param>
    public void Init(int _x, int _y, GameManager _gameManager, SweetsType _type)
    {
        X = _x;
        Y = _y;
        gameManager = _gameManager;
        Type = _type;
    }
    /// <summary>
    /// 是否可以移动
    /// </summary>
    /// <returns></returns>
    public bool CanMove()
    {
        return moveSweet != null;
    }
    /// <summary>
    /// 是否可更换精灵
    /// </summary>
    /// <returns></returns>
    public bool CanColor()
    {
        return colorSweet != null;
    }
    /// <summary>
    /// 是否可清除
    /// </summary>
    /// <returns></returns>
    public bool CanClear() {
        return clearSweet != null;
    }
    private void OnMouseEnter()
    {
        gameManager.EnterSweet(this);
    }

    private void OnMouseDown()
    {
        gameManager.PressSweet(this);
    }

    private void OnMouseUp()
    {
        gameManager.ReleaseSweet();
    }

}
