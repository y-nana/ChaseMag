using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class ChaserController : MonoBehaviour
{

    // アクションのパラメータ
    [SerializeField] private float jumpForce = 800.0f;  // ジャンプ力
    [SerializeField] private float walkForce = 8.0f;    // 歩くスピード
    [SerializeField] private float upForce = 10.0f;     // 壁を上るスピード


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

    [SerializeField] private float getPosTime = 0.5f;   // プレイヤの位置を取得する間隔
    [SerializeField] private float waitTime;     // 自身が停止しているのを待つ時間
    [SerializeField] private float stopDistance = 0.1f;// プレイヤが留まっていると判定する距離
    [SerializeField] private Vector2 margin = 
        new Vector2(0.5f, 2.0f);                // 震え防止の余白

    

    private List<Transform> route;
    private int routeIndex;

    private float getPosTimer;      // プレイヤの位置を取得する間隔のタイマー
    private Vector2 distance;       // プレイヤとの距離
    private Vector2 playerPrePos;   // プレイヤの前フレームの位置

    private bool playerIsRight;     // プレイヤが右にいるか(自分が左から追っている状態か)
    
    private float waitTimer;        // 自身が停止している間

    private Vector2 playerDir;              // プレイヤーへの向き



    private bool isCoolTime;            // 連続して経路探索しないように
    private bool isUseRoute;
    private bool wantToJump;


    // 壁用
    private Vector2 pPos;
    private Vector2 prePos;
    private Vector2 MyPos;

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


        //値の初期化
        playerPrePos = new Vector2(0, 0);
        prePos = new Vector2(0, 0);
        waitTimer = 0.0f;
        getPosTimer = 0.0f;
        SearchRoute();

    }


    void Update()
    {

        
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
                /*
                // プレイヤを余白分通りすぎるようにする処理
                if (distance.x > -margin.x)
                {
                    key = 1;
                    spriteRenderer.flipX = true;
                }
                else 
                */
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
                /*
                if (distance.x < margin.x)
                {
                    key = -1;
                    spriteRenderer.flipX = false;
                }
                else DirUpdate();
                */
                goalDir = new Vector2(distance.x - margin.x, distance.y);
                // 自分がプレイヤと余白分以上左にいたら
                if (pPos.x - margin.x > m_transform.position.x)
                {
                    DistanceUpdate();
                }
            }
        }


        // 目標ポイントへ左右移動
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




        // 目標がジャンプしないと届かない位置か
        wantToJump = goalDir.y > margin.y;


        Vector2 vel = this.rigid2D.velocity;
        this.rigid2D.velocity = new Vector2(key * this.walkForce, vel.y);

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

        // アニメーション
        Animation();


       
    }

    private void OnDrawGizmos()
    {
        if (isUseRoute)
        {
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
            if (playerIsRight)
            {
                Gizmos.color = Color.white;

            }
            else
            {
                Gizmos.color = Color.green;
            }
            Gizmos.DrawRay(m_transform.position, distance);
                
        }


    }

    //プレイヤへの方向の更新
    private void DirUpdate()
    {
        MyPos = m_transform.position;
        pPos = player.transform.position;
        playerDir = pPos - MyPos;
        playerIsRight = playerDir.x > 0;
        playerDir = pPos - MyPos;


    }

    // プレイヤとの距離の距離の更新
    private void DistanceUpdate()
    {
        MyPos = m_transform.position;
        pPos = player.transform.position;
        distance =  pPos - MyPos;
        playerIsRight = playerDir.x > 0;
        playerDir = pPos - MyPos;
    }


    // プレイヤとの相対位置を求める
    private void GetToPlayer()
    {
        MyPos = m_transform.position;
        pPos = player.transform.position;
        playerDir = pPos - MyPos;

        //Turn();
        playerIsRight = distance.x > 0;


    }

    //プレイヤがどっち向きにいるか
    private void GoToPlayer()
    {
        playerIsRight = distance.x > 0;
    }


    // ジャンプする
    public void Jump()
    {
        if (this.rigid2D.velocity.y == 0 && wantToJump)
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

    private void Turn()
    {

        if (prePos.x == MyPos.x)
        {
            playerIsRight = !playerIsRight;
        }

        prePos = MyPos;
        
    }

    private void SearchRoute()
    {
        Debug.Log("探索開始");
        DistanceUpdate();
        route = routeManager.GetRoute(m_transform, player.transform);
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

}
