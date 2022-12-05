using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �N���b�v�𐧌䂷��N���X
public class ClipManager : MonoBehaviour
{
    // �擾���ɔ���������A�N�V����
    public System.Action<ClipManager> GetAction { get; set; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �v���C���[�ȊO�Ԃ���Ȃ����ƑO��
        // �o�^���ꂽ�������s���A���g�𖳌���
        GetAction?.Invoke(this);
        this.gameObject.SetActive(false);
    }

}
