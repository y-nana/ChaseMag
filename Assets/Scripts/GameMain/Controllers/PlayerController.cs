using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    // アクションのパラメータ
    [SerializeField] private float jumpForce = 800.0f;  // ジャンプ力
    [SerializeField] private float walkForce = 8.0f;    // 歩くスピード
    [SerializeField] private float upForce = 10.0f;     // 壁を上るスピード


    // コントロール用コンポーネント
    private Rigidbody2D rigid2D;            // 移動
    private SpriteRenderer spriteRenderer;  // 身体の向き変更
    private Animator animator;              // アニメーション
    private AudioSource audioSource;        // サウンド

    // 磁力をコントロールするクラス(子オブジェクト内)
    [SerializeField] PoleController poleCnt;

    // アニメーションのbool名
    private readonly string walk = "NowWalk";  // 歩いているか
    private readonly string jump = "NowJump";  // ジャンプ中か

    // 効果音
    [SerializeField]
    private AudioClip jumpSE;

    // 効果音
    [SerializeField]
    private AudioClip upSE;

    void Start()
    {
        // コンポーネント取得
        this.rigid2D = GetComponent<Rigidbody2D>();
        this.animator = GetComponent<Animator>();
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        this.audioSource = GetComponent<AudioSource>();

        //this.poleCnt = transform.GetChild(0).GetComponent<PoleController>();
    }


    void Update()
    {

        // 左右移動
        int key = 0;
        //if (!PauseManager.nowPause)
        if (GameStateManager.instance.IsInputtable())
        {
            // 右
            if (Input.GetKey(KeyCode.D) || Input.GetAxis("Horizontal") > 0)
            {
                key = 1;
                // 身体の向きを右に
                this.spriteRenderer.flipX = true;
            }
            // 左
            if (Input.GetKey(KeyCode.A) || Input.GetAxis("Horizontal") < 0)
            {
                key = -1;
                // 身体の向きを左に
                this.spriteRenderer.flipX = false;
            }
        }
        Vector2 vel = this.rigid2D.velocity;
        this.rigid2D.velocity = new Vector2(key * this.walkForce, vel.y);

        // アニメーション
        Animation();

    }


    // ジャンプする
    public void Jump()
    {
        // 上がっている途中にジャンプができないように
        if (this.rigid2D.velocity.y <= 0 )
        {
            // 重力との相殺を防止
            this.rigid2D.velocity = new Vector2(this.rigid2D.velocity.x, 0f);
            this.rigid2D.AddForce(transform.up * this.jumpForce * poleCnt.PoleStrong);
            audioSource.PlayOneShot(jumpSE);
        }
    }

    // 上に移動する(壁用)
    public void MoveUp()
    {

        if (!audioSource.isPlaying)
        {
            //audioSource.PlayOneShot(upSE);

        }

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
