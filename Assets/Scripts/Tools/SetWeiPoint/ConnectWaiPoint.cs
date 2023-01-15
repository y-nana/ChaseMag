using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
// 自作クラスの情報を使いたいためeditorフォルダには入れない
#if UNITY_EDITOR
using UnityEditor;

using Path;
using Layer;
public class ConnectWaiPoint
{

    // ポイント同士をつなげるかチェックするときに使う
    private readonly float upMargin = 1.0f;
    private readonly float downMargin = 2.0f;
    private readonly float sideMargin = 1.0f;

    public Vector2 maxDistance { get; set; }

    // ウェイポイントをつなげる
    public void Connect(Point waiPoint, GameObject parent)
    {

        // foreach (Point toPoint in pointList)
        foreach (Transform toPointTransform in parent.transform)
        {
            Point toPoint = toPointTransform.GetComponent<Point>();
            Vector2 pos = waiPoint.transform.position;

            Vector2 toPos = toPoint.transform.position;
            Vector2 dis = toPos - pos;

            // 距離のチェックをする
            if (!CheckPosition(waiPoint, toPoint))
            {
                continue;
            }

            // rayを飛ばして、つながるかどうか判定
            Debug.Log(waiPoint.name + "から" + toPoint.name + "にレイを飛ばしてみます");

            // 余計なものにぶつからないようにレイヤーを変更
            waiPoint.gameObject.layer = LayerNumber.ignoreRaycast;


            //int layer = 1 << LayerNumber.waiPoint | 1 << LayerNumber.wall;
            int layer = 1 << LayerNumber.wall;
            if (dis.y < 0)
            {
                layer |= 1 << LayerNumber.scaffold;
            }
            Debug.Log("レイヤー：" + Convert.ToString(layer, 2));
            Debug.Log(toPoint.transform.position);


            RaycastHit2D hit = Physics2D.Raycast(pos, dis, dis.magnitude, layer);

            Debug.Log(dis);
            // 次のポイントまでレイを飛ばして障害物がなかった場合
            if (!hit)
            {
                // ほぼ真横にあるポイント同士で
                if (IsHrizontal(dis))
                {
                    // 到達までに足場が異なっていたら加しない
                    if (!IsScaffoldSame(waiPoint, toPoint))
                    {
                        continue;
                    }

                }
                waiPoint.adjacentList.Add(toPointTransform);


            }
            // 障害物があったけど、下に向かう場合
            else if (dis.y < -downMargin)
            {
                if (dis.y < -downMargin)
                {
                    Debug.Log(dis.y);
                    // 足場から降りることでたどりつけるポイントだったらつなげる

                    // rayを飛ばして、つながるかどうか判定
                    toPoint.gameObject.layer = LayerNumber.ignoreRaycast;

                    Debug.Log("降りれる？");
                    Vector2 orientation = dis.x < 0.0f ? Vector2.left : Vector2.right;
                    layer &= ~(1 << LayerNumber.waiPoint);

                    RaycastHit2D horizonHit = Physics2D.Raycast(pos, orientation, Mathf.Abs(dis.x), layer);
                    RaycastHit2D verticalHit = Physics2D.Raycast(toPos, Vector2.up, Mathf.Abs(dis.y), layer);
                    Debug.DrawRay(pos, orientation, Color.blue, 0.5f);

                    if (horizonHit)
                    {
                        Debug.Log(horizonHit.collider.name);

                    }
                    if (verticalHit)
                    {
                        Debug.Log(verticalHit.collider.name);

                    }

                    if (!horizonHit && !verticalHit)
                    {
                        waiPoint.adjacentList.Add(toPoint.transform);

                    }

                    toPoint.gameObject.layer = LayerNumber.waiPoint;

                }
            }

            waiPoint.gameObject.layer = LayerNumber.waiPoint;

        }

    }


    // つなげられるかチェックする
    private bool CheckPosition(Point waiPoint, Point toPoint)
    {

        Vector2 pos = waiPoint.transform.position;
        Vector2 toPos = toPoint.transform.position;

        // 比較ポイントが自身か、遠すぎる場合はパス
        if (toPoint.gameObject == waiPoint.gameObject)
        {
            Debug.Log(waiPoint.name + "から" + toPoint.name + "は自分自身です");
            return false;
        }

        Vector2 dis = toPos - pos;
        if (waiPoint.category != PointCategory.Floating && (Mathf.Abs(dis.x) > maxDistance.x || Mathf.Abs(dis.y) > maxDistance.y))
        {
            Debug.Log(waiPoint.name + "から" + toPoint.name + "は距離が遠すぎます");
            return false;
        }

        if (waiPoint.category == PointCategory.Floating && (Mathf.Abs(dis.x) > sideMargin || dis.y > 0.0f))
        {
            Debug.Log(waiPoint.name + "から" + toPoint.name + "は落下時たどり着けません");
            return false;
        }

        if (waiPoint.category == PointCategory.Normal && dis.y > upMargin)
        {
            Debug.Log(waiPoint.name + "から" + toPoint.name + "はジャンプできないので届きませんでした");
            return false;
        }
        return true;

    }

    private bool IsScaffoldSame(Point waiPoint, Point toPoint)
    {


        Vector2 pos = waiPoint.transform.position;
        Vector2 toPos = toPoint.transform.position;

        // 到達までに足場がつながっていなかったら追加しない
        int footLayer = 1 << LayerNumber.scaffold | 1 << LayerNumber.floor;
        RaycastHit2D horizonHit = Physics2D.Raycast(pos, Vector2.down, Mathf.Infinity, footLayer);
        RaycastHit2D verticalHit = Physics2D.Raycast(toPos, Vector2.down, Mathf.Infinity, footLayer);
        if (horizonHit)
        {
            Debug.Log(horizonHit.collider.name);

        }
        if (verticalHit)
        {
            Debug.Log(verticalHit.collider.name);

        }
        if (horizonHit.collider.gameObject != verticalHit.collider.gameObject)
        {
            Debug.Log(waiPoint.name + "から" + toPoint.name + "は足場が異なります");
            waiPoint.gameObject.layer = LayerNumber.waiPoint;

            return false;
        }

        return true;

    }

    // ほぼ真横かどうか
    private bool IsHrizontal(Vector2 distance)
    {
        return Mathf.Abs(distance.y) < upMargin && Mathf.Abs(distance.x) > maxDistance.x / 2.0f;
    }


    // 四隅にあるポイントをつなげる
    public void ConnectCorners(Dictionary<BasePoint, Point> points)
    {
        if (points.ContainsKey(BasePoint.TopLeft))
        {
            if (points.ContainsKey(BasePoint.TopRight))
            {
                points[BasePoint.TopLeft].adjacentList.Add(points[BasePoint.TopRight].transform);
                points[BasePoint.TopRight].adjacentList.Add(points[BasePoint.TopLeft].transform);

            }
            if (points.ContainsKey(BasePoint.BottomLeft))
            {
                points[BasePoint.TopLeft].adjacentList.Add(points[BasePoint.BottomLeft].transform);
                points[BasePoint.BottomLeft].adjacentList.Add(points[BasePoint.TopLeft].transform);
            }
        }

        if (points.ContainsKey(BasePoint.TopRight))
        {

            if (points.ContainsKey(BasePoint.BottomRight))
            {
                points[BasePoint.TopRight].adjacentList.Add(points[BasePoint.BottomRight].transform);
                points[BasePoint.BottomRight].adjacentList.Add(points[BasePoint.TopRight].transform);
            }
        }
    }

}
#endif