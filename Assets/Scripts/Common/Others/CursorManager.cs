using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// �I�����Ă���{�^�����킩��₷���\�����邽�߂̃N���X
public class CursorManager : MonoBehaviour
{

    [SerializeField]
    private EventSystem eventSystem;

    [SerializeField]
    private Color outlineColor;     // �A�E�g���C�����g���ăJ�[�\����\��
    [SerializeField]
    private float speed = 2.0f;     // �A�E�g���C�������ł���X�s�[�h

    private GameObject preSelectedObj;  // �A�E�g���C�����������߂ɑO�ɃZ���N�g����Ă����{�^�����Ǘ�

    private Outline outline;

    private bool isPulus;           // �����ł��i�A�j���[�V�����p�j

    // Update is called once per frame
    void Update()
    {
        try
        {
            // �C�x���g�V�X�e������I����Ԃɂ���I�u�W�F�N�g���擾
            var selected = eventSystem.currentSelectedGameObject.gameObject;
            // �I�u�W�F�N�g���O�t���[���ƈ������
            if (preSelectedObj != selected)
            {
                if (preSelectedObj != null)
                {
                    // �A�E�g���C�����ړ�
                    Debug.Log(selected);
                    Destroy(outline);
                    outline = selected.AddComponent<Outline>();
                    OutLineInit();
                }
                else
                {
                    // ���߂ĉ�����I������Ƃ�
                    outline = selected.AddComponent<Outline>();
                    OutLineInit();
                }

                preSelectedObj = selected;
            }
        }
        catch (NullReferenceException ex)
        {
            if (preSelectedObj!=null)
            {
                eventSystem.SetSelectedGameObject(preSelectedObj);

            }
        }

        // �J�[�\���̖��ŏ���
        if (outline != null)
        {
            float value = speed * Time.unscaledDeltaTime;
            if (!isPulus)
            {
                value *= -1;
            }
            Color temp = outline.effectColor;
            temp.a += value;
            outline.effectColor =temp;

            if (outline.effectColor.a <= 0)
            {
                isPulus = true;
            }
            else if(outline.effectColor.a >=1)
            {
                isPulus = false;
            }

        }


    }

    // �A�E�g���C���̏�����
    private void OutLineInit()
    {
        outline.effectDistance = new Vector2(-20, -20);
        outline.effectColor = outlineColor;
    }


    
}
