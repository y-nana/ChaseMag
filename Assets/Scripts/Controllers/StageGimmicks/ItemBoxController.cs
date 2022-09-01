using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ItemBoxController : MonoBehaviour
{
    // コントロール用コンポーネント
    private ItemManager itemManager;
    private Item item;
    private GameObject player;
    [SerializeField] GameObject guideTextObj;
    private TextMesh guideText;
    [SerializeField] GameObject showGetItem;
    private SpriteRenderer itemImage;


    [SerializeField] float interbalTime = 3.0f;              // アイテムを取得できる間隔

    private bool playerGetItem;     // プレイヤがアイテムを取得できる位置にいるか
    private bool choosed;           // アイテムが抽選された状態か

    private bool nowinterbal;   // インターバル中か
    private float timer;        // インターバル用のタイマー

    //アイテムの数を表示するオブジェクトの名前
    private readonly string itemManageName = "ItemManager";

    // 取得用タグ名
    private readonly string playerTagName = "Player";   // プレイヤ

    private readonly string chooseGuide = "";
    //private readonly string chooseGuide = "spaceキーで\nアイテム抽選";
    private readonly string getGuide = "";
    //private readonly string getGuide = "もう一度\nspaceキーで\nアイテム獲得";
    private readonly string coolTimeGuide = "クールタイム...";


    void Start()
    {
        itemManager = GameObject.Find(itemManageName).GetComponent<ItemManager>();
        player = GameObject.FindGameObjectWithTag(playerTagName);
        itemImage = showGetItem.GetComponent<SpriteRenderer>();
        guideText = guideTextObj.GetComponent<TextMesh>();
        guideText.text = chooseGuide;
    }

    void Update()
    {

        // アイテムが取得できる位置にいれば
        if (playerGetItem)
        {
            guideTextObj.SetActive(true);

            if (Input.GetKeyDown(KeyCode.Space) && !PauseManager.nowPause)
            {
                if (choosed)
                {
                    // アイテム獲得
                    itemManager.GetItem(item);
                    ShowClear();
                    guideText.text = coolTimeGuide;
                }
                else if (!nowinterbal)
                {

                    // アイテム抽選
                    ChooseItem();
                    choosed = true;
                }
            }

        }
        // インターバル中
        if (nowinterbal)
        {
            timer -= Time.deltaTime;
            // インターバル解除
            if (timer < 0)
            {
                ShowClear();
                nowinterbal = false;
            }
        }
    }

    // プレイヤがアイテムを取得できる位置に入る
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == player)
        {
            playerGetItem = true;
            
        }
    }

    // プレイヤがアイテムを取得できる位置から出る
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == player)
        {
            playerGetItem = false;
            guideTextObj.SetActive(false);
        }
    }

    // アイテム抽選
    private void ChooseItem()
    {
        item =itemManager.ItemRoulette();
        // 当選アイテム表示
        itemImage.sprite = item.GetImage();
        itemImage.color = Color.white;
        // インターバルに入る
        nowinterbal = true;
        timer = interbalTime;
        guideText.text = getGuide;
        choosed = true;
    }

    private void ShowClear()
    {
        choosed = false;
        itemImage.color = Color.clear;
        guideText.text = chooseGuide;
    }

}
