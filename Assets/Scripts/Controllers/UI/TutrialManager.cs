using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum TutorialState
{
    none,
    walk,
    jump,
    wall,
    poleRotation,
    item,
    rule
}
[System.Serializable]
struct TutorialData
{
    [field:SerializeField, TextArea]
    public string viewText { get; set; }

    [field:SerializeField]
    public TutorialState tutorialState { get; set; }

}
[System.Serializable]
public class TutorialJsonData
{
    public string viewText;

    public TutorialState state;

}
[System.Serializable]
public class TutorialJsonWrapper
{
    public TutorialJsonData[] tutorialDataList;
}

public class TutrialManager : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI tutrialText;

    [SerializeField]
    private List<TutorialData> tutorialDatas = new List<TutorialData>();
    private int tutorialIndex;

    private List<TutorialJsonData> tutorialJsonDatas = new List<TutorialJsonData>();

    private TutorialTryManage tryManage;
    private TextLoadManage textLoadManage;


    [SerializeField]
    private float textSpeed = 0.1f;

    private bool isDrowing = false;

    // Start is called before the first frame update
    void Start()
    {
        GameStateManager.instance.ToEvent();
        textLoadManage = GetComponent<TextLoadManage>();
        textLoadManage.LoadTest();
        tutorialIndex = 0;
        tryManage =GetComponent<TutorialTryManage>();
        StartCoroutine(Cotest());


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

            // 一気に表示
            if (Input.GetMouseButtonDown(0))
            {
                break;
            }

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
        //while (!(Input.GetMouseButtonDown(0) && !PauseManager.nowPause)) yield return null;
        while (!(Input.GetMouseButtonDown(0) && GameStateManager.instance.gameState == GameState.inEvent)) yield return null;
        Debug.Log("test");
    }

    // 文章を表示させるコルーチン
    IEnumerator Cotest()
    {

        Debug.Log(textLoadManage.tutorialDatas);
        Debug.Log(textLoadManage.tutorialDatas.tutorialDataList);
        //while (tutorialIndex < tutorialDatas.Count)
        while (tutorialIndex < textLoadManage.tutorialDatas.tutorialDataList.Length)
        {
            Debug.Log(textLoadManage);

            //StartCoroutine("CoDrawText", tutorialDatas[tutorialIndex].viewText);
            StartCoroutine("CoDrawText", textLoadManage.tutorialDatas.tutorialDataList[tutorialIndex].viewText);

            yield return StartCoroutine("Skip");
            //yield return tryManage.StartCoroutine("TryStart", tutorialDatas[tutorialIndex].tutorialState);
            yield return tryManage.StartCoroutine("TryStart", textLoadManage.tutorialDatas.tutorialDataList[tutorialIndex].state);
            tutorialIndex++;
        }

        GameStateManager.instance.ToPlaying();

    }


}
