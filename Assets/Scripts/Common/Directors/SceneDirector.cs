using System.Collections;
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
    W2_Easy,
    W2_Normal,
    W2_Hard,
    W2_Extra,
    tutorial,
    W3_Easy,
    W3_Normal,
    W3_Hard,
    W3_Extra
}


public class SceneDirector : MonoBehaviour
{
    private readonly string gameOver = "GameOverScene";     // ゲームオーバーシーン名
    private readonly string gameClear = "GameClearScene";   // ゲームクリアシーン名
    private readonly string stageSelectScene = "StageSelectScene";  // ステージセレクトシーン名
    private readonly string titleScene = "GameStartScene";          // タイトルシーン名
    private readonly string tutorialScene = "GameTutorialScene";    // チュートリアルシーン名

    // World_1
    private readonly string jumpEasyScene = "JumpEasyScene";        // かんたんゲームシーン名
    private readonly string jumpNormalScene = "JumpNormalScene";    // 普通ゲームシーン名
    private readonly string jumpHardScene = "JumpHardScene";        // むずかしいゲームシーン名
    private readonly string jumpExtraScene = "JumpExtraScene";      // エクストラゲームシーン名

    // World_2
    private readonly string easyGameScene = "EasyGameScene";        // かんたんゲームシーン名
    private readonly string normalGameScene = "normalGameScene";    // ふつうゲームシーン名
    private readonly string hardGameScene = "hardGameScene";        // むずかしいゲームシーン名
    private readonly string gameScene = "GameScene";                // エクストラゲームメインシーン名

    // World_2
    private readonly string w3_easyScene = "W3_EasyScene";        // かんたんゲームシーン名
    private readonly string w3_normalScene = "W3_NormalScene";    // ふつうゲームシーン名
    private readonly string w3_hardScene = "W3_HardScene";        // むずかしいゲームシーン名
    private readonly string w3_extraScene = "W3_ExtraScene";            // エクストラゲームメインシーン名

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
                SceneManager.LoadScene(jumpEasyScene);

                break;
            case StageLevelState.normal:
                SceneManager.LoadScene(jumpNormalScene);

                break;
            case StageLevelState.hard:
                SceneManager.LoadScene(jumpHardScene);
                break;
            case StageLevelState.extra:
                SceneManager.LoadScene(jumpExtraScene);
                break;

            case StageLevelState.tutorial:
                SceneManager.LoadScene(tutorialScene);
                break;

            case StageLevelState.W2_Easy:
                SceneManager.LoadScene(easyGameScene);

                break;
            case StageLevelState.W2_Normal:
                SceneManager.LoadScene(normalGameScene);

                break;
            case StageLevelState.W2_Hard:
                SceneManager.LoadScene(hardGameScene);

                break;
            case StageLevelState.W2_Extra:
                SceneManager.LoadScene(gameScene);

                break;
            case StageLevelState.W3_Easy:
                SceneManager.LoadScene(w3_easyScene);
                break;
            case StageLevelState.W3_Normal:
                SceneManager.LoadScene(w3_normalScene);
                break;
            case StageLevelState.W3_Hard:
                SceneManager.LoadScene(w3_hardScene);
                break;
            case StageLevelState.W3_Extra:
                SceneManager.LoadScene(w3_extraScene);
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

        // クリアしたことを記録
        SaveDataManager.instance.ClearStage(thisStageLevel);

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

}
