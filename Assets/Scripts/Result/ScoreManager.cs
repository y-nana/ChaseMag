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
    private Image rankImage;            // �����N��\������C���[�W
    [SerializeField]
    private Image SSSEfect;             // SSS�̎��̂ݕ\������G�t�F�N�g
    [SerializeField]
    private RankDataBase rankDataBase;   //  �����N�f�[�^

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

        // �X�R�A���L�^
        SaveScore(score);
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

    private void SaveScore(int score)
    {
        // �����N�A�X�R�A���L�^
        StageLevelState level = SceneDirector.NextStageLevel;
        SaveDataManager.instance.SetClearData(level, score);
    }
}
