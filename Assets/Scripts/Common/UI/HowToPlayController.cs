using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 遊び方の表示を制御するクラス
public class HowToPlayController : MonoBehaviour
{

    [SerializeField]
    private List<Sprite> howToPlayPages
        = new List<Sprite>();       // 遊び方の画像のリスト
    [SerializeField]
    private Image viewImaege;       // 表示するイメージオブジェクト
    [SerializeField]
    private Button nextButton;      // 次へ進むボタン
    [SerializeField]
    private Button beforeButton;    // 前へ戻るボタン
    [SerializeField]
    private Button backButton;      // 閉じるボタン

    private int nowPageNum;         // 現ページ数（リストのインデックス）
    private float preInput;         // 連続してページが進まないように前フレームの入力を保存

    // 有効になるたびに初期化
    private void OnEnable()
    {
        preInput = 0.0f;
        nowPageNum = 0;
        viewImaege.sprite = howToPlayPages[nowPageNum];
        ButtonActiveCheck();
    }

    // Update is called once per frame
    void Update()
    {


        // 右
        if (Input.GetKeyDown(KeyCode.D) ||
            (Input.GetAxis("Horizontal") > 0 && preInput == 0.0f))
        {
            ChangeToPage(true);
        }
        // 左
        if (Input.GetKeyDown(KeyCode.A) || 
            (Input.GetAxis("Horizontal") < 0 && preInput == 0.0f))
        {
            ChangeToPage(false);
        }

        // 入力状態の保存
        preInput = Input.GetAxis("Horizontal");

    }

    // ページをめくる
    public void ChangeToPage(bool isNext)
    {
        if (isNext)
        {
            if (nowPageNum < howToPlayPages.Count-1)
            {
                nowPageNum++;
                viewImaege.sprite = howToPlayPages[nowPageNum];
            }
        }
        else
        {
            if (nowPageNum > 0)
            {
                nowPageNum--;
                viewImaege.sprite = howToPlayPages[nowPageNum];
            }
        }
        // ボタンの有効無効化処理
        ButtonActiveCheck();
    }

    // ページ数によって表示ボタンを変える
    private void ButtonActiveCheck()
    {
        if (nowPageNum >= howToPlayPages.Count - 1)
        {
            nextButton.gameObject.SetActive(false);
            beforeButton.Select();
        }
        else if (nowPageNum <= 0)
        {
            beforeButton.gameObject.SetActive(false);
            nextButton.Select();
        }
        else
        {
            nextButton.gameObject.SetActive(true);
            beforeButton.gameObject.SetActive(true);
        }
    }

}
