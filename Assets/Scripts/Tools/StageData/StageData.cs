using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 一つのステージのデータ
[System.Serializable]
public class StageData
{
    public List<StagePart> stageParts;

    public float width;

    public float height;

    public StageData()
    {
        stageParts = new List<StagePart>();
    }
}
