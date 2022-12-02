using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Pole;

public class WallController : MonoBehaviour
{

    private GameObject player;
    private GameObject chaser;

    // アクションさせる用のコンポーネント
    private PlayerController playerCnt;
    private ChaserController chaserCnt;

    // 極の向きを確認する用
    private PoleController poleCnt;

    private AudioSource audioSource;        // サウンド


    // 効果音
    [SerializeField]
    private AudioClip upSE;

    //private GameObject[] moveObjects;
    // コントローラもインタフェースか継承使えば配列にできる？

    // ↓いらない？
    //private string tag;

    // 取得用タグ名
    private readonly string playerTagName = "Player";   // プレイヤ
    private readonly string chaserTagName = "Chaser";   // チェイサー
    private readonly string southTagName = "South";   // S極
    private readonly string northTagName = "North";   // N極

    // sprite変更用
    private readonly string northPath = "Sprite/Stage/wallNorth";   // N極
    private readonly string southPath = "Sprite/Stage/wallSouth";   // S極

    void Start()
    {
        //プレイヤー、チェイサータグのオブジェクトが一つであること前提
        player = GameObject.FindGameObjectWithTag(playerTagName);
        chaser = GameObject.FindGameObjectWithTag(chaserTagName);
        playerCnt = player.GetComponent<PlayerController>();
        chaserCnt = chaser.GetComponent<ChaserController>();
        poleCnt = player.GetComponentInChildren<PoleController>();
        this.audioSource = GetComponent<AudioSource>();

        // 自身のタグを確認して、自身のspriteを変える
        tag = this.gameObject.tag;
        this.gameObject.name += tag;
        SpriteRenderer spriteRenderer = this.GetComponent<SpriteRenderer>();
        if (tag == southTagName)
        {
            spriteRenderer.sprite = Resources.Load<Sprite>(southPath);
        }
        else if (tag == northTagName)
        {
            spriteRenderer.sprite = Resources.Load<Sprite>(northPath);
        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {        
        // アクションできる位置にオブジェクトが存在する
        if (collision.gameObject == player)
        {
            if (poleCnt.PoleCheck(GetOrientation()))
            {
                playerCnt.MoveUp();
                if (!audioSource.isPlaying)
                {
                    audioSource.PlayOneShot(upSE);

                }
            }
            else
            {
                // 極の向きが変わった場合
                // 慣性で上に飛んでいくのを防止
                //playerCnt.UpStop();
                playerCnt.CanMove();
            }
            
        }
        if (collision.gameObject == chaser)
        {
            chaserCnt.MoveUp();
            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(upSE);

            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // アクションできる位置からオブジェクトが出る
        if (collision.gameObject == player)
        {
            playerCnt.CanMove();
        }
    }


    // 自身の極の向き(タグ)とプレイヤの位置から
    // アクションできる極の向きを取得
    private int GetOrientation()
    {
        // プレイヤーと壁の相対座標を求める
        float relPosX = player.transform.position.x - this.transform.position.x;
        // プレイヤーが壁より左にいるとき
        if (relPosX <= 0)
        {
            // 壁がS極だったら
            if (tag == southTagName) return (int)PoleOrientation.Left;

            // N極だったら
            return (int)PoleOrientation.Right;
        }
        // 右にいるとき
        // 壁がS極だったら
        if (tag == southTagName) return (int)PoleOrientation.Right;

        // N極だったら
        return (int)PoleOrientation.Left;
    }


    // プレイヤの極の向きがアクションできる向きかを取得
    public bool getCanUp()
    {
        return poleCnt.PoleCheck(GetOrientation());
    }
}
