using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// �N���A���̃X�R�A���Ǘ�����N���X
public class ScoreManager : MonoBehaviour
{

    // �\���p
    [SerializeField]
    private Text teslaText;             // �e�X����\������e�L�X�g
    [SerializeField]
    private Text rankText;              // �����N��\������e�L�X�g
    [SerializeField]
    private Text clipText;              // �N���b�v�擾����\������e�L�X�g

    private string rank = "C";          // �����N


    void Start()
    {
        ViewResult();

    }



    private int GetScore()
    {
        return 0;
    }

    private void ViewResult()
    {
        // TeslaManager�ɋL�^����Ă���e�X���̒l��\��
        teslaText.text = TeslaManager.tesla.ToString("f0") + " T";

        // �����N�̕\��
        rankText.text = rank;

        // �N���b�v�̕\��
        clipText.text = ClipManager.count.ToString();
    }


}
