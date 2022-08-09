using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStartDirector : MonoBehaviour
{
    
    private readonly string mainScene = "GameScene";                // ゲームメインシーン名
    private readonly string titleScene = "GameStartScene";          // タイトルシーン名
    private readonly string tutorialScene = "GameTutorialScene";    // チュートリアルシーン名

    void Update()
    {
 
        // spaceキーでゲームメインシーンへ
        if (Input.GetKey(KeyCode.Space))
        {
            ToMain();
        }
        // escapeキーでタイトルシーンへ
        if (Input.GetKey(KeyCode.Escape))
        {
            ToTitle();
        }
    }

    public void ToMain()
    {
        SceneManager.LoadScene(mainScene);
    }

    public void ToTitle()
    {
        SceneManager.LoadScene(titleScene);
    }

    public void ToTutorial()
    {
        SceneManager.LoadScene(tutorialScene);
    }
}
