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
    private string viewText;

    [SerializeField]
    private float textSpeed = 0.1f;

    private bool isDrowing = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("Cotest");
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
            yield return 0;
            time += Time.deltaTime;
            int len = Mathf.FloorToInt(time / textSpeed);
            if (len > text.Length)
            {
                break;
            }
            tutrialText.text = text.Substring(0, len);
        }
        tutrialText.text = text;
        yield return 0;
        isDrowing = false;
    }

    // �N���b�N�҂��̃R���[�`��
    IEnumerator Skip()
    {
        while (isDrowing) yield return 0;
        //while (!uitext.IsClicked()) yield return 0;
    }

    // ���͂�\��������R���[�`��
    IEnumerator Cotest()
    {
        //uitext.DrawText("�i���[�V�����������炱�̂܂܏�����OK");
        //yield return StartCoroutine("Skip");

        //uitext.DrawText("���O", "�l���b���̂Ȃ炱��Ȋ���");
        CoDrawText(viewText);
        yield return StartCoroutine("Skip");

    }


}
