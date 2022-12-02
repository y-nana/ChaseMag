using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // カメラ位置調整用
    [SerializeField] int marginY;       // ド真ん中よりちょっと下に

    [SerializeField]
    private int limitDown;        // 下限
    [SerializeField]
    private int limitWidth;      // 横の限界値
    [SerializeField]
    private float limitHigh;    // 上限
    private readonly int posZ = -10;           // Zの値（固定）

    // コントロール用コンポーネント
    private Transform myTransform;
    [SerializeField] Transform playerTransform;

    // 取得用タグ名
    //private readonly string playerTagName = "Player";   // プレイヤ

    void Start()
    {
        //playerTransform = 
          //  GameObject.FindGameObjectWithTag(playerTagName)
            //.GetComponent<Transform>();
        myTransform = this.transform;

    }

    void Update()
    {
        // プレイヤのポジション取得
        Vector2 playerPos = playerTransform.position;
        Vector2 cameraPos = new Vector2(playerPos.x, playerPos.y - marginY);
        // 限界値よりはみ出していたら修正
        if (playerPos.y < marginY + limitDown)      cameraPos.y = limitDown;
        if (playerPos.y > limitHigh)    cameraPos.y = limitHigh - marginY;

        if (playerPos.x > limitWidth)   cameraPos.x = limitWidth;
        if (playerPos.x < -limitWidth)  cameraPos.x = -limitWidth;

        // 値を入れる
        myTransform.position = new Vector3(cameraPos.x, cameraPos.y, posZ);
    }
}
