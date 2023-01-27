using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �A�C�e���̃f�[�^���܂Ƃ߂�
[CreateAssetMenu(fileName = "RankDataBase", menuName = "CreateRankDataBase")]
public class RankDataBase : ScriptableObject
{
    [SerializeField]
    private List<RankData> rankDatas = new List<RankData>();

    public RankData GetRank(int score)
    {
        foreach (RankData data in rankDatas)
        {
            // �X�R�A�������Ă����炻�̃����N��Ԃ�
            if (score > data.lowerScore)
            {
                return data;
            }
        }
        // ��ԉ��̃����N��Ԃ�
        return rankDatas[rankDatas.Count - 1];
    }
}

