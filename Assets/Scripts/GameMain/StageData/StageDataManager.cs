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

}

// ステージ内のオブジェクトのデータ
[System.Serializable]
public class StagePart
{
    public StagePartsCategory category;
    public Vector2 position;
    public Vector2 scale;
    public bool isNorth;
}

// 一つのステージのデータ
[System.Serializable]
public class StageData
{
    public List<StagePart> stageParts = new List<StagePart>();
    public Vector2 topRightPos;
    public Vector2 bottomLeftPos;
}




public class StageDataManager : MonoBehaviour
{
    [SerializeField]
    private StageData stageData;



    // Start is called before the first frame update
    void Start()
    {
        GenerateStage();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GenerateStage()
    {
        // とりあえずオブジェクトの配置だけ

        foreach (var part in stageData.stageParts)
        {
            switch (part.category)
            {
                case StagePartsCategory.Scaffold:
                    break;
                case StagePartsCategory.JumpRamp:
                    break;
                case StagePartsCategory.Wall:
                    break;
                case StagePartsCategory.NormalWall:
                    break;
                case StagePartsCategory.ItemBox:
                    break;
                default:
                    break;
            }
        }

    }







}
