using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutrialManager : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI tutrialText;

    [SerializeField, TextArea]
    private List<string> viewTextList = new List<string>();

    private int textListIndex;

    [SerializeField]
    private float textSpeed = 0.1f;

    private bool isDrowing = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("Cotest");
        textListIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator CoDrawText(string text)
    {

        isDrowing = true;
        float time = 0;
        while (true)
        {

            yield return null;
            time += Time.deltaTime;
            int len = Mathf.FloorToInt(time / textSpeed);
            if (len > text.Length)
            {
                break;
            }
            tutrialText.text = text.Substring(0, len);
        }
        tutrialText.text = text;
        yield return null;
        isDrowing = false;
    }

    // クリック待ちのコルーチン
    IEnumerator Skip()
    {
        while (isDrowing) yield return null;
        while (!(Input.GetMouseButtonDown(0) && !PauseManager.nowPause)) yield return null;
        Debug.Log("test");
    }

    // 文章を表示させるコルーチン
    IEnumerator Cotest()
    {

        while (textListIndex < viewTextList.Count)
        {
            StartCoroutine("CoDrawText", viewTextList[textListIndex]);

            yield return StartCoroutine("Skip");
            textListIndex++;
        }


    }


}
