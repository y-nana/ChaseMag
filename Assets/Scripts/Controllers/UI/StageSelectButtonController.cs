using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSelectButtonController : MonoBehaviour
{
    [SerializeField]
    private StageLevelState thisStageLevel; // 該当難易度

    [SerializeField]
    private SceneDirector sceneDirector;    // シーン遷移する用

    private void Start()
    {
        if (thisStageLevel== SceneDirector.NextStageLevel)
        {
            this.GetComponent<Button>().Select();
        }
    }
    //ステージ開始処理
    public void onClick()
    {
        sceneDirector.ToGameStart(thisStageLevel);
    }


}
