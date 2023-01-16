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
    public Rank rank { get; set; }          // ƒ‰ƒ“ƒN

    [field: SerializeField]
    public int lowerScore { get; set; }     // ‰ºŒÀ’l

    [field: SerializeField]
    public Sprite sprite;                   // •\Ž¦‰æ‘œ

}
