using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadGame : MonoBehaviour
{
    /// <summary>
    /// 加载场景
    /// </summary>
    public void LoadTheGame()
    {
        SceneManager.LoadScene(1);
    }
    /// <summary>
    /// 退出游戏
    /// </summary>
    public void ExitGame()
    {
        Application.Quit();
    }
}
