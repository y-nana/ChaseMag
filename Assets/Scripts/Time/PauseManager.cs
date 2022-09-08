using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{

    [SerializeField] GameObject pauseObject;
    public static bool nowPause;

    [SerializeField] GameObject playingPage;
    [SerializeField] GameObject settingPage;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
        pauseObject.SetActive(false);
        nowPause = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetButtonDown("Menu"))
        {
            if (!pauseObject.activeSelf)
            {
                pauseObject.SetActive(true);
                Time.timeScale = 0f;
                nowPause = true;
            }
            else
            {
                GameBack();
            }
        }
    }

    // ポーズ画面で選択するためメソッド化
    public void GameBack()
    {
        pauseObject.SetActive(false);
        Time.timeScale = 1f;
        nowPause = false;
    }

    public void ToPlayingPage()
    {

    }

    public void ToSettingPage()
    {
        
    }

}
