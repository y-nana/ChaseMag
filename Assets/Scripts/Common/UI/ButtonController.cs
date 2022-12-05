using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 最初に選択されている状態にするためのクラス
public class ButtonController : MonoBehaviour
{
    [SerializeField]
    private Button firstSelectButton;   // 最初に選択するボタン

    private void OnEnable()
    {
        // 選択処理
        if (firstSelectButton != null)
        {
            firstSelectButton.Select();
        }
    }
}
