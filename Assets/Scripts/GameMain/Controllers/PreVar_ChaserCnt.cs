using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// 鬼のスクリプト 前のバージョン
public class PreVar_ChaserCnt : MonoBehaviour
{

    // アクションのパラメータ
    [SerializeField] private float jumpForce = 800.0f;  // ジャンプ力
    [SerializeField] private float walkForce = 8.0f;    // 歩くスピード
    [SerializeField] private float upForce = 10.0f;     // 壁を上るスピード


    // コントロール用コンポーネント
    private Rigidbody2D rigid2D;            // 移動
    private SpriteRenderer spriteRenderer;  // 身体の向き変更
    private Animator animator;              // アニメーション
    [SerializeField] private SceneDirector sceneDirector;        // シーン遷移用
    private AudioSource audioSource;        // サウンド


    // アニメーションのbool名
    private readonly string walk = "NowWalk";  // 歩いているか
    private readonly string jump = "NowJump";  // ジャンプ中か

    // 対プレイヤ用
    [SerializeField] private GameObject player;      // プレイヤオブジェクト
    [SerializeField] private GameObject playerPole;  // 当たり判定取得用

    [SerializeField] private float getPosTime = 0.5f;   // プレイヤの位置を取得する間隔
    [SerializeField] private float waitTime = 5.0f;     // プレイヤが停止しているのを待つ時間
    [SerializeField] private float stopDistance = 0.1f;// プレイヤが留まっていると判定する距離
    [SerializeField] private float throughTime = 0.5f;  // 何秒足場を透ける状態にするか
    [SerializeField]
    private Vector2 margin =
        new Vector2(0.5f, 2.0f);                // 震え防止の余白

    private float getPosTimer;      // プレイヤの位置を取得する間隔のタイマー
    private Vector2 distance;       // プレイヤとの距離
    private Vector2 playerPrePos;   // プレイヤの前フレームの位置

    private bool playerIsRight;     // プレイヤが右にいるか(自分が左から追っている状態か)
    private bool isThrough;         // 足場を透ける

    private float waitTimer;        // プレイヤが停止している間
    private float throughTimer;     // 足場が透ける状態の間

    // 壁用
    private Vector2 pPos;
    private Vector2 prePos;
    private Vector2 MyPos;

    // 効果音
    [SerializeField]
    private AudioClip jumpSE;



    // レイヤの名前と対応するint
    private enum LayerName
    {
        Default = 0,    // デフォルトレイヤ
        Through = 8,    // 床以外すり抜けるレイヤ
    }

    void Start()
    {
        // コンポーネント取得
        rigid2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        this.audioSource = GetComponent<AudioSource>();



        //値の初期化
        playerPrePos = new Vector2(0, 0);
        prePos = new Vector2(0, 0);
        waitTimer = 0.0f;
        throughTimer = 0.0f;
        getPosTimer = 0.0f;

    }


    void Update()
    {

        // プレイヤとの距離取得
        if (getPosTimer > getPosTime)
        {
            getPosTimer = 0.0f;
            distance = GetToPlayer();
        }
        getPosTimer += Time.deltaTime;


        float playerDis = Vector2.Distance(playerPrePos, pPos);
        // プレイヤが安置に停止していないか
        if (playerDis < stopDistance)
        {
            waitTimer += Time.deltaTime;
        }
        else playerPrePos = pPos;

        // 左右移動
        int key = 0;
        // 左から追っている状態
        if (playerIsRight)
        {
            // プレイヤを余白分通りすぎるようにする処理
            if (distance.x > -margin.x)
            {
                key = 1;
                spriteRenderer.flipX = true;
            }
            else GoToPlayer();

        }
        // 右から追っている
        else
        {
            if (distance.x < margin.x)
            {
                key = -1;
                spriteRenderer.flipX = false;
            }
            else GoToPlayer();
        }

        // 一定時間以上プレイヤが動かなくなったら
        if (waitTimer > waitTime)
        {
            waitTimer = 0f;
            // 真下で待機された時の対応
            Through(distance);
            // 上で待機された時の対応 未
        }

        // 足場透過状態
        if (isThrough)
        {
            // 左右移動はしない
            key = 0;
            throughTimer -= Time.deltaTime;
            if (throughTimer < 0)
            {
                isThrough = false;
                this.gameObject.layer = (int)LayerName.Default;
            }
        }

        Vector2 vel = this.rigid2D.velocity;
        this.rigid2D.velocity = new Vector2(key * this.walkForce, vel.y);

        // アニメーション
        Animation();

    }


    // プレイヤとの相対位置を求める
    private Vector2 GetToPlayer()
    {
        MyPos = transform.position;
        pPos = player.transform.position;
        Vector2 distance = pPos - MyPos;

        Turn();

        return distance;
    }

    //プレイヤがどっち向きにいるか
    private void GoToPlayer()
    {
        playerIsRight = distance.x > 0;
    }

    // 足場透過
    private void Through(Vector2 distance)
    {

        // プレイヤが下にいたら
        if (distance.y < 0 && distance.x < stopDistance && distance.x > -stopDistance)
        {

            isThrough = true;
            throughTimer = throughTime;
            // レイヤ変更により足場を透過
            this.gameObject.layer = (int)LayerName.Through;
        }

    }

    // ジャンプする
    public void Jump()
    {
        if (this.rigid2D.velocity.y == 0 && distance.y > margin.y)
        {
            this.rigid2D.velocity = new Vector2(this.rigid2D.velocity.x, 0f);
            this.rigid2D.AddForce(transform.up * this.jumpForce);
            audioSource.PlayOneShot(jumpSE);

        }
    }

    // 上に移動する(壁用)
    public void MoveUp()
    {
 
        this.rigid2D.velocity = Vector2.up * this.upForce;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // プレイヤーに接触したらゲームオーバーへ遷移
        if (collision.gameObject == player || collision.gameObject == playerPole)
        {
            sceneDirector.ToGameOver();
        }

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

    private void Turn()
    {

        if (prePos.x == MyPos.x)
        {
            playerIsRight = !playerIsRight;
        }

        prePos = MyPos;

    }

}
