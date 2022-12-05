using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// タイトルシーンを制御するクラス
public class GameStartDirector : MonoBehaviour
{
    
    private readonly string stageSelectScene = "StageSelectScene";  // ステージセレクトシーン名
    private readonly string titleScene = "GameStartScene";          // タイトルシーン名



    void Update()
    {

        // spaceキーでステージセレクトシーンへ
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Start"))
        {
            ToStageSelect();
        }
        // escapeキーでタイトルシーンへ
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // すでにタイトルだったらゲームを終了する
            if (SceneManager.GetActiveScene().name == titleScene)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }
            ToTitle();
        }
    }

    public void ToStageSelect()
    {
        SceneManager.LoadScene(stageSelectScene);
    }

    public void ToTitle()
    {
        SceneManager.LoadScene(titleScene);
    }

}
