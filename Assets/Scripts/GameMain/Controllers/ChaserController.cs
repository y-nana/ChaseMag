using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class ChaserController : MonoBehaviour
{

    // アクションのパラメータ
    [SerializeField] private float jumpForce;   // ジャンプ力
    [SerializeField] private float walkForce;   // 歩くスピード
    [SerializeField] private float upForce;     // 壁を上るスピード


    // コントロール用コンポーネント
    private Rigidbody2D rigid2D;            // 移動
    private SpriteRenderer spriteRenderer;  // 身体の向き変更
    private Animator animator;              // アニメーション
    [SerializeField] 
    private GameObject sceneDirector;        // シーン遷移用
    private AudioSource audioSource;        // サウンド
    private Transform myTransform;          // トランスフォーム

    // アニメーションのbool名
    private readonly string walk = "NowWalk";  // 歩いているか
    private readonly string jump = "NowJump";  // ジャンプ中か

    // 対プレイヤ用

    [SerializeField] 
    private GameObject player;      // プレイヤオブジェクト
    [SerializeField] 
    private GameObject playerPole;  // 当たり判定取得用
    private Transform pTransform;   // プレイヤーのトランスフォーム

    [SerializeField]
    private RouteManager routeManager;          // 経路探索を行うスクリプト

    private List<Transform> route;              // 現在たどっているルート
    private int routeIndex;                     // 現在向かっているポイントを示す添え字

    [SerializeField] private float waitTime;     // 自身が停止しているのを待つ時間
    private float waitTimer;        

    [SerializeField] private Vector2 margin = 
    new Vector2(0.5f, 2.0f);                // 震え防止の余白

    private bool isUseRoute;    // ルートを使っていたらtrue
    public bool wantToJump { get; private set; }   // ジャンプが必要かどうか
    public bool wantToDown { get; private set; }   // 下に行きたいかどうか


    [SerializeField]
    private float toPlayerTime;
    private float toPlayerTimer;
    [SerializeField]
    private float pMoveValue;

    private Vector2 prePlayerPos;


    // 効果音
    [SerializeField]
    private AudioClip jumpSE;




    void Start()
    {
        // コンポーネント取得
        rigid2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        this.audioSource = GetComponent<AudioSource>();
        myTransform = transform;

        toPlayerTimer = 0.0f;
        pTransform = player.transform;
        waitTimer = 0.0f;
        SearchRoute();



    }


    void Update()
    {

        int key = 0;
        Vector2 goalDir = Vector2.zero;
        if (isUseRoute)
        {
            goalDir = UseRoute();
        }
        else
        {
            goalDir = ToPlayer();
        }

        // 目標ポイントへの左右の向きを判定
        if (Mathf.Abs(goalDir.x) > 0.1f)
        {
            if (goalDir.x < 0)
            {
                key = -1;
                spriteRenderer.flipX = false;

            }
            else
            {
                key = 1;
                spriteRenderer.flipX = true;

            }
        }


        // 目標がジャンプしないと届かない位置ならジャンプを行う
        wantToJump = goalDir.y > margin.y;
        wantToDown = goalDir.y < -margin.y;
        // 速度に代入
        Vector2 vel = this.rigid2D.velocity;
        this.rigid2D.velocity = new Vector2(key * this.walkForce, vel.y);

        // アニメーション
        Animation();


       
    }

    // プレイヤーへのベクトルを取得
    private Vector2 ToPlayer()
    {
        Vector2 toPos;
        toPlayerTimer += Time.deltaTime;
        // 一定時間プレイヤーを追っていたら探索を開始
        if (toPlayerTimer > toPlayerTime)
        {
            toPlayerTimer = 0.0f;
            SearchRoute();
        }
        // プレイヤーまでのベクトルを入れる
        toPos = pTransform.position - myTransform.position;

        return toPos;

    }

    // ルートのポイントへのベクトルを取得
    private Vector2 UseRoute()
    {

        // 今の目標地点についた
        if (Vector2.Distance(route[routeIndex].position, myTransform.position) < 0.75f)
        {
            Debug.Log(route[routeIndex].gameObject.name + "着いた！");
            waitTimer = 0.0f;

            if (IsPlayerAlotMove())
            {
                Debug.Log("プレイヤーが移動しているので再検索します");
                SearchRoute();
            }
            else
            {
                Debug.Log("次のポイントへ");
                routeIndex++;
                // ルートの最後まで行っていたら
                if (routeIndex >= route.Count)
                {
                    Debug.Log("ルートの終了プレイヤーを追います");
                    if (route[route.Count - 1] != routeManager.GetNearlyPointTransform(pTransform.position))
                    {
                        Debug.Log("ゴールがちょっと違う");
                        SearchRoute();
                        return route[routeIndex].position - myTransform.position;
                    }
                    isUseRoute = false;
                    return ToPlayer();
                }
            }
            return route[routeIndex].position-myTransform.position;
        }

        // 目標地点へついていない
        waitTimer += Time.deltaTime;
        if (waitTimer > waitTime)
        {
            Debug.Log("着かなすぎるので検査");
            waitTimer = 0.0f;
            if (routeManager.GetNearlyPointTransform(myTransform.position) == route[routeIndex])
            {
                Debug.Log("何かがおかしいのでそのままプレイヤーを追ってみます");
                isUseRoute = false;
                return ToPlayer();

            }

            Debug.Log("スタートが違うので経路探索します");
            SearchRoute();

        }

        if (isUseRoute)
        {
            return route[routeIndex].position - myTransform.position;

        }
        return ToPlayer();
    }


    // プレイヤーが前回の探索から規定値以上動いているかどうか
    private bool IsPlayerAlotMove()
    {
        return Vector2.Distance(prePlayerPos, pTransform.position)> pMoveValue||
            route[route.Count-1] != routeManager.GetNearlyPointTransform(pTransform.position);
    }

    
    // ジャンプする
    public void Jump()
    {
        if (this.rigid2D.velocity.y <= 0 && wantToJump)
        {
            this.rigid2D.velocity = new Vector2(this.rigid2D.velocity.x, 0f);
            this.rigid2D.AddForce(myTransform.up * this.jumpForce);
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
            ExecuteEvents.Execute<SceneCaller>(
                target: sceneDirector,
                eventData: null,
                functor: (reciever, eventData) => reciever.ToGameOver()
                );
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

    // 経路探索をする
    public void SearchRoute()
    {
        Debug.Log("探索開始");
        route = routeManager.GetRoute(myTransform, pTransform);
        prePlayerPos = pTransform.position;
        routeIndex = 0;
        Debug.Log("今回のルートは");
        if (route == null)
        {
            Debug.Log("ないです");

            isUseRoute = false;
            return;

        }
        foreach (var item in route ?? new List<Transform>())
        {
            Debug.Log(item.gameObject.name);
        }
        // 目標がなかったら直接プレイヤーを追う
        isUseRoute = route.Count >= 1;
    }


#if UNITY_EDITOR
    // デバッグ用 鬼の経路または目標地点までのベクトルをレイで表示
    private void OnDrawGizmos()
    {
        if (Application.isPlaying && this.enabled)
        {
            if (isUseRoute)
            {
                // 経路を赤色でシーンビューに表示
                Gizmos.color = Color.red;
                float arrowAngle = 20;
                float arrowLength = 0.75f;

                for (int i = 0; i < route.Count - 1; i++)
                {
                    Gizmos.DrawRay(route[i].transform.position, route[i + 1].position - route[i].transform.position);
                    // 矢印追加
                    Vector2 dir = route[i].transform.position - route[i + 1].position;
                    dir = dir.normalized * arrowLength;
                    Vector2 right = Quaternion.Euler(0, 0, arrowAngle) * dir;
                    Vector2 left = Quaternion.Euler(0, 0, -arrowAngle) * dir;
                    Gizmos.DrawRay(route[i + 1].position, right);
                    Gizmos.DrawRay(route[i + 1].position, left);
                }
            }
            else
            {
                // プレイヤーへのベクトルを表示（緑色）

                Gizmos.color = Color.green;
                Gizmos.DrawRay(myTransform.position, pTransform.position - myTransform.position);

            }
        }

    }


#endif
}
