using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ゲームのセーブデータ
[System.Serializable]
public class SaveData
{
    [field:SerializeField]
    public List<StageClearData> stageClearDatas { get; set; }
    public SaveData()
    {
        stageClearDatas = new List<StageClearData>();
    }


}
