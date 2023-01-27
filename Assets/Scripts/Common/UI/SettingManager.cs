using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    [SerializeField]
    private Button viewSettingButton;     // �V�ѕ���\������{�^��

    [SerializeField]
    private GameObject setting;  // �V�ѕ��I�u�W�F�N�g
    [SerializeField]
    private Button firstButton;  // �I���{�^��


    private void OnEnable()
    {
        setting.gameObject.SetActive(false);
    }

    // �V�ѕ���\������
    public void ViewSetting()
    {
        setting.gameObject.SetActive(true);
        firstButton.Select();
    }

    // �V�ѕ������
    public void CloseSetting()
    {
        setting.gameObject.SetActive(false);
        // �\������{�^����I����Ԃɂ���
        viewSettingButton.Select();

    }
}
