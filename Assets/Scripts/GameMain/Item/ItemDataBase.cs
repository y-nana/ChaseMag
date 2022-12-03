using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// アイテムのデータをまとめる
[CreateAssetMenu(fileName ="ItemDataBase", menuName ="CreateItemDataBase")]
public class ItemDataBase : ScriptableObject
{
    [SerializeField]
    private List<Item> itemList = new List<Item>();

    // アイテムのリストを返す
    public List<Item> GetItems()
    {
        return itemList;
    }
}
