using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CountDownManager : MonoBehaviour
{    

    [SerializeField] float countDownTime = 5.0f;    // カウントダウンする時間
    [SerializeField] float cageDestroyTime = 1.0f;  // 檻を破壊するまでの時間

    // コントロール用コンポーネント
    //[SerializeField] Text countDownText; // 文字を表示する(子オブジェクト)
    [SerializeField] TextMeshProUGUI countDownText; // 文字を表示する(子オブジェクト)
    private Image gage;         // 周りのゲージ

    [SerializeField] TimeMamager timeManage;     // ゲームを始めるため
    [SerializeField] GameObject cage;   // チェイサーを閉じ込めている檻

    private float timer;    // タイマー

    // デストロイメソッドをInvokeで呼ぶため
    private readonly string DestroyMethod = "Destroy";


    // Start is called before the first frame update
    void Start()
    {       
        gage = this.GetComponent<Image>();
        // タイマー初期化
        timer = countDownTime;
        countDownText.text = timer.ToString("f0");
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        countDownText.text =  Mathf.Ceil(timer).ToString();
        // 一秒で一周回るようにゲージを削る
        gage.fillAmount -= Time.deltaTime;
        // 一秒ごとにゲージを戻す
        if (gage.fillAmount <= 0)
        {
            gage.fillAmount = 1;
        }
        // カウントダウンが終わったらゲーム開始
        if (timer <= 0)
        {
            GameStart();
        }
    }
    
    private void GameStart()
    {
        // ゲームのタイマーを起動
        timeManage.SetPlaying(true);
        // 檻を上に移動させる
        cage.transform.position += Vector3.up * Time.deltaTime * 4;
        // Destroyが実行されるまで透明にしておく
        countDownText.gameObject.SetActive(false);
        gage.color = Color.clear;
        // 一定時間後にDestroy
        Invoke(DestroyMethod, cageDestroyTime);
    }

    // 檻とカウントダウンを消す
    private void Destroy()
    {
        Destroy(cage);
        Destroy(this.gameObject);
    }
}
