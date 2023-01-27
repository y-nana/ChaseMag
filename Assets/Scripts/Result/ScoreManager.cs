using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// クリア時のスコアを管理するクラス
public class ScoreManager : MonoBehaviour
{

    // 表示用
    [SerializeField]
    private Text teslaText;             // テスラを表示するテキスト
    [SerializeField]
    private Text clipText;              // クリップ取得数を表示するテキスト
    [SerializeField]
    private Text scoreText;             // スコアを表示するテキスト
    [SerializeField]
    private Text rankText;              // ランクを表示するテキスト
    [SerializeField]
    private Image rankImage;            // ランクを表示するイメージ
    [SerializeField]
    private Image SSSEfect;             // SSSの時のみ表示するエフェクト
    [SerializeField]
    private RankDataBase rankDataBase;   //  ランクデータ

    [SerializeField]
    private int baseScore;
    [SerializeField]
    private float teslaWeight;
    [SerializeField]
    private int clipScore;


    void Start()
    {

        ViewResult();

    }


    private void ViewResult()
    {
        // TeslaManagerに記録されているテスラの値を表示
        teslaText.text = TeslaManager.tesla.ToString("f0") + " T";

        // クリップの表示
        clipText.text = ClipManager.count.ToString();

        // スコアの表示
        int score = GetScore();
        scoreText.text = score.ToString();

        // ランクの表示
        RankData rankData = rankDataBase.GetRank(score);
        rankImage.sprite = rankData.sprite;
        //rankText.text = rankData.rank.ToString();
        if (rankData.rank == Rank.SSS)
        {
            SSSEfect.enabled = true;
        }
        else
        {
            SSSEfect.enabled = false;
        }

        // スコアを記録
        SaveScore(score);
    }

    // ベーススコアからテスラの分だけ減点、クリップの分だけ加算
    private int GetScore()
    {
        int score = baseScore;
        // テスラ減算
        float teslaValue = TeslaManager.tesla * teslaWeight;
        score -= (int)Mathf.Round(teslaValue);
        // クリップ加算
        int clipBornus = ClipManager.count * clipScore;
        score += clipBornus;

        return score;
    }

    private void SaveScore(int score)
    {
        // ランク、スコアを記録
        StageLevelState level = SceneDirector.NextStageLevel;
        SaveDataManager.instance.SetClearData(level, score);
    }
}
