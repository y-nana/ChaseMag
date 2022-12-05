using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// タイトル用 壁の挙動を管理するクラス（子オブジェクト）
public class StartWallColliderCnt : MonoBehaviour
{
    // 当たり判定取得用
    private GameObject player;
    private GameObject playerPole;

    // アクション用コンポーネント
    private StartPlayerCnt playerCnt;
    private StartWallController wallCnt;

    private readonly string playerTagName = "Player";   // プレイヤ取得用タグ名


    void Start()
    {
        player = GameObject.FindGameObjectWithTag(playerTagName);
        playerPole = player.transform.GetChild(0).gameObject;
        playerCnt = player.GetComponent<StartPlayerCnt>();
        // 親オブジェクトからコンポーネント取得
        wallCnt = transform.parent.gameObject.GetComponent<StartWallController>();
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject == player || collision.gameObject == playerPole)
        {
            if (wallCnt.getCanUp())
            {
                playerCnt.FreezeY();
            }
            else
            {
                //極の向きが変わった場合
                playerCnt.CanMove();
            }
        }
    }

}
