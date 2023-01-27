using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StageClearData
{
    public StageLevelState level;
    public int score;

    public StageClearData()
    {
        level = StageLevelState.easy;
        score = 0;
    }
      

}
