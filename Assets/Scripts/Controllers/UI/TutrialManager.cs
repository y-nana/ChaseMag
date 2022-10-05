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
public class TutrialManager : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI tutrialText;

    [SerializeField, TextArea]
    private List<string> viewTextList = new List<string>();

    private int textListIndex;

    [SerializeField]
    private List<TutorialData> tutorialDatas = new List<TutorialData>();
    private int tutorialIndex;

    private TutorialTryManage tryManage;

    [SerializeField]
    private float textSpeed = 0.1f;

    private bool isDrowing = false;

    // Start is called before the first frame update
    void Start()
    {
        GameStateManager.instance.ToEvent();
        StartCoroutine("Cotest");
        textListIndex = 0;
        tutorialIndex = 0;
        tryManage =GetComponent<TutorialTryManage>();
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
        //while (!(Input.GetMouseButtonDown(0) && !PauseManager.nowPause)) yield return null;
        while (!(Input.GetMouseButtonDown(0) && GameStateManager.instance.gameState == GameState.inEvent)) yield return null;
        Debug.Log("test");
    }

    // 文章を表示させるコルーチン
    IEnumerator Cotest()
    {

        while (tutorialIndex < tutorialDatas.Count)
        {
            StartCoroutine("CoDrawText", tutorialDatas[tutorialIndex].viewText);

            yield return StartCoroutine("Skip");
            yield return tryManage.StartCoroutine("TryStart", tutorialDatas[tutorialIndex].tutorialState);
            tutorialIndex++;
        }

        GameStateManager.instance.ToPlaying();

    }


}
