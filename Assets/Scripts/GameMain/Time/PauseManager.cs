using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{

    [SerializeField] GameObject pauseObject;

    //public static bool nowPause { get; set; }


    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
        pauseObject.SetActive(false);
        
        //nowPause = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Qキーか指定のボタンでポーズを表示、非表示
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetButtonDown("Menu"))
        {
            if (!pauseObject.activeSelf)
            {
                // 表示する
                pauseObject.SetActive(true);
                // 時間を止める
                Time.timeScale = 0f;
                GameStateManager.instance.ToPause();
                //nowPause = true;
            }
            else
            {
                // 非表示にする
                GameBack();
            }
        }
    }

    // ゲームに戻る ポーズの非表示
    public void GameBack()
    {
        pauseObject.SetActive(false);
        // 時間を進める
        Time.timeScale = 1f;
        GameStateManager.instance.ToPlaying();
        //nowPause = false;
    }


}
