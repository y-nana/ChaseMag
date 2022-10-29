﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum StageLevelState
{
    easy,
    normal,
    hard,
    extra,
    tutorial
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

    public static StageLevelState NextStageLevel { get; set; }

    public System.Action tutorialClearAct { get; set; }
    public System.Action tutorialOverAct { get; set; }

    // テスト用
#if UNITY_EDITOR
    [SerializeField]
    private bool isTest;
    private readonly string test = "TestScene";        // むずかしいゲームシーン名

#endif



    private void Update()
    {

        // escapeキーでタイトルシーンへ
        if (Input.GetKey(KeyCode.Escape))
        {

#if UNITY_EDITOR
            if (isTest)
            {
                SceneManager.LoadScene(test);

            }
            else
            {
                ToTitle();

            }
#else
            ToTitle();

#endif


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
            case StageLevelState.tutorial:
                SceneManager.LoadScene(tutorialScene);
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
            case StageLevelState.tutorial:
                SceneManager.LoadScene(tutorialScene);
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
#if UNITY_EDITOR
        if (isTest)
        {
            Debug.Log("捕まりました");

        }
        else
        {
            Time.timeScale = 1f;
            //SceneManager.sceneLoaded += SendToNextScene;
            NextStageLevel = thisStageLevel;
            if (thisStageLevel == StageLevelState.tutorial)
            {
                tutorialOverAct?.Invoke();
                return;
            }
            SceneManager.LoadScene(gameOver);

        }
#else
        Time.timeScale = 1f;
        //SceneManager.sceneLoaded += SendToNextScene;
        NextStageLevel = thisStageLevel;
        if (thisStageLevel== StageLevelState.tutorial)
        {
            tutorialOverAct?.Invoke();
            return;
        }
        SceneManager.LoadScene(gameOver);

#endif


    }
    // ゲームクリアシーンへ
    public void ToGameClear()
    {
        Time.timeScale = 1f;
        //SceneManager.sceneLoaded += SendToNextScene;
        NextStageLevel = thisStageLevel;
        if (thisStageLevel == StageLevelState.tutorial)
        {
            tutorialClearAct?.Invoke();
            return;
        }
        SceneManager.LoadScene(gameClear);
    }

    // チュートリアルシーンへ いらなくなる予定
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
