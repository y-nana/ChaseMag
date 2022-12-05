using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// タイトル用 鬼の挙動を制御するクラス
public class StartChaserCnt : MonoBehaviour
{

    [SerializeField] float walkForce = 4.0f;    // 歩くスピード

    // コントロール用コンポーネント
    private Rigidbody2D rigid2D;            // 移動
    private SpriteRenderer spriteRenderer;  // 身体の向き変更
    private Animator animator;              // アニメーション

    // 対プレイヤ用
    private GameObject player;      // プレイヤオブジェクト
    private Vector2 distance;       // プレイヤとの距離
    private readonly string playerTagName = "Player";  // 取得用タグ名

    // アニメーションのbool名
    private readonly string walk = "NowWalk";  // 歩いているか

    void Start()
    {
        // コンポーネント取得
        rigid2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        // プレイヤ取得
        player = GameObject.FindGameObjectWithTag(playerTagName);
    }

    void Update()
    {
        // プレイヤとの距離取得
        distance = GetToPlayer();

        // 左右移動
        int key = 0;
        if (distance.x > 0)
        {
            key = 1;
            spriteRenderer.flipX = true;
        }
        else
        {
            key = -1;
            spriteRenderer.flipX = false;

        }
        this.rigid2D.velocity = new Vector2(key * this.walkForce, 0);

        // アニメーション
        Animation();

    }

    // プレイヤとの相対位置を求める
    private Vector2 GetToPlayer()
    {
        Vector2 MyPos = transform.position;
        Vector2 pPos = player.transform.position;
        Vector2 distance = pPos - MyPos;

        return distance;
    }


    // アニメーション用
    private void Animation()
    {
        this.animator.SetBool(walk, false);

        // 横移動していたら歩く
        if (rigid2D.velocity.x != 0) this.animator.SetBool(walk, true);

    }

}
