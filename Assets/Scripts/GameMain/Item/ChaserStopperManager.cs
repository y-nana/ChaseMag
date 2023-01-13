using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChaserStopperManager : MonoBehaviour
{

    public bool isGetStopper { get; set; }          // ���݃X�g�b�p�[�������Ă��邩�ǂ���
    private bool canUse;                            // �g����ʒu�ɂ��邩�ǂ���
    [SerializeField]
    private Transform player;                       // �v���C���[�̈ʒu�ɐ�������Ƃ��p
    [SerializeField]
    private Image icon;                        // �擾�󋵂�\������A�C�R��
    [SerializeField]
    private Image YButtonIcon;                 // �g�p�ۂ�\������A�C�R��
    [SerializeField]
    private ChaserStopperController stopperPrefab;  // ��������I�u�W�F�N�g

    private readonly string itemBoxTagName = "ItemBox";

    // Start is called before the first frame update
    void Start()
    {
        canUse = true;
        isGetStopper = false;
        icon.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Action3"))
            && GameStateManager.instance.IsInputtable())
        {
            if (isGetStopper&& canUse)
            {
                UseStopper();

            }
        }



    }

    // �X�g�b�p�[���擾
    public void GetStopper()
    {
        isGetStopper = true;
        icon.gameObject.SetActive(true);
        icon.color = Color.white;

    }

    // �X�g�b�p�[�𗘗p
    private void UseStopper()
    {
        ChaserStopperController stopper = Instantiate(stopperPrefab);
        stopper.transform.position = player.position;
        isGetStopper = false;
        icon.gameObject.SetActive(false);
    }

    // �A�C�e�������߂��ɂ��鎞�͎g�p�s�ɂ���
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(itemBoxTagName))
        {
            canUse = false;
            YButtonIcon.gameObject.SetActive(false);
            icon.color = Color.grey;
            
        }
    }

    // �A�C�e�������痣�ꂽ��g�p�\
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(itemBoxTagName))
        {
            canUse = true;
            YButtonIcon.gameObject.SetActive(true);
            icon.color = Color.white;


        }
    }
}
