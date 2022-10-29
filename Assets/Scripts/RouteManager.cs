using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �L���O���t���Ǘ�����N���X
[System.Serializable]
public class Point
{
    // �|�C���g
    [field: SerializeField]
    public Transform transform { get; private set; }    // ����̃I�u�W�F�N�g�����̂܂ܓ�����悤�Ƀg�����X�t�H�[��

    // �x�N�g���Ƃ��Ĉ���
    public Vector2 Pos 
    {
        get { return transform.position; }
    }
    
    // �אڃ|�C���g
    [field: SerializeField]
    public List<Transform> adjacentList { get; private set; }

}



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
    private int count;

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
        // ���ꂼ��̈�ԋ߂�Route�|�C���g��T��
        for (int i = 0; i < PointList.Count; i++)
        {
            float temp;
            temp = Vector2.Distance(PointList[i].Pos, startPos);
            if (temp < startDis)
            {
                startDis = temp;
                sPoint = PointList[i];
            }

            temp = Vector2.Distance(PointList[i].Pos, goalPos);
            if (temp < goalDis)
            {
                goalDis = temp;
                gPoint = PointList[i];
            }

        }
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
        count = 0;
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
                closeList.Add(nextPoint.transform);
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
            route.Add(item.transform);
        }
        route.Add(goal.transform);
        

         
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
                    if (tempList[j].transform == start.adjacentList[i])
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
            if (item.transform == point)
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

        return point.transform;
    }


}
