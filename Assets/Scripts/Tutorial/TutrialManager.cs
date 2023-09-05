using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// �`���[�g���A���̃e�L�X�g����Ɋւ�����
public enum TutorialState
{
    Auto,       // ��莞�Ԍ㏟��Ɏ��̃e�L�X�g��
    Trigger,    // �����g���K�[����������܂ł��̂܂�
    Click       // �N���b�Nor�{�^���Ŏ��̃e�L�X�g��
}

// �`���[�g���A���ɗp����f�[�^
[System.Serializable]
struct TutorialData
{
    [field:SerializeField, TextArea]
    public string viewText { get; set; }                // �e�L�X�g�ɕ\�����镶��

    [field:SerializeField]
    public TutorialState tutorialState { get; set; }    // ��������̏��

    [field:SerializeField]
    public bool chaseSetting { get; set; }              // �S���Z�b�e�B���O����t�F�[�Y���ǂ���

    [field: SerializeField]
    public bool isResult { get; set; }                  // ���U���g�̃t�F�[�Y���ǂ���

}

// �`���[�g���A���̐i�s�𐧌䂷��N���X
public class TutrialManager : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI tutrialText;    // �`���[�g���A���̃e�L�X�g

    [SerializeField]
    private List<TutorialData> tutorialDatas = new List<TutorialData>();
                                            // �f�[�^�̃��X�g

    // ���U���g�ŕ\������e�L�X�g �󋵂ŕω�������
    [SerializeField]
    private string clearTxt;                // �N���A�������̕���
    [SerializeField]
    private string overTxt;                 // �߂܂������̕���

    private int tutorialIndex;              // �f�[�^���X�g�̃C���f�b�N�X

    [SerializeField]
    private float waitTime = 1.0f;          // �I�[�g�̂Ƃ��ɂ��ׂĕ\����҂���

    private float waitTimer;                // �҂��Ă��鎞�Ԃ��v������^�C�}�[

    public bool waitTrigger { get; set; }               // ���ɐi�ރg���K�[

    // �`���[�g���A���ŕK�v�ȏ�������������Ăяo����悤�ɂ���
    public System.Action setChaser { get; set; }        // �S���Z�b�e�B���O���鏈��
    public System.Action startChaser { get; set; }      // �S���X�^�[�g�����鏈��
    public System.Action tutorialFinish { get; set; }   // �`���[�g���A���I����̏���

    public bool clearFlag { get; set; } = false;        // ���U���g�̏��

    [SerializeField]
    private float textSpeed = 0.1f;         // ��������̃X�s�[�h

    private bool isDrowing;                 // ���ݕ�����\���r�����ǂ���


    void Start()
    {
        // ������
        waitTrigger = false;
        isDrowing = false;
        tutorialIndex = 0;
        // �R���[�`���̊J�n
        StartCoroutine(CoTutorial());
    }

    // ������\��������R���[�`��
    IEnumerator CoDrawText(TutorialData data)
    {
        // �N���b�N�҂��̃e�L�X�g�̂Ƃ��͕\���I���܂Ŏ��֐i�߂Ȃ�����
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
        // �����̕\��
        while (true)
        {

            yield return null;
            time += Time.deltaTime;

            /*
            // ��C�ɕ\��
            if (Input.GetButtonDown("Submit") && GameStateManager.instance.gameState == GameState.inEvent)
            {
                break;
            }
            */

            int len = Mathf.FloorToInt(time / textSpeed);
            // ���ׂĂ̕�����\�����I������while����o��
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

    // ���̃e�L�X�g�֑���̂�҂R���[�`��
    IEnumerator ToNextText(TutorialState state)
    {
        while (isDrowing) yield return null;
        switch (state)
        {
            // �I�[�g�������玞�Ԍo�߂Ŏ���
            case TutorialState.Auto:
                waitTimer = waitTime;
                while (waitTimer > 0.0f)
                {
                    waitTimer -= Time.deltaTime;
                    yield return null;
                }
                break;

                // �g���K�[��true�ɂȂ�܂ő҂�
            case TutorialState.Trigger:
                while (!waitTrigger) yield return null;
                waitTrigger = false;
                break;

                // �N���b�N�����܂ő҂�
            case TutorialState.Click:
                while (!(Input.GetButtonDown("Submit") && GameStateManager.instance.gameState == GameState.InEvent)) yield return null;
                break;
        }

    }

    // �`���[�g���A���̃R���[�`��
    IEnumerator CoTutorial()
    {
        // �`���[�g���A���f�[�^���������J��Ԃ�
        while (tutorialIndex < tutorialDatas.Count)
        {

            //Debug.Log(tutorialIndex);
            // �S���Z�b�e�B���O����t�F�[�Y�̂Ƃ�
            if (tutorialDatas[tutorialIndex].chaseSetting)
            {
                setChaser?.Invoke();

            }
            // ���U���g�̃t�F�[�Y�̂Ƃ�
            if (tutorialDatas[tutorialIndex].isResult)
            {
                TutorialData data = new TutorialData();
                // �����؂ꂽ���ǂ����Ń��b�Z�[�W��ω�
                data.viewText = clearFlag ? clearTxt : overTxt;
                data.tutorialState = tutorialDatas[tutorialIndex].tutorialState;
               tutorialDatas[tutorialIndex] = data;
            }

            // ������\������
            StartCoroutine(CoDrawText(tutorialDatas[tutorialIndex]));

            //�O�̃f�[�^�ŋS���Z�b�e�B���O���Ă���Ƃ�
            if (tutorialIndex > 0)
            {
                if (tutorialDatas[tutorialIndex - 1].chaseSetting)
                {
                    // �S�𓮂����n�߂鏈�����s��
                    startChaser?.Invoke();
                }
            }
            // �e�L�X�g����̃R���[�`�����J�n
            yield return StartCoroutine(ToNextText(tutorialDatas[tutorialIndex].tutorialState));
            // ���̃f�[�^��
            tutorialIndex++;


        }
        yield return null;

        // �`���[�g���A���I��
        GameStateManager.instance.ToPlaying();
        tutorialFinish?.Invoke();
    }


}
