using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �Q�[���̃Z�[�u�f�[�^���Ǘ�����N���X
// �V���O���g����DontDestroyOnLoad
public class SaveData : MonoBehaviour
{

    public static SaveData instance;    // ���̃N���X�̃C���X�^���X

    public List<StageClearData> stageClearDatas { get; private set; }

    private void Awake()
    {
        // �V���O���g���ɂ���
        if (instance == null)
        {
            instance = this;
            ResetData();
            DontDestroyOnLoad(gameObject);

        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ResetData()
    {

        stageClearDatas = new List<StageClearData>();

        for (int i = 0; i < System.Enum.GetValues(typeof(StageLevelState)).Length; i++)
        {
            StageClearData clearData = new StageClearData();
            clearData.stageLevel = (StageLevelState)System.Enum.ToObject(typeof(StageLevelState), i);
            clearData.isClear = false;
            stageClearDatas.Add(clearData);
        }
    }

    // �N���A����
    public void ClearStage(StageLevelState level)
    {
        if (stageClearDatas == null)
        {
            ResetData();
        }
        for (int i = 0; i < stageClearDatas.Count; i++)
        {
            if (stageClearDatas[i].stageLevel == level)
            {
                stageClearDatas[i].isClear = true;
                return;
            }
        }

        StageClearData data = new StageClearData();
        data.stageLevel = level;
        data.isClear = true;
        stageClearDatas.Add(data);
    }

    // �N���A�������ǂ���
    public bool IsClear(StageLevelState level)
    {
        if (stageClearDatas == null)
        {
            ResetData();
        }
        for (int i = 0; i < stageClearDatas.Count; i++)
        {
            if (stageClearDatas[i].stageLevel == level)
            {
                return stageClearDatas[i].isClear;
            }
        }

        return false;
    }

}
