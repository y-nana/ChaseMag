using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RouteManager : MonoBehaviour
{
    // �L���O���t
    [SerializeField]
    private List<Point> PointList = new List<Point>();
    private List<Transform> closeList = new List<Transform>();
    private List<Point> tempList = new List<Point>();
    


    // ���������h�~�̒T����
    [SerializeField]
    private int maxSearchCount;

    private List<Transform> route;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<Transform> GetRoute(Transform start, Transform goal)
    {

        Vector2 startPos = start.position;
        Vector2 goalPos = goal.position;
        Point sPoint = null;
        Point gPoint = null;
        float startDis = 999;
        float goalDis = 999;
        // �X�^�[�g�œ���A�^���̃|�C���g��I�΂Ȃ��悤��
        float dis_y = 999;
        // ���ꂼ��̈�ԋ߂�Route�|�C���g��T��
        for (int i = 0; i < PointList.Count; i++)
        {
            float temp;
            temp = Vector2.Distance(PointList[i].Pos, startPos);
            float tempY = Mathf.Abs(PointList[i].Pos.y - startPos.y);
            if (temp < startDis&&tempY <= dis_y)
            {
                startDis = temp;
                dis_y = tempY;
                sPoint = PointList[i];
            }

            temp = Vector2.Distance(PointList[i].Pos, goalPos);
            if (temp < goalDis)
            {
                goalDis = temp;
                gPoint = PointList[i];
            }

        }
        //Debug.Log(sPoint.transform.gameObject.name);
        //Debug.Log(gPoint.transform.gameObject.name);




        // �X�^�[�g����S�[���܂ł̌o�H��T������
        if (gPoint != null && sPoint != null)
        {
            SearchRoute(sPoint, gPoint);

        }



        
        return route;
    }

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

        foreach (var item in tempList)
        {
            route.Add(item.m_transform);

        }
        route.Add(goal.m_transform);


    }

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
        foreach (var item in PointList)
        {
            if (item.m_transform == point)
            {

                return item;
            }
        }
        return null;
    }

    public Transform GetNearlyPointTransform(Vector2 pos)
    {

        Point point = null;
        float dis = 99999;
        // ��ԋ߂��|�C���g��T��
        for (int i = 0; i < PointList.Count; i++)
        {
            float temp;
            temp = Vector2.Distance(PointList[i].Pos, pos);
            if (temp < dis)
            {
                dis = temp;
                point = PointList[i];
            }
        }

        return point.m_transform;
    }


    void OnDrawGizmosSelected()

    {
        float arrowAngle = 20;
        float arrowLength = 0.75f;
        
        foreach (var point in PointList)
        {
            foreach (var adjacent in point.adjacentList)
            {
                Gizmos.DrawRay(point.transform.position, adjacent.position - point.transform.position);
                // ���ǉ�
                Vector2 dir = point.transform.position - adjacent.position;
                dir = dir.normalized * arrowLength;
                Vector2 right = Quaternion.Euler(0, 0, arrowAngle) * dir;
                Vector2 left = Quaternion.Euler(0, 0, -arrowAngle) * dir;
                Gizmos.DrawRay(adjacent.position, right);
                Gizmos.DrawRay(adjacent.position, left);
            }

        }

    }

}
