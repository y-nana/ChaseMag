using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class TimeMamager : MonoBehaviour
{
    [SerializeField] GameObject sceneDirector;    // シーン遷移用
    [SerializeField] GameObject timeBack;           // ゲージ操作用

    [SerializeField] float limitTime = 45.0f;           // 制限時間
    [SerializeField] float changeOrangeTime = 20.0f;    // 文字色変更オレンジ
    [SerializeField] float changeRedTime = 10.0f;       // 文字色変更赤


    private Image timeGage;

    private Text timeText;  // 表示用
    private float timer;    // タイマー
    private bool playing;   // ゲーム進行中か



    private readonly Color orange = new Color32(255, 100, 0, 255);   // 文字色変更用(オレンジ)

    // Start is called before the first frame update
    void Start()
    {
        timeGage = timeBack.GetComponent<Image>();
        this.timeText = GetComponent<Text>();
        // 初期化
        this.timer = this.limitTime;
        timeText.text = Mathf.Ceil(timer).ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (playing)
        {
            this.timer -= Time.deltaTime;
            // ゲージ減少
            timeGage.fillAmount = timer / limitTime;
            // 切り上げ表示
            timeText.text = Mathf.Ceil(timer).ToString();
            // 文字色変更
            ColorChange();
            // タイマーが0以下になったらクリアシーンへ
            if (timer <= 0)
            {
                playing = false;
                ExecuteEvents.Execute<SceneCaller>(
                    target: sceneDirector,
                    eventData: null,
                    functor: (reciever, eventData) => reciever.ToGameClear()
                    );

                
            }
            
        }

    }

    // 文字色変更
    private void ColorChange()
    {
        if (timer < changeOrangeTime)
        {
            timeText.color = orange;
            if (timer < changeRedTime)
            {
                timeText.color = Color.red;
            }
        }
    }

    // プロパティにしたほうがいい？
    public void SetPlaying(bool now)
    {
        playing = now;
    }

}
