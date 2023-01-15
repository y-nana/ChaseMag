using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StagePartsCategory
{
    Scaffold,
    JumpRamp,
    Wall,
    NormalWall,
    ItemBox,
    PoleScaffold,
    Clip

}

// ステージ内のオブジェクトのデータ
[System.Serializable]
public class StagePart
{
    public StagePartsCategory category;

    public Vector2 position;

    public Vector2 sizeMagnification = Vector2.one;

    public bool isNorth;
}

