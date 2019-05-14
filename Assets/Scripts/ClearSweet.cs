using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearSweet : MonoBehaviour
{
    public AnimationClip animClip;
    public AudioClip destorAudio;
    public bool IsClearing { get; set; }
    protected GameSweet sweet;
     void Awake()
    {
        sweet = GetComponent<GameSweet>();
    }
    /// <summary>
    /// 清楚当前物体
    /// </summary>
    public virtual void Clear()
    {
        IsClearing = true;
        StartCoroutine(ClearCoroutine());
    }
    IEnumerator ClearCoroutine()
    {
        Animator animation = GetComponent<Animator>();
        if (animation != null)
        {
            animation.Play(animClip.name);
            //得分加1并播放声音
            GameUI.Instance.score++;
            AudioSource.PlayClipAtPoint(destorAudio,Camera.main.transform.position,10);
            yield return new WaitForSeconds(animClip.length);
            Destroy(gameObject);
        }
    }
}
