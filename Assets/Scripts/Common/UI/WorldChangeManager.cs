using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// �X�e�[�W�Z���N�g�̃{�^�����O���[�v���ƂɈړ�������N���X
public class WorldChangeManager : MonoBehaviour
{

    private RectTransform rectTransform;

    private int nowWorld = 0;

    private bool isScroll;
    
    private float timer;

    private float startPosition;
    private float goalPosition;

    [SerializeField,Min(0.1f)]
    private float scrollTime;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        isScroll = false;
        goalPosition = 0.0f;
        nowWorld = 0;
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
    //���[���h��0����
    public void ChangeSelectedButtom(int world)
    {

        if (world == nowWorld)
        {
            return;
        }
        nowWorld = world;
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
        }
        startPosition = rectTransform.localPosition.x;
        goalPosition =  -rectTransform.sizeDelta.x * world;
        isScroll = true;
        timer = 0.0f;
    }

    

}
