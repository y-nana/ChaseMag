using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Pole;

public class PoleController : MonoBehaviour
{
    // 磁力の強さ
    [SerializeField] float defaultPoleStrong = 1.0f;  // デフォルト値
    // 現在の強さ
    public float PoleStrong {
        get;
        private set;
    }

    // 極の向き s極を基準に
    private int southPole;
    // 0～4の間で
    private readonly int poleMin = 0;
    private readonly int poleMax = 3;  


    // 極の向きを変更する用の定数 enumのほうがいい？？
    private readonly int turnRight = 1;    // 右回転
    private readonly int turnLeft = -1;    // 左回転
    private readonly int reverse = 2;      // 上下反転
    private readonly int angle = -90;      // 一回の回転の角度

    // 向きをわかりやすくするためのもの(子オブジェクト)
    [SerializeField] Transform pole;

    private AudioSource audioSource;        // サウンド
                                            // 効果音
    [SerializeField]
    private AudioClip changePoleSE;

    private void Start()
    {
        // 磁力の強さを初期化
        PoleStrong = defaultPoleStrong;

        // 極の向きを最初はs極を左に
        southPole = (int)PoleOrientation.Left;
        this.audioSource = GetComponent<AudioSource>();

        //pole = transform.GetChild(0);
    }

    void Update()
    {
        if (GameStateManager.instance.IsInputtable())
        //if (!PauseManager.nowPause)
        {
            // 極の向きを変更する
            if (Input.GetKeyDown(KeyCode.RightArrow) 
                || Input.GetMouseButtonDown(1)
                || Input.GetButtonDown("Action1")) PoleChange(turnRight);
            if (Input.GetKeyDown(KeyCode.LeftArrow) 
                || Input.GetMouseButtonDown(0)
                || Input.GetButtonDown("Action0")) PoleChange(turnLeft);

            if (Input.GetKeyDown(KeyCode.UpArrow)
                || Input.GetKeyDown(KeyCode.DownArrow)
                || Input.GetButtonDown("Action2")) PoleChange(reverse);

        }

        /*
        // 極の向きを変更する 矢印キーがそのまま向きに対応するVar
        if (Input.GetKeyDown(KeyCode.RightArrow)) PoleChange(PoleOrientation.RIGHT);
        if (Input.GetKeyDown(KeyCode.LeftArrow)) PoleChange(PoleOrientation.LEFT);
        if (Input.GetKeyDown(KeyCode.UpArrow)) PoleChange(PoleOrientation.UP);
        if (Input.GetKeyDown(KeyCode.DownArrow)) PoleChange(PoleOrientation.DOWN);
        */

    }

    // 極の向き変更
    private void PoleChange(int change)
    {
        // 見た目の角度を変更
        transform.Rotate(0, 0, angle * change);

        southPole += change;

        // southPoleが範囲外になったら修正
        if (southPole < poleMin) southPole += 4;
        if (southPole > poleMax) southPole -= 4;

        audioSource.PlayOneShot(changePoleSE);

    }

    // 極の向き変更 矢印キーがそのまま向きに対応するVar
    /*
    private void PoleChange(int change)
    {
        // 見た目の角度を変更
        transform.localEulerAngles = new Vector3(0f,0f, change * angle);

        southPole = change;
    }
    */

    // 今の磁力の向きが指定された向きと一致するかどうか見る
    public bool PoleCheck(int orientation)
    {
        return southPole == orientation;
    }


    // 磁力の強さを変える
    public void ChangePoleStrong(float change)
    {
        pole.localScale += new Vector3(change, change,0);
        PoleStrong += change;
    }



}
