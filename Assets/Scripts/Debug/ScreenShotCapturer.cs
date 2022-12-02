using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// スクリーンショットを取るためのクラス
public class ScreenShotCapturer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // バックスペースでスクリーンショットを取る
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            CaptureScreenShot("ScreenShot.png");
        }
    }

    private void CaptureScreenShot(string filePath)
    {
        ScreenCapture.CaptureScreenshot(filePath);
    }
}
