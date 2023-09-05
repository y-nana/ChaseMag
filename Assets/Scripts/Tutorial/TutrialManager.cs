using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// チュートリアルのテキスト送りに関する状態
public enum TutorialState
{
    Auto,       // 一定時間後勝手に次のテキストへ
    Trigger,    // 何かトリガーが発生するまでそのまま
    Click       // クリックorボタンで次のテキストへ
}

// チュートリアルに用いるデータ
[System.Serializable]
struct TutorialData
{
    [field:SerializeField, TextArea]
    public string viewText { get; set; }                // テキストに表示する文章

    [field:SerializeField]
    public TutorialState tutorialState { get; set; }    // 文字送りの状態

    [field:SerializeField]
    public bool chaseSetting { get; set; }              // 鬼をセッティングするフェーズかどうか

    [field: SerializeField]
    public bool isResult { get; set; }                  // リザルトのフェーズかどうか

}

// チュートリアルの進行を制御するクラス
public class TutrialManager : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI tutrialText;    // チュートリアルのテキスト

    [SerializeField]
    private List<TutorialData> tutorialDatas = new List<TutorialData>();
                                            // データのリスト

    // リザルトで表示するテキスト 状況で変化させる
    [SerializeField]
    private string clearTxt;                // クリアした時の文章
    [SerializeField]
    private string overTxt;                 // 捕まった時の文章

    private int tutorialIndex;              // データリストのインデックス

    [SerializeField]
    private float waitTime = 1.0f;          // オートのときにすべて表示後待つ時間

    private float waitTimer;                // 待っている時間を計測するタイマー

    public bool waitTrigger { get; set; }               // 次に進むトリガー

    // チュートリアルで必要な処理をここから呼び出せるようにする
    public System.Action setChaser { get; set; }        // 鬼をセッティングする処理
    public System.Action startChaser { get; set; }      // 鬼をスタートさせる処理
    public System.Action tutorialFinish { get; set; }   // チュートリアル終了後の処理

    public bool clearFlag { get; set; } = false;        // リザルトの状態

    [SerializeField]
    private float textSpeed = 0.1f;         // 文字送りのスピード

    private bool isDrowing;                 // 現在文字を表示途中かどうか


    void Start()
    {
        // 初期化
        waitTrigger = false;
        isDrowing = false;
        tutorialIndex = 0;
        // コルーチンの開始
        StartCoroutine(CoTutorial());
    }

    // 文字を表示させるコルーチン
    IEnumerator CoDrawText(TutorialData data)
    {
        // クリック待ちのテキストのときは表示終了まで次へ進めなくする
        switch (data.tutorialState)
        {
            case TutorialState.Auto:
                GameStateManager.instance.ToPlaying();
                break;
            case TutorialState.Trigger:
                GameStateManager.instance.ToPlaying();
                break;
            case TutorialState.Click:
                GameStateManager.instance.ToEvent();
                break;
            default:
                break;
        }

        isDrowing = true;
        float time = 0;
        // 文字の表示
        while (true)
        {

            yield return null;
            time += Time.deltaTime;

            /*
            // 一気に表示
            if (Input.GetButtonDown("Submit") && GameStateManager.instance.gameState == GameState.inEvent)
            {
                break;
            }
            */

            int len = Mathf.FloorToInt(time / textSpeed);
            // すべての文字を表示し終えたらwhileから出る
            if (len > data.viewText.Length)
            {
                break;
            }

            tutrialText.text = data.viewText.Substring(0, len);
        }
        tutrialText.text = data.viewText;
        yield return null;
        isDrowing = false;
    }

    // 次のテキストへ送るのを待つコルーチン
    IEnumerator ToNextText(TutorialState state)
    {
        while (isDrowing) yield return null;
        switch (state)
        {
            // オートだったら時間経過で次へ
            case TutorialState.Auto:
                waitTimer = waitTime;
                while (waitTimer > 0.0f)
                {
                    waitTimer -= Time.deltaTime;
                    yield return null;
                }
                break;

                // トリガーがtrueになるまで待つ
            case TutorialState.Trigger:
                while (!waitTrigger) yield return null;
                waitTrigger = false;
                break;

                // クリックされるまで待つ
            case TutorialState.Click:
                while (!(Input.GetButtonDown("Submit") && GameStateManager.instance.gameState == GameState.InEvent)) yield return null;
                break;
        }

    }

    // チュートリアルのコルーチン
    IEnumerator CoTutorial()
    {
        // チュートリアルデータがある限り繰り返す
        while (tutorialIndex < tutorialDatas.Count)
        {

            //Debug.Log(tutorialIndex);
            // 鬼をセッティングするフェーズのとき
            if (tutorialDatas[tutorialIndex].chaseSetting)
            {
                setChaser?.Invoke();

            }
            // リザルトのフェーズのとき
            if (tutorialDatas[tutorialIndex].isResult)
            {
                TutorialData data = new TutorialData();
                // 逃げ切れたかどうかでメッセージを変化
                data.viewText = clearFlag ? clearTxt : overTxt;
                data.tutorialState = tutorialDatas[tutorialIndex].tutorialState;
               tutorialDatas[tutorialIndex] = data;
            }

            // 文字を表示する
            StartCoroutine(CoDrawText(tutorialDatas[tutorialIndex]));

            //前のデータで鬼をセッティングしているとき
            if (tutorialIndex > 0)
            {
                if (tutorialDatas[tutorialIndex - 1].chaseSetting)
                {
                    // 鬼を動かし始める処理を行う
                    startChaser?.Invoke();
                }
            }
            // テキスト送りのコルーチンを開始
            yield return StartCoroutine(ToNextText(tutorialDatas[tutorialIndex].tutorialState));
            // 次のデータへ
            tutorialIndex++;


        }
        yield return null;

        // チュートリアル終了
        GameStateManager.instance.ToPlaying();
        tutorialFinish?.Invoke();
    }


}
