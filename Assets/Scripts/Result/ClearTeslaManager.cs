using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClearTeslaManager : MonoBehaviour
{
    [SerializeField] int SSS = 1;
    [SerializeField] int S = 2;
    [SerializeField] int A = 3;
    [SerializeField] int B = 4;
    [SerializeField] int C = 5;

    private Text teslaText;
    [SerializeField] Text rankText;
    private string rank = "C";
    private readonly string showTesla = "テスラ：";
    private readonly string showRank = "ランク：";

    void Start()
    {
        this.teslaText = GetComponent<Text>();
        // TeslaManagerに記録されているテスラの値を表示
        teslaText.text = showTesla + 
            TeslaManager.tesla.ToString("f0") + "  T";
        int tesla = (int)TeslaManager.tesla / 10;
        if (tesla <= SSS) rank = nameof(SSS);
        if (tesla == S) rank = nameof(S);
        if (tesla == A) rank = nameof(A);
        if (tesla == B) rank = nameof(B);
        if (tesla >= C) rank = nameof(C);

        rankText.text = showRank + " " + rank;

    }


}
