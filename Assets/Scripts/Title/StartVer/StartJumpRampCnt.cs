using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Pole;

// タイトル用 ジャンプ台の挙動を制御するクラス
public class StartJumpRampCnt : MonoBehaviour
{

    private GameObject player;

    // アクションさせる用のコンポーネント
    private StartPlayerCnt playerCnt;

    // 極の向きを確認する用
    private PoleController poleCnt;

    private int orientation;    // アクションする極の向き

    // オブジェクトがアクションできる位置にいるか
    private bool playerCanJump;

    // 取得用タグ名
    private readonly string playerTagName = "Player";   // プレイヤ
    private readonly string southTagName = "South";   // S極
    private readonly string northTagName = "North";   // N極

    // sprite変更用
    private readonly string southPath = "Sprite/Stage/jumpRampSouth";   // S極
    private readonly string northPath = "Sprite/Stage/jumpRampNorth";   // N極


    void Start()
    {
        player = GameObject.FindGameObjectWithTag(playerTagName);
        playerCnt = player.GetComponent<StartPlayerCnt>();
        poleCnt = player.GetComponentInChildren<PoleController>();
        SpriteRenderer spriteRenderer = this.GetComponent<SpriteRenderer>();
        // 自身のタグを確認して、極の向きがどっちになるとジャンプできるのか判定するための変数に代入
        tag = this.gameObject.tag;
        this.gameObject.name += tag;
        if (tag == southTagName)
        {
            spriteRenderer.sprite = Resources.Load<Sprite>(southPath);
            orientation = (int)PoleOrientation.Down;
        }
        else if (tag == northTagName)
        {
            spriteRenderer.sprite = Resources.Load<Sprite>(northPath);
            orientation = (int)PoleOrientation.Up;
        }

    }

    void Update()
    {
        // アクションできる位置にいて極の向きが正解
        if (playerCanJump && poleCnt.PoleCheck(orientation))
        {
            playerCnt.Jump();
            playerCanJump = false;

        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        playerCanJump = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        playerCanJump = false;

    }
}
