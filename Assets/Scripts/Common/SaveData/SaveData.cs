using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �Q�[���̃Z�[�u�f�[�^
[System.Serializable]
public class SaveData
{

    public List<StageClearData> stageClearDatas;
    
    public SaveData()
    {
        stageClearDatas = new List<StageClearData>();
    }

    // �N���A����
    public void ClearStage(StageLevelState level)
    {

        for (int i = 0; i < stageClearDatas.Count; i++)
        {
            if (stageClearDatas[i].level == level)
            {
                return;
            }
        }
        StageClearData data = new StageClearData();
        data.level = level;
        stageClearDatas.Add(data);

    }

    public void SetClearData(StageLevelState level, int score)
    {

        for (int i = 0; i < stageClearDatas.Count; i++)
        {
            if (stageClearDatas[i].level == level)
            {
                if (score > stageClearDatas[i].score)
                {
                    stageClearDatas[i].score = score;

                }

                return;
            }
        }

        StageClearData data = new StageClearData();
        data.level = level;
        data.score = score;
        stageClearDatas.Add(data);

    }


    // �N���A�������ǂ���
    public bool IsClear(StageLevelState level)
    {

        for (int i = 0; i < stageClearDatas.Count; i++)
        {
            if (stageClearDatas[i].level == level)
            {
                return true;
            }
        }
        return false;
    }

    public int GetMaxScore(StageLevelState level)
    {
        int score = 0;
        for (int i = 0; i < stageClearDatas.Count; i++)
        {
            if (stageClearDatas[i].level == level)
            {
                return stageClearDatas[i].score;
            }
        }

        return score;
    }




}
