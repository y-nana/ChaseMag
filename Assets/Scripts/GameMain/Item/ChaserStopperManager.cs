using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChaserStopperManager : MonoBehaviour
{

    public bool isGetStopper { get; set; }          // 現在ストッパーを持っているかどうか
    private bool canUse;                            // 使える位置にいるかどうか
    [SerializeField]
    private Transform player;                       // プレイヤーの位置に生成するとき用
    [SerializeField]
    private Image icon;                        // 取得状況を表示するアイコン
    [SerializeField]
    private Image YButtonIcon;                 // 使用可否を表示するアイコン
    [SerializeField]
    private ChaserStopperController stopperPrefab;  // 生成するオブジェクト

    private readonly string itemBoxTagName = "ItemBox";

    // Start is called before the first frame update
    void Start()
    {
        canUse = true;
        isGetStopper = false;
        icon.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Action3"))
            && GameStateManager.instance.IsInputtable())
        {
            if (isGetStopper&& canUse)
            {
                UseStopper();

            }
        }



    }

    // ストッパーを取得
    public void GetStopper()
    {
        isGetStopper = true;
        icon.gameObject.SetActive(true);
        icon.color = Color.white;

    }

    // ストッパーを利用
    private void UseStopper()
    {
        ChaserStopperController stopper = Instantiate(stopperPrefab);
        stopper.transform.position = player.position;
        isGetStopper = false;
        icon.gameObject.SetActive(false);
    }

    // アイテム箱が近くにある時は使用不可にする
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(itemBoxTagName))
        {
            canUse = false;
            YButtonIcon.gameObject.SetActive(false);
            icon.color = Color.grey;
            
        }
    }

    // アイテム箱から離れたら使用可能
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(itemBoxTagName))
        {
            canUse = true;
            YButtonIcon.gameObject.SetActive(true);
            icon.color = Color.white;


        }
    }
}
