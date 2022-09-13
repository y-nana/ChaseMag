using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HowToPlayController : MonoBehaviour
{

    [SerializeField]
    private List<Sprite> howToPlayPages
        = new List<Sprite>();
    [SerializeField]
    private Image viewImaege;
    [SerializeField]
    private Button nextButton;
    [SerializeField]
    private Button beforeButton;
    [SerializeField]
    private Button backButton;

    private Image image;
    private int nowPageNum;

    private float preInput;

    private void OnEnable()
    {
        //image = this.GetComponent<Image>();
        nowPageNum = 0;
        viewImaege.sprite = howToPlayPages[nowPageNum];
        ButtonActiveCheck();
        nextButton.gameObject.SetActive(true);
        nextButton.Select();
        preInput = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {


        // 右
        if (Input.GetKeyDown(KeyCode.D) ||
            (Input.GetAxis("Horizontal") > 0 && preInput == 0.0f))
        {
            ChangeToPage(true);
        }
        // 左
        if (Input.GetKeyDown(KeyCode.A) || 
            (Input.GetAxis("Horizontal") < 0 && preInput == 0.0f))
        {
            ChangeToPage(false);
        }

        preInput = Input.GetAxis("Horizontal");

    }

    public void ChangeToPage(bool isNext)
    {
        if (isNext)
        {
            if (nowPageNum < howToPlayPages.Count-1)
            {
                nowPageNum++;
                viewImaege.sprite = howToPlayPages[nowPageNum];
            }
        }
        else
        {
            if (nowPageNum > 0)
            {
                nowPageNum--;
                viewImaege.sprite = howToPlayPages[nowPageNum];
            }
        }
        ButtonActiveCheck();
    }

    // ページ数によって表示ボタンを変える
    private void ButtonActiveCheck()
    {
        if (nowPageNum >= howToPlayPages.Count - 1)
        {
            nextButton.gameObject.SetActive(false);
            beforeButton.Select();
        }
        else if (nowPageNum <= 0)
        {
            beforeButton.gameObject.SetActive(false);
            nextButton.Select();
        }
        else
        {
            nextButton.gameObject.SetActive(true);
            beforeButton.gameObject.SetActive(true);
        }
    }

}
