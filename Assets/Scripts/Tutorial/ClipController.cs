using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �N���b�v�𐧌䂷��N���X
public class ClipController : MonoBehaviour
{
    // �擾���ɔ���������A�N�V����
    public System.Action<ClipController> GetAction { get; set; }
    // �擾�p�^�O��
    private readonly string playerTagName = "Player";   // �v���C��
    private GameObject player;

    private void Start()
    {
        //�v���C���[�^�O�̃I�u�W�F�N�g����ł��邱�ƑO��
        player = GameObject.FindGameObjectWithTag(playerTagName);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �o�^���ꂽ�������s���A���g�𖳌���
        if (collision.gameObject == player)
        {
            this.gameObject.SetActive(false);
            GetAction?.Invoke(this);
        }

    }

}
