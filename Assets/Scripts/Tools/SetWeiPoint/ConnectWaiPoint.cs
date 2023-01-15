using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
// ����N���X�̏����g����������editor�t�H���_�ɂ͓���Ȃ�
#if UNITY_EDITOR
using UnityEditor;

using Path;
using Layer;
public class ConnectWaiPoint
{

    // �|�C���g���m���Ȃ��邩�`�F�b�N����Ƃ��Ɏg��
    private readonly float upMargin = 1.0f;
    private readonly float downMargin = 2.0f;
    private readonly float sideMargin = 1.0f;

    public Vector2 maxDistance { get; set; }

    // �E�F�C�|�C���g���Ȃ���
    public void Connect(Point waiPoint, GameObject parent)
    {

        // foreach (Point toPoint in pointList)
        foreach (Transform toPointTransform in parent.transform)
        {
            Point toPoint = toPointTransform.GetComponent<Point>();
            Vector2 pos = waiPoint.transform.position;

            Vector2 toPos = toPoint.transform.position;
            Vector2 dis = toPos - pos;

            // �����̃`�F�b�N������
            if (!CheckPosition(waiPoint, toPoint))
            {
                continue;
            }

            // ray���΂��āA�Ȃ��邩�ǂ�������
            Debug.Log(waiPoint.name + "����" + toPoint.name + "�Ƀ��C���΂��Ă݂܂�");

            // �]�v�Ȃ��̂ɂԂ���Ȃ��悤�Ƀ��C���[��ύX
            waiPoint.gameObject.layer = LayerNumber.ignoreRaycast;


            //int layer = 1 << LayerNumber.waiPoint | 1 << LayerNumber.wall;
            int layer = 1 << LayerNumber.wall;
            if (dis.y < 0)
            {
                layer |= 1 << LayerNumber.scaffold;
            }
            Debug.Log("���C���[�F" + Convert.ToString(layer, 2));
            Debug.Log(toPoint.transform.position);


            RaycastHit2D hit = Physics2D.Raycast(pos, dis, dis.magnitude, layer);

            Debug.Log(dis);
            // ���̃|�C���g�܂Ń��C���΂��ď�Q�����Ȃ������ꍇ
            if (!hit)
            {
                // �قڐ^���ɂ���|�C���g���m��
                if (IsHrizontal(dis))
                {
                    // ���B�܂łɑ��ꂪ�قȂ��Ă���������Ȃ�
                    if (!IsScaffoldSame(waiPoint, toPoint))
                    {
                        continue;
                    }

                }
                waiPoint.adjacentList.Add(toPointTransform);


            }
            // ��Q�������������ǁA���Ɍ������ꍇ
            else if (dis.y < -downMargin)
            {
                if (dis.y < -downMargin)
                {
                    Debug.Log(dis.y);
                    // ���ꂩ��~��邱�Ƃł��ǂ����|�C���g��������Ȃ���

                    // ray���΂��āA�Ȃ��邩�ǂ�������
                    toPoint.gameObject.layer = LayerNumber.ignoreRaycast;

                    Debug.Log("�~����H");
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


    // �Ȃ����邩�`�F�b�N����
    private bool CheckPosition(Point waiPoint, Point toPoint)
    {

        Vector2 pos = waiPoint.transform.position;
        Vector2 toPos = toPoint.transform.position;

        // ��r�|�C���g�����g���A��������ꍇ�̓p�X
        if (toPoint.gameObject == waiPoint.gameObject)
        {
            Debug.Log(waiPoint.name + "����" + toPoint.name + "�͎������g�ł�");
            return false;
        }

        Vector2 dis = toPos - pos;
        if (waiPoint.category != PointCategory.Floating && (Mathf.Abs(dis.x) > maxDistance.x || Mathf.Abs(dis.y) > maxDistance.y))
        {
            Debug.Log(waiPoint.name + "����" + toPoint.name + "�͋������������܂�");
            return false;
        }

        if (waiPoint.category == PointCategory.Floating && (Mathf.Abs(dis.x) > sideMargin || dis.y > 0.0f))
        {
            Debug.Log(waiPoint.name + "����" + toPoint.name + "�͗��������ǂ蒅���܂���");
            return false;
        }

        if (waiPoint.category == PointCategory.Normal && dis.y > upMargin)
        {
            Debug.Log(waiPoint.name + "����" + toPoint.name + "�̓W�����v�ł��Ȃ��̂œ͂��܂���ł���");
            return false;
        }
        return true;

    }

    private bool IsScaffoldSame(Point waiPoint, Point toPoint)
    {


        Vector2 pos = waiPoint.transform.position;
        Vector2 toPos = toPoint.transform.position;

        // ���B�܂łɑ��ꂪ�Ȃ����Ă��Ȃ�������ǉ����Ȃ�
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
            Debug.Log(waiPoint.name + "����" + toPoint.name + "�͑��ꂪ�قȂ�܂�");
            waiPoint.gameObject.layer = LayerNumber.waiPoint;

            return false;
        }

        return true;

    }

    // �قڐ^�����ǂ���
    private bool IsHrizontal(Vector2 distance)
    {
        return Mathf.Abs(distance.y) < upMargin && Mathf.Abs(distance.x) > maxDistance.x / 2.0f;
    }


    // �l���ɂ���|�C���g���Ȃ���
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