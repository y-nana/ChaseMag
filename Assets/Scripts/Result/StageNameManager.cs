using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
// ステージ名を管理する
public struct StageName
{
    public StageLevelState level;
    public string name;
}

// ステージ名を表示する
public class StageNameManager : MonoBehaviour
{
    [SerializeField]
    private List<StageName> stageNames = new List<StageName>();


    private void Start()
    {
        if (gameObject.GetComponent<Text>())
        {
            gameObject.GetComponent<Text>().text = GetStageName();
        }
    }

    private string GetStageName()
    {
        foreach (var item in stageNames)
        {
            if (item.level == SceneDirector.NextStageLevel)
            {
                return item.name;
            }
        }
        return string.Empty;
    }



}
