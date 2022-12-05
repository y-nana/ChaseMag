using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    // アイテムデータベース
    [SerializeField] ItemDataBase itemDataBase;

    // 効果付与用オブジェクト
    [SerializeField] PoleController poleCnt;
    [SerializeField] TeslaManager teslaManage;

    // アイテムリスト
    private List<Item> itemList;

    // 取得数表示用
    [SerializeField] List<Text> itemTextList;

    // アイテムの数を管理
    private Dictionary<Item, int> itemNum = 
        new Dictionary<Item, int>();

    // チュートリアル時は決まったリストで排出
    [SerializeField]
    private List<Item> tutorialItemList = new List<Item>();
    private int tutorialListIndex;
    [SerializeField]
    private bool isTutorial;


    void Start()
    {
        itemList = itemDataBase.GetItems();

        // アイテム数初期化
        foreach(Item item in itemList)
        {
            itemNum.Add(item, 0);
        }
        if (isTutorial)
        {
            tutorialListIndex = 0;
        }
    }

    // アイテム抽選
    public Item ItemRoulette()
    {
        if (isTutorial)
        {
            // チュートリアルだったらリスト通りにアイテムを出す
            var item = tutorialItemList[tutorialListIndex];
            tutorialListIndex++;
            // リストの最後まで行ったら繰り返す
            if (tutorialListIndex >= tutorialItemList.Count)
            {
                tutorialListIndex = 0;
            }
            return item;
        }
        int randmonInt = Random.Range(0, itemList.Count);
        return itemList[randmonInt];
    }


    // アイテム取得
    public void GetItem(Item item)
    {
        // 取得数増加
        itemNum[item]++;
        // 効果付与
        poleCnt.ChangePoleStrong(item.GetItemEffect());
        // 取得数増加
        itemTextList[item.GetItemNo()-1].text = itemNum[item].ToString();
        // テスラ
        teslaManage.ChangeWeight(item.GetTeslaEffect());
    }
    
}
