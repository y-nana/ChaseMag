using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Pole;

public class JumpRampController : MonoBehaviour
{

    private GameObject player;
    private GameObject chaser;

    // アクションさせる用のコンポーネント
    private PlayerController playerCnt;
    private ChaserController chaserCnt;

    // 極の向きを確認する用
    private PoleController poleCnt;
    
    private int orientation;    // アクションする極の向き

    // オブジェクトがアクションできる位置にいるか
    private bool playerCanJump;
    private bool chaserCanJump;

    // 取得用タグ名
    private readonly string playerTagName = "Player";   // プレイヤ
    private readonly string chaserTagName = "Chaser";   // チェイサー
    private readonly string southTagName = "South";   // S極
    private readonly string northTagName = "North";   // N極

    // sprite変更用
    private readonly string northPath = "Sprite/Stage/jumpRampNorth";   // N極
    private readonly string southPath = "Sprite/Stage/jumpRampSouth";   // S極

    private AudioSource audioSource;        // サウンド

    // 効果音
    [SerializeField]
    private AudioClip jumpSE;

    void Start()
    {
        //プレイヤー、チェイサータグのオブジェクトが一つであること前提
        player = GameObject.FindGameObjectWithTag(playerTagName);
        chaser = GameObject.FindGameObjectWithTag(chaserTagName);
        playerCnt = player.GetComponent<PlayerController>();
        chaserCnt = chaser.GetComponent<ChaserController>();
        poleCnt = player.GetComponentInChildren<PoleController>();
        SpriteRenderer spriteRenderer = this.GetComponent<SpriteRenderer>();
        this.audioSource = GetComponent<AudioSource>();

        // 自身のタグを確認して、極の向きがどっちになるとジャンプできるのか判定するための変数に代入
        // タグは宣言いらない？
        tag = this.gameObject.tag;
        this.gameObject.name += tag;
        if (tag == southTagName)
        {
            // 自身のspriteを変える
            spriteRenderer.sprite = Resources.Load<Sprite>(southPath);
            orientation = (int)PoleOrientation.Down;
        }
        else if (tag == northTagName)
        {
            spriteRenderer.sprite = Resources.Load<Sprite>(northPath);
            orientation = (int)PoleOrientation.Up;
        }
    }

    void Update()
    {
        // アクションできる位置にいて極の向きが正解
        if (playerCanJump && poleCnt.PoleCheck(orientation))
        {
            //audioSource.PlayOneShot(jumpSE);

            playerCnt.Jump();
            // 連続してすっ飛んでいくのを防ぐ
            playerCanJump = false;
        }
        if (chaserCanJump)
        {
            //audioSource.PlayOneShot(jumpSE);

            chaserCnt.Jump();
            chaserCanJump = false;
        }

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // アクションできる位置にオブジェクトが入る
        if (collision.gameObject == player)
        {
            playerCanJump = true;
        }
        if (collision.gameObject == chaser)
        {
            chaserCanJump = true;
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // アクションできる位置からオブジェクトが出る
        if (collision.gameObject == player)
        {
            playerCanJump = false;
        }
        if (collision.gameObject == chaser)
        {
            chaserCanJump = false;
        }
    }

}
