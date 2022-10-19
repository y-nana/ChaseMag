using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum TutorialState
{
    Auto,
    Trigger,
    Click
}
[System.Serializable]
struct TutorialData
{
    [field:SerializeField, TextArea]
    public string viewText { get; set; }

    [field:SerializeField]
    public TutorialState tutorialState { get; set; }

    [field:SerializeField]
    public bool chaseSetting { get; set; }

    [field: SerializeField]
    public bool isResult { get; set; }

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

    [SerializeField]
    private string clearTxt;
    [SerializeField]
    private string overTxt;

    private int tutorialIndex;

    private List<TutorialJsonData> tutorialJsonDatas = new List<TutorialJsonData>();

    //private TutorialTryManage tryManage;
    private TextLoadManage textLoadManage;

    [SerializeField]
    private float waitTime = 1.0f;

    private float waitTimer;

    public bool waitTrigger { get; set; } = false;
    public System.Action setChaser { get; set; }
    public System.Action startChaser { get; set; }
    public System.Action tutorialFinish { get; set; }

    public bool clearFlag { get; set; } = false;

    [SerializeField]
    private float textSpeed = 0.1f;

    private bool isDrowing = false;


    // Start is called before the first frame update
    void Start()
    {
        //GameStateManager.instance.ToEvent();
        //textLoadManage = GetComponent<TextLoadManage>();
        //textLoadManage.LoadTest();
        tutorialIndex = 0;
        //tryManage =GetComponent<TutorialTryManage>();
        StartCoroutine(Cotest());


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator CoDrawText(TutorialData data)
    {
        switch (data.tutorialState)
        {
            case TutorialState.Auto:
                GameStateManager.instance.ToPlaying();
                break;
            case TutorialState.Trigger:
                GameStateManager.instance.ToPlaying();
                break;
            case TutorialState.Click:
                GameStateManager.instance.ToEvent();
                break;
            default:
                break;
        }
        isDrowing = true;
        float time = 0;
        while (true)
        {

            yield return null;
            time += Time.deltaTime;

            // 一気に表示
            if (Input.GetButtonDown("Submit") && GameStateManager.instance.gameState == GameState.inEvent)
            {
                break;
            }

            int len = Mathf.FloorToInt(time / textSpeed);
            if (len > data.viewText.Length)
            {
                break;
            }
            tutrialText.text = data.viewText.Substring(0, len);
        }
        tutrialText.text = data.viewText;
        yield return null;
        isDrowing = false;
    }

    // クリック待ちのコルーチン
    IEnumerator ToNextText(TutorialState state)
    {
        while (isDrowing) yield return null;
        switch (state)
        {
            case TutorialState.Auto:
                waitTimer = waitTime;
                while (waitTimer > 0.0f)
                {
                    waitTimer -= Time.deltaTime;
                    yield return null;
                }
                break;
            case TutorialState.Trigger:
                while (!waitTrigger) yield return null;
                waitTrigger = false;
                break;
            case TutorialState.Click:
                while (!(Input.GetButtonDown("Submit") && GameStateManager.instance.gameState == GameState.inEvent)) yield return null;
                break;
            default:
                break;
        }

    }

    // 文章を表示させるコルーチン
    IEnumerator Cotest()
    {

        //Debug.Log(textLoadManage.tutorialDatas);
        //Debug.Log(textLoadManage.tutorialDatas.tutorialDataList);
        while (tutorialIndex < tutorialDatas.Count)
        //while (tutorialIndex < textLoadManage.tutorialDatas.tutorialDataList.Length)
        {

            Debug.Log(tutorialIndex);
            
            if (tutorialDatas[tutorialIndex].chaseSetting)
            {
                setChaser?.Invoke();

            }
            // 逃げ切れたかどうかでメッセージを変化
            if (tutorialDatas[tutorialIndex].isResult)
            {
                TutorialData data = new TutorialData();
                data.viewText = clearFlag ? clearTxt : overTxt;
                data.tutorialState = tutorialDatas[tutorialIndex].tutorialState;
               tutorialDatas[tutorialIndex] = data;
            }

            StartCoroutine(CoDrawText(tutorialDatas[tutorialIndex]));

            //StartCoroutine("CoDrawText", textLoadManage.tutorialDatas.tutorialDataList[tutorialIndex].viewText);

            if (tutorialIndex > 0)
            {
                if (tutorialDatas[tutorialIndex - 1].chaseSetting)
                {
                    startChaser?.Invoke();
                }
            }

            yield return StartCoroutine(ToNextText(tutorialDatas[tutorialIndex].tutorialState));
            //yield return tryManage.StartCoroutine("TryStart", tutorialDatas[tutorialIndex].tutorialState);
            //yield return tryManage.StartCoroutine("TryStart", textLoadManage.tutorialDatas.tutorialDataList[tutorialIndex].state);

            tutorialIndex++;


        }
        yield return null;

        GameStateManager.instance.ToPlaying();
        tutorialFinish?.Invoke();
    }


}
