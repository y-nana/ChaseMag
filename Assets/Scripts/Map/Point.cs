using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 有向グラフを管理するクラス
[System.Serializable]
public class Point:MonoBehaviour
{
    // ポイント
    public Transform m_transform { get; private set; }    // からのオブジェクトをそのまま入れれるようにトランスフォーム

    // ベクトルとして扱う
    public Vector2 Pos
    {
        get { return m_transform.position; }
    }

    // 隣接ポイント
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

            // 矢印追加
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
