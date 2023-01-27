using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ステージセレクトボタンを制御するクラス
public class StageSelectButtonController : MonoBehaviour
{
    [SerializeField]
    private StageLevelState thisStageLevel; // 該当難易度

    [field:SerializeField]
    public World thisWorld { get; private set; }    // 該当ワールド

    [SerializeField]
    private SceneDirector sceneDirector;    // シーン遷移する用

    [SerializeField]
    private Image clearIcon;                // クリアアイコン
    [SerializeField]
    private Text scoreText;                 // スコア表示
    [SerializeField]
    private Image rankImage;                // ランク表示

    [SerializeField]
    RankDataBase rankDataBase;              // ランク取得用

    private void Start()
    {
        // 直前やってたステージへのボタンを選択する
        if (thisStageLevel == SceneDirector.NextStageLevel)
        {
            this.GetComponent<Button>().Select();
        }

        // クリア状況の表示
        ShowClearData();
    }

    // クリアデータを見せる
    private void ShowClearData()
    {
        bool isClear = SaveDataManager.instance.IsClear(thisStageLevel);
        // アイコンを見せる
        ShowIcon(isClear);
        if (!isClear)
        {
            return;
        }

        // スコア、ランクのデータを入れる
        int score = SaveDataManager.instance.GetMaxScore(thisStageLevel);

        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }

        if (rankImage != null)
        {
            RankData rankData = rankDataBase.GetRank(score);
            rankImage.sprite = rankData.sprite;
        }
    }


    // アイコンを表示する
    private void ShowIcon(bool isClear)
    {
        if (clearIcon != null)
        {
            clearIcon.gameObject.SetActive(isClear);
        }
        if (scoreText != null)
        {
            scoreText.gameObject.SetActive(isClear);
        }
        if (rankImage != null)
        {
            rankImage.gameObject.SetActive(isClear);
        }
    }


    //ステージ開始処理
    public void onClick()
    {
        sceneDirector.ToGameStart(thisStageLevel);
    }




}
