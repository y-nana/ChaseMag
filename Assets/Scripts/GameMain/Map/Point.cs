using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PointCategory
{
    Normal,
    CanJump,
    Floating
}


// 有向グラフを管理するクラス
[System.Serializable]
public class Point:MonoBehaviour
{
    // ポイント
    public Transform myTransform { get; private set; }    // からのオブジェクトをそのまま入れれるようにトランスフォーム

    public PointCategory category { get; set; }             // ポイントの種類

    // ベクトルとして扱う
    public Vector2 Pos
    {
        get { return myTransform.position; }
    }

    // 隣接ポイント
    [field:SerializeField]
    public List<Transform> adjacentList { get; set; }

    
    private void OnEnable()
    {
        myTransform = transform;
    }


#if UNITY_EDITOR
    // 自分とつないだポイントへの矢印をレイで表示
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

    
#endif



}
