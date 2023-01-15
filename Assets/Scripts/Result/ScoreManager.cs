using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// クリア時のスコアを管理するクラス
public class ScoreManager : MonoBehaviour
{

    // 表示用
    [SerializeField]
    private Text teslaText;             // テスラを表示するテキスト
    [SerializeField]
    private Text rankText;              // ランクを表示するテキスト
    [SerializeField]
    private Text clipText;              // クリップ取得数を表示するテキスト

    private string rank = "C";          // ランク


    void Start()
    {
        ViewResult();

    }



    private int GetScore()
    {
        return 0;
    }

    private void ViewResult()
    {
        // TeslaManagerに記録されているテスラの値を表示
        teslaText.text = TeslaManager.tesla.ToString("f0") + " T";

        // ランクの表示
        rankText.text = rank;

        // クリップの表示
        clipText.text = ClipManager.count.ToString();
    }


}
