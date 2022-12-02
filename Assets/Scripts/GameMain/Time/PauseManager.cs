using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{

    [SerializeField] GameObject pauseObject;
    public static bool nowPause;


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
                GameStateManager.instance.ToPause();
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
        GameStateManager.instance.ToPlaying();
        nowPause = false;
    }


}
