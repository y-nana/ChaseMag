using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 壁の頂上で止まる挙動を制御するクラス（子オブジェクト）
public class WallColliderController : MonoBehaviour
{

    // 当たり判定取得用
    private GameObject player;
    private GameObject playerPole;

    // アクション用コンポーネント
    private PlayerController playerCnt;
    private WallController wallCnt;

    // 取得用タグ名
    private readonly string playerTagName = "Player";   // プレイヤ

    void Start()
    {
        player = GameObject.FindGameObjectWithTag(playerTagName);
        playerPole = player.transform.GetChild(0).gameObject;
        playerCnt = player.GetComponent<PlayerController>();
        // 親オブジェクトからコンポーネント取得
        wallCnt = transform.parent.gameObject.GetComponent<WallController>();
    }


    //壁の一番上に到達したら、極の向きが変更されるまで動きを止めたい(現在はプレイヤのみ)
    private void OnTriggerStay2D(Collider2D collision)
    {
        // プレイヤーが入ってきたら
        if (collision.gameObject == player || collision.gameObject == playerPole)
        {
            // フリーズさせる
            if (wallCnt.getCanUp())
            {
                playerCnt.FreezeY();
            }
            else
            {
                //極の向きが変わった場合フリーズ解除
                playerCnt.CanMove();
            }
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // プレイヤーが出て行ったらフリーズ解除
        if (collision.gameObject ==  player)
        {
            playerCnt.CanMove();
        }
        
    }

}
