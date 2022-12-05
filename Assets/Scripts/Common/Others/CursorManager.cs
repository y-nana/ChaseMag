using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// 選択しているボタンをわかりやすく表示するためのクラス
public class CursorManager : MonoBehaviour
{

    [SerializeField]
    private EventSystem eventSystem;

    [SerializeField]
    private Color outlineColor;     // アウトラインを使ってカーソルを表現
    [SerializeField]
    private float speed = 2.0f;     // アウトラインが明滅するスピード

    private GameObject preSelectedObj;  // アウトラインを消すために前にセレクトされていたボタンを管理

    private Outline outline;

    private bool isPulus;           // 明か滅か（アニメーション用）

    // Update is called once per frame
    void Update()
    {
        try
        {
            // イベントシステムから選択状態にあるオブジェクトを取得
            var selected = eventSystem.currentSelectedGameObject.gameObject;
            // オブジェクトが前フレームと違ったら
            if (preSelectedObj != selected)
            {
                if (preSelectedObj != null)
                {
                    // アウトラインを移動
                    Debug.Log(selected);
                    Destroy(outline);
                    outline = selected.AddComponent<Outline>();
                    OutLineInit();
                }
                else
                {
                    // 初めて何かを選択するとき
                    outline = selected.AddComponent<Outline>();
                    OutLineInit();
                }

                preSelectedObj = selected;
            }
        }
        catch (NullReferenceException ex)
        {
            if (preSelectedObj!=null)
            {
                eventSystem.SetSelectedGameObject(preSelectedObj);

            }
        }

        // カーソルの明滅処理
        if (outline != null)
        {
            float value = speed * Time.unscaledDeltaTime;
            if (!isPulus)
            {
                value *= -1;
            }
            Color temp = outline.effectColor;
            temp.a += value;
            outline.effectColor =temp;

            if (outline.effectColor.a <= 0)
            {
                isPulus = true;
            }
            else if(outline.effectColor.a >=1)
            {
                isPulus = false;
            }

        }


    }

    // アウトラインの初期化
    private void OutLineInit()
    {
        outline.effectDistance = new Vector2(-20, -20);
        outline.effectColor = outlineColor;
    }


    
}
