using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class ChaserController : MonoBehaviour
{

    // アクションのパラメータ
    [SerializeField] private float jumpForce;  // ジャンプ力
    [SerializeField] private float walkForce;    // 歩くスピード
    [SerializeField] private float upForce;     // 壁を上るスピード


    // コントロール用コンポーネント
    private Rigidbody2D rigid2D;            // 移動
    private SpriteRenderer spriteRenderer;  // 身体の向き変更
    private Animator animator;              // アニメーション
    [SerializeField] private GameObject sceneDirector;        // シーン遷移用
    private AudioSource audioSource;        // サウンド
    private Transform m_transform;          // トランスフォーム

    // アニメーションのbool名
    private readonly string walk = "NowWalk";  // 歩いているか
    private readonly string jump = "NowJump";  // ジャンプ中か

    // 対プレイヤ用
    [SerializeField]
    private RouteManager routeManager;

    [SerializeField] private GameObject player;      // プレイヤオブジェクト
    [SerializeField] private GameObject playerPole;  // 当たり判定取得用
    private Transform pTransform;

    [SerializeField] private float getPosTime;   // プレイヤの位置を取得する間隔
    [SerializeField] private float waitTime;     // 自身が停止しているのを待つ時間
    [SerializeField] private float stopDistance;// プレイヤが留まっていると判定する距離
    [SerializeField] private Vector2 margin = 
        new Vector2(0.5f, 2.0f);                // 震え防止の余白

    

    private List<Transform> route;
    private int routeIndex;

    private float getPosTimer;      // プレイヤの位置を取得する間隔のタイマー
    private Vector2 distance;       // プレイヤとの距離

    private bool playerIsRight;     // プレイヤが右にいるか(自分が左から追っている状態か)
    
    private float waitTimer;        // 自身が停止している間


    private bool isUseRoute;
    private bool wantToJump;

    private Vector2 pPos;       // プレイヤーの位置

    // 改善
    [SerializeField]
    private float toPlayerTime;
    private float toPlayerTimer;
    [SerializeField]
    private float pMoveValue;

    private Vector2 prePlayerPos;


    // 効果音
    [SerializeField]
    private AudioClip jumpSE;

    // 効果音
    [SerializeField]
    private AudioClip upSE;



    void Start()
    {
        // コンポーネント取得
        rigid2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        this.audioSource = GetComponent<AudioSource>();
        m_transform = transform;

        //改善
        toPlayerTimer = 0.0f;
        pTransform = player.transform;
        //値の初期化
        waitTimer = 0.0f;
        getPosTimer = 0.0f;
        SearchRoute();



    }


    void Update()
    {

        /*
        // プレイヤとの距離取得
        // ルートを使っていない状態で一定時間たったらルート検索
        if (!isUseRoute)
        {
            getPosTimer += Time.deltaTime;
            if (getPosTimer > getPosTime)
            {
                Debug.Log("時間経過により探索を開始");
                getPosTimer = 0.0f;
                SearchRoute();
                DistanceUpdate();
            }

        }

        // 左右移動
        int key = 0;
        Vector2 goalDir = Vector2.zero;
        //ルートを使うかどうか
        if (isUseRoute)//&&routeIndex < route.Count)
        {
            goalDir = route[routeIndex].position - m_transform.position;

            // 目標ポイントまでの距離が近かったら次のポイントを目標にする
            if (Vector2.Distance(route[routeIndex].position, m_transform.position) < 0.75f)
            {
                Debug.Log(route[routeIndex].gameObject.name + "を通過しました");
                routeIndex++;
                
                if (routeIndex >= route.Count)
                {
                    //SearchRoute();
                    Debug.Log("ルートが終了したので自分で追います");
                    isUseRoute = false;
                    DistanceUpdate();

                }
            }


        }
        // プレイヤーへの方向をそのまま使う
        if(!isUseRoute)
        {
            if (playerIsRight)
            {

                goalDir = new Vector2(distance.x + margin.x,distance.y);
                // 自分がプレイヤと余白分以上右にいたら
                if (pPos.x + margin.x < m_transform.position.x)
                {
                    DistanceUpdate();
                }

            }
            // 右から追っている
            else
            {
                goalDir = new Vector2(distance.x - margin.x, distance.y);
                // 自分がプレイヤと余白分以上左にいたら
                if (pPos.x - margin.x > m_transform.position.x)
                {
                    DistanceUpdate();
                }
            }
        }

        */

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

        // 速度に代入
        Vector2 vel = this.rigid2D.velocity;
        this.rigid2D.velocity = new Vector2(key * this.walkForce, vel.y);

        /*
        // 動きが停止した状態が続いたら経路探索を行う
        if (Mathf.Approximately((rigid2D.velocity.magnitude), 0.0f))
        {
            waitTimer += Time.deltaTime;
            if (waitTime < waitTimer)
            {
                waitTimer = 0.0f;
                Debug.Log("止まってるよ");
                SearchRoute();

            }
        }
        */
        // アニメーション
        Animation();


       
    }


    private Vector2 ToPlayer()
    {
        Vector2 toPos = Vector2.zero;
        toPlayerTimer += Time.deltaTime;
        // 一定時間プレイヤーを追っていたら探索を開始
        if (toPlayerTimer > toPlayerTime)
        {
            toPlayerTimer = 0.0f;
            SearchRoute();
        }
        // プレイヤーまでのベクトルを入れる
        toPos = pTransform.position - m_transform.position;

        return toPos;

    }

    private Vector2 UseRoute()
    {

        // 今の目標地点についた
        if (Vector2.Distance(route[routeIndex].position, m_transform.position) < 0.75f)
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
                        return route[routeIndex].position - m_transform.position;
                    }
                    isUseRoute = false;
                    return ToPlayer();
                }
            }
            return route[routeIndex].position-m_transform.position;
        }

        // 目標地点へついていない
        waitTimer += Time.deltaTime;
        if (waitTimer > waitTime)
        {
            Debug.Log("着かなすぎるので検査");
            waitTimer = 0.0f;
            if (routeManager.GetNearlyPointTransform(m_transform.position) == route[routeIndex])
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
            return route[routeIndex].position - m_transform.position;

        }
        return ToPlayer();
    }


    // プレイヤーが前回の探索から規定値以上動いているかどうか
    private bool IsPlayerAlotMove()
    {
        return Vector2.Distance(prePlayerPos, pTransform.position)> pMoveValue||
            route[route.Count-1] != routeManager.GetNearlyPointTransform(pTransform.position);
    }

    

    // プレイヤとの距離の距離の更新
    private void DistanceUpdate()
    {
        Vector2 myPos = m_transform.position;
        pPos = player.transform.position;
        distance =  pPos - myPos;
        playerIsRight = distance.x > 0;

    }



    // ジャンプする
    public void Jump()
    {
        if (this.rigid2D.velocity.y <= 0 && wantToJump)
        {
            this.rigid2D.velocity = new Vector2(this.rigid2D.velocity.x, 0f);
            this.rigid2D.AddForce(m_transform.up * this.jumpForce);
            audioSource.PlayOneShot(jumpSE);

        }
    }

    // 上に移動する(壁用)
    public void MoveUp()
    {
        if (!audioSource.isPlaying)
        {
            //audioSource.Play(upSE);

        }

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
        DistanceUpdate();
        route = routeManager.GetRoute(m_transform, pTransform);
        prePlayerPos = pTransform.position;
        routeIndex = 0;
        Debug.Log("今回のルートは");
        if (route == null)
        {
            Debug.Log("ないです自分で頑張ってください");

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
    // デバッグ用
    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
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
                // プレイヤーへのベクトルを表示
                // 右向きなら白、左なら緑色
                if (playerIsRight)
                {
                    Gizmos.color = Color.white;
                }
                else
                {
                    Gizmos.color = Color.green;
                }
                Gizmos.DrawRay(m_transform.position, pTransform.position - m_transform.position);

            }
        }

    }


#endif
}
