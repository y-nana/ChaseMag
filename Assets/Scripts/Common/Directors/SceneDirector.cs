﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


// ステージの難易度（種類）
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

    [SerializeField]
    private StageLevelState thisStageLevel;     // unityエディター上で設定するこのシーンはどのステージなのか

    public static StageLevelState NextStageLevel { get; set; }      // 遷移時にセットする（ボタン選択用）

    // チュートリアルのときはシーン遷移するタイミングで遷移せずに指定の処理を行う
    public System.Action tutorialClearAct { get; set; }
    public System.Action tutorialOverAct { get; set; }

    // テスト用 エディター上のみ
#if UNITY_EDITOR
    [SerializeField]
    private bool isTest;    // このシーンはテストですか
    private readonly string test = "TestScene";        // テストシーン名

#endif


    private void Update()
    {

        // escapeキーでタイトルシーンへ
        if (Input.GetKey(KeyCode.Escape))
        {

#if UNITY_EDITOR
            // テストのときは遷移せずリロード
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
    }


    // タイトルへ
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


    // ポーズ、リザルトから呼び出される（やりなおす、もう一度やる）
    public void ToGameStart()
    {
        ToGameStart(NextStageLevel);
    }

    // ステージセレクトへ
    public void ToStageSelect()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(stageSelectScene);
    }


    // ゲームオーバーシーンへ
    public void ToGameOver()
    {
#if UNITY_EDITOR
        // テスト時は捕まっても遷移しない
        if (isTest)
        {
            Debug.Log("捕まりました");

        }
        else
        {
            Time.timeScale = 1f;
            //SceneManager.sceneLoaded += SendToNextScene;
            NextStageLevel = thisStageLevel;
            // チュートリアルのときは遷移せず指定の処理を行う
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
        // チュートリアルのときは遷移せず指定の処理を行う
        if (thisStageLevel == StageLevelState.tutorial)
        {
            tutorialClearAct?.Invoke();
            return;
        }

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
