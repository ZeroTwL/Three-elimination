using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    /// <summary>
    /// 游戏时间显示
    /// </summary>
    public Text timeText;
    /// <summary>
    /// 游戏时间
    /// </summary>
    public float time;
    /// <summary>
    /// 得分
    /// </summary>
    public int score;
    public Text scoreText;
    /// <summary>
    /// 结束面板
    /// </summary>
    public GameObject panelGameOver;
    public Text TextFinalScore;
    [HideInInspector]
    public bool gameover;
    public static GameUI Instance { get; set; }
    void Awake()
    {
        Instance = this;
    }
    void Update()
    {
        time -= Time.deltaTime;
        if (time <= 0)
        {
            time = 0;
            gameover = true;
            panelGameOver.SetActive(true);
            TextFinalScore.text = score.ToString();
        }
        timeText.text = time.ToString("0");
        scoreText.text = score.ToString();
    }
    /// <summary>
    /// 返回主菜单
    /// </summary>
    public void ReturnToMian()
    {
        SceneManager.LoadScene(0);
    }
    /// <summary>
    /// 重玩
    /// </summary>
    public void Replay()
    {
        SceneManager.LoadScene(1);
    }
}
