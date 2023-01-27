using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    [SerializeField]
    private Button viewSettingButton;     // 遊び方を表示するボタン

    [SerializeField]
    private GameObject setting;  // 遊び方オブジェクト
    [SerializeField]
    private Button firstButton;  // 選択ボタン


    private void OnEnable()
    {
        setting.gameObject.SetActive(false);
    }

    // 遊び方を表示する
    public void ViewSetting()
    {
        setting.gameObject.SetActive(true);
        firstButton.Select();
    }

    // 遊び方を閉じる
    public void CloseSetting()
    {
        setting.gameObject.SetActive(false);
        // 表示するボタンを選択状態にする
        viewSettingButton.Select();

    }
}
