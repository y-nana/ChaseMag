using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[Serializable]
[CreateAssetMenu(fileName ="Item", menuName ="CreateItem")]
public class Item : ScriptableObject
{

    [SerializeField] string itemName;   // 名前
    [SerializeField] int itemNo;        // 番号
    [SerializeField] float itemEffect;  // 効果 
    [SerializeField] Sprite image;      // イメージ
    [SerializeField] float teslaEffect; // テスラへの効力

    public string GetItemName()
    {
        return itemName;
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
