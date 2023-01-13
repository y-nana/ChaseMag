using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public enum ItemType
{
    Tesla,
    Stopper
}



// アイテムのデータを管理
[Serializable]
[CreateAssetMenu(fileName ="Item", menuName ="CreateItem")]
public class Item : ScriptableObject
{

    [SerializeField] string itemName;   // 名前
    [SerializeField] ItemType type; // 種類
    [SerializeField] int itemNo;        // 番号
    [SerializeField] float itemEffect;  // 効果 
    [SerializeField] Sprite image;      // 画像
    [SerializeField] float teslaEffect; // テスラへの効力

    public string GetItemName()
    {
        return itemName;
    }

    public ItemType GetItemType()
    {
        return type;
    }

    public int GetItemNo()
    {
        return itemNo;
    }
    public float GetItemEffect()
    {
        return itemEffect;
    }
    public Sprite GetImage()
    {
        return image;
    }

    public float GetTeslaEffect()
    {
        return teslaEffect;
    }
}
