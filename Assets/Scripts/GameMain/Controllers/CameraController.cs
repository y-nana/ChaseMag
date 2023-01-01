using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // カメラ位置調整用
    [SerializeField] int marginY;       // ド真ん中よりちょっと下に


    [SerializeField]
    private float limitWidthMargin;      // 限界からどれだけ描画するか
    [SerializeField]
    private float limitHeightMargin;      // 限界からどれだけ描画するか
    [SerializeField]
    private float limitFloorMargin;

    [SerializeField]
    private Transform floor;
    [SerializeField]
    private Transform rightWall;
    [SerializeField]
    private Transform leftWall;
    [SerializeField]
    private Transform ceiling;


    private readonly int posZ = -10;           // Zの値（固定）

    // コントロール用コンポーネント
    private Transform myTransform;
    [SerializeField] Transform playerTransform; // プレイヤーの位置

    void Start()
    {
        myTransform = this.transform;

    }

    void Update()
    {
        // プレイヤのポジション取得
        Vector2 playerPos = playerTransform.position;
        Vector2 cameraPos = new Vector2(playerPos.x, playerPos.y - marginY);
        // 限界値よりはみ出していたら修正
        if (cameraPos.y < floor.position.y + limitFloorMargin) cameraPos.y = floor.position.y + limitFloorMargin;
        if (cameraPos.y > ceiling.position.y - limitHeightMargin) cameraPos.y = ceiling.position.y - limitHeightMargin;

        if (cameraPos.x > rightWall.position.x - limitWidthMargin) cameraPos.x = rightWall.position.x - limitWidthMargin;
        if (cameraPos.x < leftWall.position.x + limitWidthMargin) cameraPos.x = leftWall.position.x + limitWidthMargin;

        // 値を入れる
        myTransform.position = new Vector3(cameraPos.x, cameraPos.y, posZ);
    }
}
