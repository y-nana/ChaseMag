using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// クリップ取得数を管理するクラス
public class ClipManager : MonoBehaviour
{

    public static int count { get; set; }
    private readonly string clipTag = "Clip";
    private GameObject[] clips;

    [SerializeField]
    private Text countText;

    [SerializeField]
    private float repopTime = 20.0f;

    [SerializeField]
    private float allRepopTime = 1.0f;


    // Start is called before the first frame update
    void Start()
    {
        count = 0;
        clips = GameObject.FindGameObjectsWithTag(clipTag);
        foreach (var clip in clips)
        {
            clip.SetActive(true);
            // 処理の登録
            clip.GetComponent<ClipController>().GetAction = GetClip;
        }
    }

    // クリップを取得する
    public void GetClip(ClipController clip)
    {
        count++;
        // 取得数表示
        if (countText != null)
        {
            countText.text = count.ToString();
        }
        StartCoroutine(Repop(clip.gameObject));
        // 残りチェック
        if (CheckClip())
        {
            StartCoroutine(ResetClip());
        }
    }

    // 一つのクリップのリポップ
    IEnumerator Repop(GameObject obj)
    {
        yield return new WaitForSeconds(repopTime);
        obj.SetActive(true);
    }

    // すべて取ってたらtrue
    public bool CheckClip()
    {
        foreach (var clip in clips)
        {
            if (clip.activeSelf)
            {
                return false;
            }
        }
        return true;

    }

    //すべて取ったら設定しなおす
    IEnumerator ResetClip()
    {
        yield return new WaitForSeconds(allRepopTime);
        foreach (var clip in clips)
        {
            clip.SetActive(true);
        }
    }

}
