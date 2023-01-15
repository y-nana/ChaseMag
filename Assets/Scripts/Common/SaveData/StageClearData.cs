using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StageClearData
{
    [field:SerializeField]
    public StageLevelState stageLevel { get; set; }
    [field:SerializeField]
    public bool isClear { get; set; }
    

}
