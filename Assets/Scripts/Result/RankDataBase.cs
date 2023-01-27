using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// アイテムのデータをまとめる
[CreateAssetMenu(fileName = "RankDataBase", menuName = "CreateRankDataBase")]
public class RankDataBase : ScriptableObject
{
    [SerializeField]
    private List<RankData> rankDatas = new List<RankData>();

    public RankData GetRank(int score)
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

