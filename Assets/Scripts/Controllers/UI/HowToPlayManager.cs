using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HowToPlayManager : MonoBehaviour
{

    [SerializeField]
    private Button viewHowToPlayButton;

    [SerializeField]
    private HowToPlayController howToPlay;


    private void OnEnable()
    {
        howToPlay.gameObject.SetActive(false);
    }

    // ページ数によって表示ボタンを変える
    public void ViewHowToPlay()
    {
        howToPlay.gameObject.SetActive(true);
    }

    public void CloseHowToPlay()
    {
        howToPlay.gameObject.SetActive(false);
        viewHowToPlayButton.Select();

    }


}
