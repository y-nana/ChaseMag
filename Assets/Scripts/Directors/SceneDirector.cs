using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum StageLevelState
{
    easy,
    normal,
    hard,
    extra
}


public class SceneDirector : MonoBehaviour, SceneCaller
{
    private readonly string gameOver = "GameOverScene";     // ゲームオーバーシーン名
    private readonly string gameClear = "GameClearScene";   // ゲームクリアシーン名
    private readonly string gameScene = "GameScene";                // ゲームメインシーン名
    private readonly string stageSelectScene = "StageSelectScene";  // ステージセレクトシーン名
    private readonly string titleScene = "GameStartScene";          // タイトルシーン名
    private readonly string tutorialScene = "GameTutorialScene";    // チュートリアルシーン名
    
    private readonly string easyGameScene = "EasyGameScene";        // かんたんゲームシーン名
    private readonly string normalGameScene = "normalGameScene";    // ふつうゲームシーン名
    private readonly string hardGameScene = "hardGameScene";        // むずかしいゲームシーン名


    private readonly string sceneDirectorTag = "SceneDirector";     // オブジェクト取得用タグ


    [SerializeField]
    private StageLevelState thisStageLevel;


    private static StageLevelState NextStageLevel;


    private void Update()
    {
        // escapeキーでタイトルシーンへ
        if (Input.GetKey(KeyCode.Escape))
        {
            ToTitle();
        }
        //Debug.Log("this"+thisStageLevel);
        //Debug.Log("next"+NextStageLevel);
    }

    public void ToGameStart()
    {
        Time.timeScale = 1f;
        //Debug.Log(NextStageLevel);

        switch (NextStageLevel)
        {
            case StageLevelState.easy:
                SceneManager.LoadScene(easyGameScene);

                break;
            case StageLevelState.normal:
                SceneManager.LoadScene(normalGameScene);

                break;
            case StageLevelState.hard:
                SceneManager.LoadScene(hardGameScene);

                break;
            case StageLevelState.extra:
                SceneManager.LoadScene(gameScene);

                break;
        }
    }

    public void ToTitle()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(titleScene);
    }

    // 難易度によってシーンを変える
    public void ToGameStart(StageLevelState stageLevel)
    {
        Time.timeScale = 1f;
        NextStageLevel = stageLevel;

        switch (stageLevel)
        {
            case StageLevelState.easy:
                SceneManager.LoadScene(easyGameScene);

                break;
            case StageLevelState.normal:
                SceneManager.LoadScene(normalGameScene);

                break;
            case StageLevelState.hard:
                SceneManager.LoadScene(hardGameScene);

                break;
            case StageLevelState.extra:
                SceneManager.LoadScene(gameScene);

                break;
        }
    }

    public void ToStageSelect()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(stageSelectScene);
    }


    // ゲームオーバーシーンへ
    public void ToGameOver()
    {
        Time.timeScale = 1f;
        //SceneManager.sceneLoaded += SendToNextScene;
        NextStageLevel = thisStageLevel;

        SceneManager.LoadScene(gameOver);
    }
    // ゲームクリアシーンへ
    public void ToGameClear()
    {
        Time.timeScale = 1f;
        //SceneManager.sceneLoaded += SendToNextScene;
        NextStageLevel = thisStageLevel;
        SceneManager.LoadScene(gameClear);
    }

    // チュートリアルシーンへ
    public void ToTurorial()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(tutorialScene);
    }

    // 遷移時のイベント登録で変数を渡したいけどうまくいかないので没
    /*
    private void SendToNextScene(Scene next, LoadSceneMode mode)
    {
        Debug.Log(thisStageLevel);

        var nextSceneDirector =
            GameObject.FindWithTag(sceneDirectorTag)
            .GetComponent<SceneDirector>();

        nextSceneDirector.NextStageLevel = thisStageLevel;
        nextSceneDirector.test = testInt;
        Debug.Log(nextSceneDirector.NextStageLevel);

        SceneManager.sceneLoaded -= SendToNextScene;
    }
    */
}
