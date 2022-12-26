using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PointCategory
{
    Normal,
    CanJump,
    Floating
}


// �L���O���t���Ǘ�����N���X
[System.Serializable]
public class Point:MonoBehaviour
{
    // �|�C���g
    public Transform myTransform { get; private set; }    // ����̃I�u�W�F�N�g�����̂܂ܓ�����悤�Ƀg�����X�t�H�[��

    public PointCategory category { get; set; }             // �|�C���g�̎��

    // �x�N�g���Ƃ��Ĉ���
    public Vector2 Pos
    {
        get { return myTransform.position; }
    }

    // �אڃ|�C���g
    [field:SerializeField]
    public List<Transform> adjacentList { get; set; }

    
    private void OnEnable()
    {
        myTransform = transform;
    }


#if UNITY_EDITOR
    // �����ƂȂ����|�C���g�ւ̖������C�ŕ\��
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

    
#endif



}
