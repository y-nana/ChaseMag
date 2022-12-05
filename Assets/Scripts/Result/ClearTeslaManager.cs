using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// リザルトの表示を管理するクラス
public class ClearTeslaManager : MonoBehaviour
{
    // ランク 値は十の位
    [SerializeField] int SSS = 1;
    [SerializeField] int S = 2;
    [SerializeField] int A = 3;
    [SerializeField] int B = 4;
    [SerializeField] int C = 5;

    // 表示用
    private Text teslaText;             // テスラを表示するテキスト
    [SerializeField] Text rankText;     // ランクを表示するテキスト

    private string rank = "C";          // ランク
    // 前に置く表示するテキスト
    private readonly string showTesla = "テスラ：";
    private readonly string showRank = "ランク：";

    void Start()
    {
        this.teslaText = GetComponent<Text>();
        // TeslaManagerに記録されているテスラの値を表示
        teslaText.text = showTesla + 
            TeslaManager.tesla.ToString("f0") + "  T";
        // 十の位を用いてランクを設定
        int tesla = (int)TeslaManager.tesla / 10;
        if (tesla <= SSS) rank = nameof(SSS);
        if (tesla == S) rank = nameof(S);
        if (tesla == A) rank = nameof(A);
        if (tesla == B) rank = nameof(B);
        if (tesla >= C) rank = nameof(C);
        // ランクの表示
        rankText.text = showRank + " " + rank;

    }


}
