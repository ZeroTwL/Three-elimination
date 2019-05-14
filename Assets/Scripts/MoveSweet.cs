using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 移动脚本
/// </summary>
public class MoveSweet : MonoBehaviour
{
    private GameSweet sweet;
    /// <summary>
    /// 协程
    /// </summary>
    private IEnumerator moveCoroutione;
    private void Awake()
    {
        sweet = GetComponent<GameSweet>();
    }
    /// <summary>
    /// 移动
    /// </summary>
    /// <param name="newX">x坐标</param>
    /// <param name="newY">y坐标</param>
    public void Move(int newX, int newY,float time)
    {
        if (moveCoroutione!=null)
        {
            StopCoroutine(moveCoroutione);
        }
        moveCoroutione = MoveCoroutine(newX,newY,time);
        StartCoroutine(moveCoroutione);
    }
    /// <summary>
    /// 使用协程移动甜品
    /// </summary>
    /// <param name="newX"></param>
    /// <param name="newY"></param>
    /// <param name="time">到达指定位置的时间</param>
    /// <returns></returns>
    IEnumerator MoveCoroutine(int newX,int newY,float time) {
        sweet.X = newX;
        sweet.Y = newY;
        //每一帧移动一点位置
        Vector3 startPos = transform.position;
        Vector3 endPos = sweet.gameManager.CorrectPos(newX,newY);
        //循环移动
        for (float t = 0; t < time; t+=Time.deltaTime)
        {
            sweet.transform.position = Vector3.Lerp(startPos, endPos, t / time);
            yield return 0;
        }
        //预防为移动到指定位置
        sweet.transform.position = endPos;
    }
}
