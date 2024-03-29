using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Rank
{
    SSS,
    S,
    A,
    B,
    C,
    D,
    E
}

[System.Serializable]
public class RankData
{
    [field: SerializeField]
    public Rank rank { get; set; }          // ランク

    [field: SerializeField]
    public int lowerScore { get; set; }     // 下限値

    [field: SerializeField]
    public Sprite sprite;                   // 表示画像

}
