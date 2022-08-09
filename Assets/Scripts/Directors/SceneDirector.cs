using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class SceneDirector : MonoBehaviour, SceneCaller
{
    private readonly string gameOver = "GameOverScene";     // ゲームオーバーシーン名
    private readonly string gameClear = "GameClearScene";   // ゲームクリアシーン名
    private readonly string mainScene = "GameScene";                // ゲームメインシーン名
    private readonly string titleScene = "GameStartScene";          // タイトルシーン名



    private void Update()
    {
        // escapeキーでタイトルシーンへ
        if (Input.GetKey(KeyCode.Escape))
        {
            ToTitle();
        }
    }

    public void ToMain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainScene);
    }

    public void ToTitle()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(titleScene);
    }



    // ゲームオーバーシーンへ
    public void ToGameOver()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameOver);
    }
    // ゲームクリアシーンへ
    public void ToGameClear()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameClear);
    }
}
