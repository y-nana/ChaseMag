using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// ���[���h
public enum World
{
    first,
    second,
    third
}



// �X�e�[�W�Z���N�g�̃{�^�����O���[�v���ƂɈړ�������N���X
public class WorldChangeManager : MonoBehaviour
{

    private RectTransform rectTransform;

    private World nowWorld;         // ���ݑI�𒆂̃��[���h

    private bool isScroll;          // ���X�N���[������
    
    private float timer;            // �X�N���[���^�C�}�[

    private float startPosition;
    private float goalPosition;

    [SerializeField,Min(0.1f)]
    private float scrollTime;       // ���b�ŃX�N���[���������邩

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        isScroll = false;
        goalPosition = 0.0f;
        nowWorld = World.first;
        timer = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (isScroll)
        {
            
            float deltaMove = (goalPosition-startPosition) * (timer/scrollTime);

            rectTransform.localPosition = new Vector2(startPosition + deltaMove, rectTransform.localPosition.y);
            timer += Time.deltaTime;
            if (timer > scrollTime)
            {
                rectTransform.localPosition = new Vector2(goalPosition, rectTransform.localPosition.y);
                isScroll = false;
            }
        }
    }

    //�I�����ς�邲�ƂɌĂяo��
    public void ChangeSelectedButtom(StageSelectButtonController button)
    {

        if (button.thisWorld == nowWorld)
        {
            return;
        }
        nowWorld = button.thisWorld;
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }
        startPosition = rectTransform.localPosition.x;
        goalPosition = -rectTransform.sizeDelta.x * (int)nowWorld;
        isScroll = true;
        timer = 0.0f;
    }

}
