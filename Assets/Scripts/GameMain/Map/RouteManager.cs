using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �o�H�y�ьo�H�T������������N���X
public class RouteManager : MonoBehaviour
{
    // �L���O���t
    private List<Point> pointList = new List<Point>();
    // �T�����Ɏg�����X�g
    private List<Transform> closeList = new List<Transform>();
    private List<Point> tempList = new List<Point>();
    


    // ���������h�~�̒T����
    [SerializeField]
    private int maxSearchCount;
    // ���[�g
    private List<Transform> route;



    // Start is called before the first frame update
    void Start()
    {
        // �q�I�u�W�F�N�g����|�C���g���擾
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
        // ���ꂼ��̈�ԋ߂�Route�|�C���g��T��
        for (int i = 0; i < pointList.Count; i++)
        {
            float temp;
            temp = Vector2.Distance(pointList[i].Pos, startPos);
            // �c�ړ��͖����ȏꍇ�������̂ŏd�݂�����
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

        // �X�^�[�g����S�[���܂ł̌o�H��T������
        if (gPoint != null && sPoint != null)
        {
            SearchRoute(sPoint, gPoint);

        }

        return route;
    }

    // �o�H�T��
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
                // ���̌�₪�Ȃ������炱��������X�g�ɒǉ�
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
        // �|�C���g�̃��X�g����g�����X�t�H�[���̃��X�g��
        foreach (var item in tempList)
        {
            route.Add(item.m_transform);

        }
        route.Add(goal.m_transform);


    }

    // ���̃|�C���g���擾����
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
                // �����X�g�ɂ��̃|�C���g���Ȃ����ǂ���
                for (int j = 0; j < closeList.Count; j++)
                {
                    if (closeList[j] == start.adjacentList[i])
                    {
                        isClose = true;
                        break;
                    }

                }

                // ���܂łɒʂ���������Ȃ���
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
        // �g�����X�t�H�[������|�C���g�����
        foreach (var item in pointList)
        {
            if (item.m_transform == point)
            {

                return item;
            }
        }
        return null;
    }

    // ���W�����ԋ߂��|�C���g��Ԃ�
    public Transform GetNearlyPointTransform(Vector2 pos)
    {

        Point point = null;
        float dis = 99999;
        // ��ԋ߂��|�C���g��T��
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
