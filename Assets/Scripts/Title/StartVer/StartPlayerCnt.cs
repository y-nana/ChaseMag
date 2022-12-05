using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// タイトル用 プレイヤーの挙動を制御するクラス
public class StartPlayerCnt : MonoBehaviour
{
    // 他のスクリプトで自動で走る部分を作れば
    // PlayerControllerをそのまま使える気がする

    // アクションのパラメータ
    [SerializeField] float jumpForce = 800.0f;  // ジャンプ力
    [SerializeField] float walkForce = 6.0f;    // 歩くスピード
    [SerializeField] float upForce = 10.0f;     // 壁を上るスピード

    // 無限に走らせるための位置調整用
    [SerializeField] float winSize = 11.5f;     // カメラの端

    // コントロール用コンポーネント
    private Rigidbody2D rigid2D;            // 移動
    private SpriteRenderer spriteRenderer;  // 身体の向き変更
    private Animator animator;              // アニメーション

    // 磁力をコントロールするクラス(子オブジェクト内)
    private PoleController poleCnt;

    // アニメーションのbool名
    private readonly string walk = "NowWalk";  // 歩いているか
    private readonly string jump = "NowJump";  // ジャンプ中か

    private bool actioned;  // プレイヤが移動する入力を行ったか


    void Start()
    {
        // コンポーネント取得
        this.rigid2D = GetComponent<Rigidbody2D>();
        this.animator = GetComponent<Animator>();
        this.spriteRenderer = GetComponent<SpriteRenderer>();

        this.poleCnt = transform.GetChild(0).GetComponent<PoleController>();
    }


    void Update()
    {
        // 左右移動
        int key = 0;

        // プレイヤが入力していないときは自動で右へ移動し続ける
        if (!actioned)
        {
            key = 1;
        }

        if (Input.GetAxis("Horizontal") > 0)
        {
            actioned = true;
            key = 1;
            spriteRenderer.flipX = true;
        }
        if (Input.GetAxis("Horizontal") < 0)
        {
            actioned = true;
            key = -1;
            spriteRenderer.flipX = false;

        }

        Vector2 vel = this.rigid2D.velocity;
        this.rigid2D.velocity = new Vector2(key * this.walkForce, vel.y);

        // アニメーション
        Animation();

        // カメラ外にいたら反対側から出てくるように位置を調整
        if (this.transform.position.x > winSize)
        {
            this.transform.position = new Vector2(-winSize, this.transform.position.y);
        }
        if (this.transform.position.x < -winSize)
        {
            this.transform.position = new Vector2(winSize, this.transform.position.y);
        }


    }

    // ジャンプする
    public void Jump()
    {
        // 上がっている途中にジャンプができないように
        if (this.rigid2D.velocity.y <= 0)
        {
            // 重力との相殺を防止
            this.rigid2D.velocity = new Vector2(this.rigid2D.velocity.x, 0f);
            this.rigid2D.AddForce(transform.up * this.jumpForce * poleCnt.PoleStrong);
        }
    }

    // 上に移動する(壁用)
    public void MoveUp()
    {
        this.rigid2D.velocity = new Vector2(this.rigid2D.velocity.x, upForce * poleCnt.PoleStrong);
        // 壁にくっついているので動かないようにする
        this.rigid2D.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
    }


    // 上下に移動できないようにする(壁用)
    public void FreezeY()
    {
        this.rigid2D.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    // フリーズ解除
    public void CanMove()
    {
        this.rigid2D.constraints = RigidbodyConstraints2D.FreezeRotation;

    }

    // アニメーション用
    private void Animation()
    {
        this.animator.SetBool(walk, false);
        this.animator.SetBool(jump, false);

        // 横移動のみしていたら歩く
        if (rigid2D.velocity.y == 0 && rigid2D.velocity.x != 0) this.animator.SetBool(walk, true);

        // 上下移動していたらジャンプ
        if (rigid2D.velocity.y != 0) this.animator.SetBool(jump, true);

    }

}
