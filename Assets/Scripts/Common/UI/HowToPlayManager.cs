using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 遊び方の表示
public class HowToPlayManager : MonoBehaviour
{

    [SerializeField]
    private Button viewHowToPlayButton;     // 遊び方を表示するボタン

    [SerializeField]
    private HowToPlayController howToPlay;  // 遊び方オブジェクト


    private void OnEnable()
    {
        howToPlay.gameObject.SetActive(false);
    }

    // 遊び方を表示する
    public void ViewHowToPlay()
    {
        howToPlay.gameObject.SetActive(true);
    }

    // 遊び方を閉じる
    public void CloseHowToPlay()
    {
        howToPlay.gameObject.SetActive(false);
        // 表示するボタンを選択状態にする
        viewHowToPlayButton.Select();

    }

    


}
