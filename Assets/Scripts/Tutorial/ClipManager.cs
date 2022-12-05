using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// クリップを制御するクラス
public class ClipManager : MonoBehaviour
{
    // 取得時に発生させるアクション
    public System.Action<ClipManager> GetAction { get; set; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // プレイヤー以外ぶつからないこと前提
        // 登録された処理を行い、自身を無効化
        GetAction?.Invoke(this);
        this.gameObject.SetActive(false);
    }

}
