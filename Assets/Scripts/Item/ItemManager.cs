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


    void Start()
    {
        itemList = itemDataBase.GetItems();

        // アイテム数初期化
        foreach(Item item in itemList)
        {
            itemNum.Add(item, 0);
        }
    }

    // アイテム抽選
    public Item ItemRoulette()
    {
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
        teslaManage.ChangeWight(item.GetTeslaEffect());
    }
    
}
