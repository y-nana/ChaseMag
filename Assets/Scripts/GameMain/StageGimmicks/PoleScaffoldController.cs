using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Pole;


public enum PoleScaffoldState
{
    Neutral,
    up,
    down
}

public class PoleScaffoldController : MonoBehaviour
{

    [SerializeField,Min(0.0f)]
    private float higherMove;       // 最高
    [SerializeField, Min(0.0f)]
    private float lowerMove;        // 最低
    [SerializeField, Min(0.0f)]
    private float speed;            // 動くスピード
    [SerializeField, Min(0.0f)]
    private float chaserSpeed;      // チェイサーが近づいた時のスピード
    [SerializeField, Min(0.0f)]
    private float playerSpeed;      // プレイヤーが近づいた時のスピード
    [SerializeField, Min(0.0f)]
    private float margin;           // 遊び

    private float defaultPosY;      // 初期位置


    private GameObject player;
    private GameObject chaser;

    // アクションさせる用のコンポーネント
    private PlayerController playerCnt;
    private Transform playerTransform;
    private ChaserController chaserCnt;
    private Transform chaserTransform;
    [SerializeField]
    private Transform moveTransform;    // 動かす足場
    private Rigidbody2D moveRigid;    // 動かす足場

    // 極の向きを確認する用
    private PoleController poleCnt;


    // 状態
    private PoleScaffoldState state;

    // アクションする範囲内にいるかどうか
    private bool isPlayerInArea;
    private bool isChaserInArea;

    // 取得用タグ名
    private readonly string playerTagName = "Player";   // プレイヤ
    private readonly string chaserTagName = "Chaser";   // チェイサー
    private readonly string southTagName = "South";   // S極
    private readonly string northTagName = "North";   // N極

    // sprite変更用
    private readonly string northPath = "Sprite/Stage/wallNorth";   // N極
    private readonly string southPath = "Sprite/Stage/wallSouth";   // S極


    // Start is called before the first frame update
    void Start()
    {

        //プレイヤー、チェイサータグのオブジェクトが一つであること前提
        player = GameObject.FindGameObjectWithTag(playerTagName);
        chaser = GameObject.FindGameObjectWithTag(chaserTagName);
        playerCnt = player.GetComponent<PlayerController>();
        playerTransform = player.GetComponent<Transform>();
        chaserCnt = chaser.GetComponent<ChaserController>();
        chaserTransform = chaser.transform;
        poleCnt = player.GetComponentInChildren<PoleController>();

        defaultPosY = moveTransform.position.y;
        moveRigid = moveTransform.gameObject.GetComponent<Rigidbody2D>();
        SpriteRenderer spriteRenderer = moveTransform.gameObject.GetComponent<SpriteRenderer>();

        // エディター上での名前にタグ名を追加
        tag = this.gameObject.tag;
        this.gameObject.name += tag;
        // 自身のタグを確認して、極の向きがどっちになるとジャンプできるのか判定するための変数に代入
        if (tag == southTagName)
        {
            // 自身のspriteを変える
            spriteRenderer.sprite = Resources.Load<Sprite>(southPath);
        }
        else if (tag == northTagName)
        {
            spriteRenderer.sprite = Resources.Load<Sprite>(northPath);
        }

        SetState();

    }

    // Update is called once per frame
    void Update()
    {

        switch (state)
        {
            case PoleScaffoldState.Neutral:
                // 元の位置に戻ろうとする
                if (moveTransform.position.y < defaultPosY - margin)
                {
                    moveRigid.velocity = Vector2.up * speed;
                    break;

                }
                if (moveTransform.position.y > defaultPosY + margin)
                {
                    moveRigid.velocity = Vector2.down * speed;

                    break;
                }
                moveRigid.velocity = Vector2.zero;

                break;
                // 上に上がる
            case PoleScaffoldState.up:

                if (moveTransform.position.y > defaultPosY + higherMove - margin)
                {
                    moveTransform.position = new Vector2(moveTransform.position.x, defaultPosY + higherMove);

                    moveRigid.velocity = Vector2.zero;
                    break;
                }

                moveRigid.velocity = Vector2.up * GetVelocity();

                break;
                // 下に下がる
            case PoleScaffoldState.down:

                if (moveTransform.position.y < defaultPosY-lowerMove + margin)
                {
                    moveTransform.position = new Vector2(moveTransform.position.x, defaultPosY - lowerMove);
                    moveRigid.velocity = Vector2.zero;
                    break;

                }
                moveRigid.velocity = Vector2.down * GetVelocity();

                break;

        }


    }
    private void OnTriggerStay2D(Collider2D collision)
    {

        // アクションできる位置にオブジェクトが入る
        if (collision.gameObject == player)
        {
            isPlayerInArea = true;
            SetState();

        }
        if (collision.gameObject == chaser)
        {
            isChaserInArea = true;
            SetState();
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // アクションできる位置からオブジェクトが出る
        if (collision.gameObject == player)
        {
            isPlayerInArea = false;
            SetState();

        }
        if (collision.gameObject == chaser)
        {
            isChaserInArea = false;
            SetState();
        }
    }

    // 状態をセット
    private void SetState()
    {
        // 常にチェイサーが優先される
        if (isChaserInArea)
        {
            state = GetStateForChaser();
            return;
        }
        if (isPlayerInArea)
        {
            state = GetStateForPlayer();
            return;
        }

        state = PoleScaffoldState.Neutral;

    }

    private PoleScaffoldState GetStateForChaser()
    {
        // チェイサーと床の相対座標を求める
        float relPosY = chaserTransform.position.y - moveTransform.position.y;
        // チェイサーが床より上にいるとき
        if (relPosY >= 0)
        {
            if (chaserCnt.wantToJump)
            {
                return PoleScaffoldState.up;
            }
            if (chaserCnt.wantToDown)
            {
                return PoleScaffoldState.down;
            }

        }
        else
        {
            // 下にいるとき
            if (chaserCnt.wantToJump)
            {
                return PoleScaffoldState.down;
            }

        }
        return PoleScaffoldState.Neutral;
    }

    // 自身の極の向き(タグ)とプレイヤの位置から
    // 状態を取得
    private PoleScaffoldState GetStateForPlayer()
    {
        // プレイヤーと床の相対座標を求める
        float relPosY = playerTransform.position.y - moveTransform.position.y;
        // プレイヤーが床より上にいるとき
        if (relPosY >= 0)
        {
            if (tag == southTagName)
            {
                if (poleCnt.PoleCheck((int)PoleOrientation.Up))
                {
                    return PoleScaffoldState.up;
                }
                else if (poleCnt.PoleCheck((int)PoleOrientation.Down))
                {
                    return PoleScaffoldState.down;

                }
            }
            else
            {
                if (poleCnt.PoleCheck((int)PoleOrientation.Down))
                {
                    return PoleScaffoldState.up;
                }
                else if (poleCnt.PoleCheck((int)PoleOrientation.Up))
                {
                    return PoleScaffoldState.down;

                }
            }
        }
        else
        {
            // 下にいるとき
            if (tag == southTagName)
            {
                if (poleCnt.PoleCheck((int)PoleOrientation.Down))
                {
                    return PoleScaffoldState.down;
                }
                else if (poleCnt.PoleCheck((int)PoleOrientation.Up))
                {
                    return PoleScaffoldState.up;

                }
            }
            else
            {
                if (poleCnt.PoleCheck((int)PoleOrientation.Up))
                {
                    return PoleScaffoldState.down;
                }
                else if (poleCnt.PoleCheck((int)PoleOrientation.Down))
                {
                    return PoleScaffoldState.up;

                }
            }
        }
        return PoleScaffoldState.Neutral;

    }

    // 近くの状況によって返す値を変える
    private float GetVelocity()
    {
        if (isChaserInArea)
        {
            return chaserSpeed;
        }
        if (isPlayerInArea)
        {
            return speed * poleCnt.PoleStrong * poleCnt.PoleStrong;
        }

        return speed;
    }

}
