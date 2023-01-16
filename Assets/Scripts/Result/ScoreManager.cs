using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// �N���A���̃X�R�A���Ǘ�����N���X
public class ScoreManager : MonoBehaviour
{

    // �\���p
    [SerializeField]
    private Text teslaText;             // �e�X����\������e�L�X�g
    [SerializeField]
    private Text clipText;              // �N���b�v�擾����\������e�L�X�g
    [SerializeField]
    private Text scoreText;             // �X�R�A��\������e�L�X�g
    [SerializeField]
    private Text rankText;              // �����N��\������e�L�X�g
    [SerializeField]
    private List<RankData> rankDatas;   //  �����N�f�[�^

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
        // TeslaManager�ɋL�^����Ă���e�X���̒l��\��
        teslaText.text = TeslaManager.tesla.ToString("f0") + " T";

        // �N���b�v�̕\��
        clipText.text = ClipManager.count.ToString();

        // �X�R�A�̕\��
        int score = GetScore();
        scoreText.text = score.ToString();

        // �����N�̕\��
        RankData rankData = GetRank(score);
        rankText.text = rankData.rank.ToString();
    }

    // �x�[�X�X�R�A����e�X���̕��������_�A�N���b�v�̕��������Z
    private int GetScore()
    {
        int score = baseScore;
        // �e�X�����Z
        float teslaValue = TeslaManager.tesla * teslaWeight;
        score -= (int)Mathf.Round(teslaValue);
        // �N���b�v���Z
        int clipBornus = ClipManager.count * clipScore;
        score += clipBornus;

        return score;
    }

    private RankData GetRank(int score)
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
