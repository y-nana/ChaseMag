﻿using System.Collections;
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
    private SaveData saveData;              // クリア状況取得用

    [SerializeField]
    private Image clearIcon;                // クリアアイコン

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
            if (saveData != null)
            {
                clearIcon.enabled = saveData.IsClear(thisStageLevel);
                return;

            }
            else
            {
                clearIcon.enabled = false;
            }

        }
    }

    //ステージ開始処理
    public void onClick()
    {
        sceneDirector.ToGameStart(thisStageLevel);
    }




}
