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
    private List<RankData> rankDatas;   //  ランクデータ

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
        RankData rankData = GetRank(score);
        rankText.text = rankData.rank.ToString();
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

    private RankData GetRank(int score)
    {
        foreach (RankData data in rankDatas)
        {
            // スコアが上回っていたらそのランクを返す
            if (score > data.lowerScore)
            {
                return data;
            }
        }
        // 一番下のランクを返す
        return rankDatas[rankDatas.Count - 1];
    }

}
