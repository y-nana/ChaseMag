using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 経路及び経路探索を処理するクラス
public class RouteManager : MonoBehaviour
{
    // 有向グラフ
    private List<Point> pointList = new List<Point>();
    // 探索時に使うリスト
    private List<Transform> closeList = new List<Transform>();
    private List<Point> tempList = new List<Point>();
    


    // 処理落ち防止の探索回数
    [SerializeField]
    private int maxSearchCount;
    // ルート
    private List<Transform> route;



    // Start is called before the first frame update
    void Start()
    {
        // 子オブジェクトからポイントを取得
        var points = GetComponentsInChildren<Point>();
        foreach (var p in points)
        {
            pointList.Add(p);
        }
    }

    public List<Transform> GetRoute(Transform start, Transform goal)
    {

        Vector2 startPos = start.position;
        Vector2 goalPos = goal.position;
        Point sPoint = null;
        Point gPoint = null;
        float startDis = 999;
        float goalDis = 999;
        // それぞれの一番近いRouteポイントを探す
        for (int i = 0; i < pointList.Count; i++)
        {
            float temp;
            temp = Vector2.Distance(pointList[i].Pos, startPos);
            // 縦移動は無理な場合が多いので重みをつける
            temp += Mathf.Abs(pointList[i].Pos.y - startPos.y);
            if (temp < startDis)
            {
                startDis = temp;
                sPoint = pointList[i];
            }

            temp = Vector2.Distance(pointList[i].Pos, goalPos);
            if (temp < goalDis)
            {
                goalDis = temp;
                gPoint = pointList[i];
            }

        }

        // スタートからゴールまでの経路を探索する
        if (gPoint != null && sPoint != null)
        {
            SearchRoute(sPoint, gPoint);

        }

        return route;
    }

    // 経路探索
    private void SearchRoute(Point start ,Point goal)
    {
        route = new List<Transform>();
        closeList = new List<Transform>();
        tempList = new List<Point>();
        tempList.Add(start);

        var nextPoint = GetNextPoint(start, goal);
        //tempList.Add(nextPoint);
        for (int i = 0; i < maxSearchCount; i++)
        {
            if (nextPoint == goal)
            {
                break;
                
            }
            var tempPoint = GetNextPoint(nextPoint, goal);
            if (tempPoint == null)
            {
                // 次の候補がなかったらここを閉じリストに追加
                closeList.Add(nextPoint.m_transform);
                if (tempList.Count > 0)
                {
                    nextPoint = tempList[tempList.Count - 1];
                    tempList.Remove(nextPoint);
                }

            }
            else
            {
                tempList.Add(nextPoint);
                nextPoint = tempPoint;
            }


        }
        // ポイントのリストからトランスフォームのリストへ
        foreach (var item in tempList)
        {
            route.Add(item.m_transform);

        }
        route.Add(goal.m_transform);


    }

    // 次のポイントを取得する
    private Point GetNextPoint(Point start, Point goal)
    {
        float dis = 999;
        Transform point = null;
        for (int i = 0; i < start.adjacentList.Count; i++)
        {

            float temp = Vector2.Distance(start.adjacentList[i].position, goal.Pos);
            if (temp < dis)
            {
                bool isClose = false;
                bool isAlready = false;
                // 閉じリストにそのポイントがないかどうか
                for (int j = 0; j < closeList.Count; j++)
                {
                    if (closeList[j] == start.adjacentList[i])
                    {
                        isClose = true;
                        break;
                    }

                }

                // 今までに通った道じゃないか
                for (int j = 0; j < tempList.Count; j++)
                {
                    if (tempList[j].m_transform == start.adjacentList[i])
                    {
                        isAlready = true;
                        break;
                    }
                }
                if (!isClose && !isAlready)
                {
                    dis = temp;
                    point = start.adjacentList[i];
                }


            }
        }
        // トランスフォームからポイントを特定
        foreach (var item in pointList)
        {
            if (item.m_transform == point)
            {

                return item;
            }
        }
        return null;
    }

    // 座標から一番近いポイントを返す
    public Transform GetNearlyPointTransform(Vector2 pos)
    {

        Point point = null;
        float dis = 99999;
        // 一番近いポイントを探す
        for (int i = 0; i < pointList.Count; i++)
        {
            float temp;
            temp = Vector2.Distance(pointList[i].Pos, pos);
            if (temp < dis)
            {
                dis = temp;
                point = pointList[i];
            }
        }

        return point.m_transform;
    }


}
