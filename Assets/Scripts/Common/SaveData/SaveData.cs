using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// アイテムのデータを管理
[System.Serializable]
[CreateAssetMenu(fileName = "SaveData", menuName = "CreateSaveData")]
public class SaveData: ScriptableObject
{
    [field:SerializeField]
    public List<StageClearData> stageClearDatas { get; set; }

    private void OnEnable()
    {
        //ResetData();
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

    // クリアした
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

    // クリアしたかどうか
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
