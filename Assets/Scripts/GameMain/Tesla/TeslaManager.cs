using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// テスラの値を制御するクラス
public class TeslaManager : MonoBehaviour
{

    // テスラ計算用
    [SerializeField] GameObject player;
    [SerializeField] GameObject chaser;

    [SerializeField] float maxDistance = 80;    // 距離の最大値
    [SerializeField] float maxTesla = 50;       // テスラの最大値
    private float itemWeight = 0;

    private string teslaText;   // 表示用

    private Text text;          // UI表示用

    private float distance;     // 距離
    public static float tesla;  // テスラ (小さい方がいい)


    void Start()
    {
        text = GetComponent<Text>();
    }

    void Update()
    {
        // チェイサーとプレイヤの距離を取得
        Vector2 pPos = player.transform.position;
        Vector2 cPos = chaser.transform.position;
        distance = Vector2.Distance(pPos, cPos);
        // テスラを求めるための計算 要検討
        tesla = maxTesla - distance / maxDistance * maxTesla;
        tesla += itemWeight;


        // 表示
        teslaText = tesla.ToString("f0") + "  T";
        text.text = teslaText;
    }

    // アイテムによる効果を加算する
    public void ChangeWeight(float amount)
    {
        itemWeight += amount;
    }

}
