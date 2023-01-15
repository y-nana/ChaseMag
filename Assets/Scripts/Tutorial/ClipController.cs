using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// クリップを制御するクラス
public class ClipController : MonoBehaviour
{
    // 取得時に発生させるアクション
    public System.Action<ClipController> GetAction { get; set; }
    // 取得用タグ名
    private readonly string playerTagName = "Player";   // プレイヤ
    private GameObject player;

    private void Start()
    {
        //プレイヤータグのオブジェクトが一つであること前提
        player = GameObject.FindGameObjectWithTag(playerTagName);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 登録された処理を行い、自身を無効化
        if (collision.gameObject == player)
        {
            this.gameObject.SetActive(false);
            GetAction?.Invoke(this);
        }

    }

}
