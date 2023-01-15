using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ステージセレクトボタンを制御するクラス
public class StageSelectButtonController : MonoBehaviour
{
    [SerializeField]
    private StageLevelState thisStageLevel; // 該当難易度

    [SerializeField]
    private SceneDirector sceneDirector;    // シーン遷移する用

    [SerializeField]
    private Image clearIcon;                // クリアアイコン

    [SerializeField]
    private Text text;
    private void Start()
    {
        // 直前やってたステージへのボタンを選択する
        if (thisStageLevel == SceneDirector.NextStageLevel)
        {
            this.GetComponent<Button>().Select();
        }

        // クリア状況の表示
        if (clearIcon != null)
        {
            if (text)
            {
                text.text = SaveData.instance.IsClear(thisStageLevel).ToString();
            }
            clearIcon.enabled = SaveData.instance.IsClear(thisStageLevel);
        }
    }

    //ステージ開始処理
    public void onClick()
    {
        sceneDirector.ToGameStart(thisStageLevel);
    }




}
