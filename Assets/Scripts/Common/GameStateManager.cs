using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// ゲームの状態
public enum GameState
{
    Playing,    // プレイ中
    InEvent,    // イベント中（チュートリアルなど）
    Pause       // ポーズ中
}

// ゲームの状態を管理するクラス
// シングルトンでDontDestroyOnLoad
public class GameStateManager : MonoBehaviour
{
    public static GameStateManager instance;    // このクラスのインスタンス

    public GameState gameState { get; private set; }    // 現在の状態

    private GameState preGameState;                     // 前の状態

    private void Awake()
    {
        // シングルトンにする
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            instance.gameState = GameState.Playing;
            // シーン遷移後は絶対にPlaying
            SceneManager.sceneLoaded += ToNextScene;

        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ポーズ中にする
    public void ToPause()
    {
        preGameState = instance.gameState;
        instance.gameState = GameState.Pause;
    }

    // プレイ中にする
    public void ToPlaying()
    {
        if (instance.gameState == GameState.Pause)
        {
            instance.gameState = instance.preGameState;
        }
        else
        {
            instance.gameState = GameState.Playing;
        }
    }

    // シーン遷移時に行う処理
    public void ToNextScene(Scene nextScene, LoadSceneMode mode)
    {
        // 状態をプレイ中にする
        instance.gameState = GameState.Playing;
        // 時間を進める
        Time.timeScale = 1.0f;
    }

    // イベント中にする
    public void ToEvent()
    {
        instance.gameState = GameState.InEvent;
    }

    // 入力を受け付ける状態かどうかを取得
    public bool IsInputtable()
    {
        // プレイ中だったら入力を受け付ける
        return gameState == GameState.Playing;
    }

    

}
