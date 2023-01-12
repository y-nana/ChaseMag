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

    [SerializeField]
    private Transform maxTransform; // 最高
    [SerializeField]
    private Transform minTransform; // 最低
    [SerializeField, Min(0.0f)]
    private float speed;            // 動くスピード
    [SerializeField, Min(0.0f)]
    private float margin;           // 遊び

    private float defaultPosY;     // 初期位置


    private GameObject player;
    private GameObject chaser;

    // アクションさせる用のコンポーネント
    private PlayerController playerCnt;
    private Transform playerTransform;
    private ChaserController chaserCnt;
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
        //chaser = GameObject.FindGameObjectWithTag(chaserTagName);
        playerCnt = player.GetComponent<PlayerController>();
        playerTransform = player.GetComponent<Transform>();
        //chaserCnt = chaser.GetComponent<ChaserController>();
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
        Debug.Log(state);
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

            case PoleScaffoldState.up:

                if (moveTransform.position.y > maxTransform.position.y - margin)
                {
                    moveTransform.position = new Vector2(moveTransform.position.x, maxTransform.position.y);

                    moveRigid.velocity = Vector2.zero;
                    break;
                }
                moveRigid.velocity = Vector2.up * speed;

                break;
            case PoleScaffoldState.down:

                if (moveTransform.position.y < minTransform.position.y + margin)
                {
                    moveTransform.position = new Vector2(moveTransform.position.x, minTransform.position.y);
                    moveRigid.velocity = Vector2.zero;
                    break;

                }
                moveRigid.velocity = Vector2.down * speed;

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
        }
    }

    // 状態をセット
    private void SetState()
    {
        if (isPlayerInArea)
        {
            state = GetScaffoldState();
            return;
        }

        state = PoleScaffoldState.Neutral;

    }

    // 自身の極の向き(タグ)とプレイヤの位置から
    // 状態を取得
    private PoleScaffoldState GetScaffoldState()
    {
        // プレイヤーと床の相対座標を求める
        float relPosY = playerTransform.position.y - moveTransform.position.y;
        // プレイヤーが床より上にいるとき
        if (relPosY <= 0)
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
        }
        return PoleScaffoldState.Neutral;

    }

}
