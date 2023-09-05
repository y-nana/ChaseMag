using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ItemBoxController : MonoBehaviour
{
    // コントロール用コンポーネント
    private ItemManager itemManager;
    private Item item;              // 抽選されたアイテムを格納する
    private GameObject player;
    [SerializeField] GameObject guideTextObj;
    private TextMesh guideText;
    [SerializeField] GameObject showGetItem;    // 抽選されたアイテムを表示するオブジェクト
    private SpriteRenderer itemImage;


    [SerializeField] float interbalTime;              // アイテムを取得できる間隔

    private bool playerGetItem;     // プレイヤがアイテムを取得できる位置にいるか
    private bool choosed;           // アイテムが抽選された状態か

    private bool nowinterbal;   // インターバル中か
    private float timer;        // インターバル用のタイマー

    //アイテムの数を表示するオブジェクトの名前
    private readonly string itemManageName = "ItemManager";

    // 取得用タグ名
    private readonly string playerTagName = "Player";   // プレイヤ

    private readonly string chooseGuide = "Xボタンで\nアイテム抽選";
    private readonly string getGuide = "もう一度Xボタンで\nアイテムゲット";
    private readonly string coolTimeGuide = "クールタイム...";



    void Start()
    {
        // コンポーネント、プレイヤー取得
        itemManager = GameObject.Find(itemManageName).GetComponent<ItemManager>();
        player = GameObject.FindGameObjectWithTag(playerTagName);
        itemImage = showGetItem.GetComponent<SpriteRenderer>();
        guideText = guideTextObj.GetComponent<TextMesh>();
        // テキスト初期化
        guideText.text = chooseGuide;

        
    }

    void Update()
    {

        // アイテムが取得できる位置にいれば
        if (playerGetItem)
        {
            guideTextObj.SetActive(true);


            if ((Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Action3"))
                && GameStateManager.instance.IsInputtable())
            {
                // 抽選された状態かどうか
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


    // インターバル状態の解除
    private void ShowClear()
    {
        choosed = false;
        // アイテムの画像、テキストのリセット
        itemImage.color = Color.clear;
        guideText.text = chooseGuide;
    }

}
