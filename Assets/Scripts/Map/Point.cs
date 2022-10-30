using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �L���O���t���Ǘ�����N���X
[System.Serializable]
public class Point:MonoBehaviour
{
    // �|�C���g
    public Transform m_transform { get; private set; }    // ����̃I�u�W�F�N�g�����̂܂ܓ�����悤�Ƀg�����X�t�H�[��

    // �x�N�g���Ƃ��Ĉ���
    public Vector2 Pos
    {
        get { return m_transform.position; }
    }

    // �אڃ|�C���g
    [field: SerializeField]
    public List<Transform> adjacentList { get; private set; }

    
    private void OnEnable()
    {
        m_transform = transform;
    }
    
    


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        float arrowAngle = 20;
        float arrowLength = 0.75f;

        foreach (var item in adjacentList)
        {
            Gizmos.DrawRay(transform.position, item.position - transform.position);

            // ���ǉ�
            Vector2 center = (item.position - transform.position) / 2.0f + transform.position;

            Vector2 dir = transform.position - item.position;
            dir = dir.normalized * arrowLength;
            Vector2 right = Quaternion.Euler(0, 0, arrowAngle) * dir;
            Vector2 left = Quaternion.Euler(0, 0, -arrowAngle) * dir;
            Gizmos.DrawRay(item.position, right);
            Gizmos.DrawRay(item.position, left);
            Gizmos.DrawRay(center, right);
            Gizmos.DrawRay(center, left);

        }

    }

    

}
