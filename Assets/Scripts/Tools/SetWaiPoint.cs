using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// ����N���X�̏����g����������editor�t�H���_�ɂ͓���Ȃ�
#if UNITY_EDITOR
using UnityEditor;

using Path;
using Layer;

public enum BasePoint
{
    Center,
    TopRight,
    TopLeft,
    BottomRight,
    BottomLeft
}

[System.Serializable]
public class PointPosition
{
    [field:SerializeField]
    public Vector2 position { get; set; }
    [field:SerializeField]
    public BasePoint basePoint { get; set; }



    public PointPosition()
    {
        this.position = Vector2.zero;
        this.basePoint = BasePoint.Center;
    }

    
    public PointPosition(Vector2 pos, BasePoint basePoint)
    {
        this.position = pos;
        this.basePoint = basePoint;
    }
    

}

[System.Serializable]
public class PointSettingData
{
    [field: SerializeField]
    public GameObject obj { get; set; }

    [field: SerializeField]
    public List<PointPosition> pointPosition { get; set; }

    [field:SerializeField]
    public PointCategory pointCategory { get; set; }

}

public class SetWaiPoint : EditorWindow
{

    private GameObject parent;
    private GameObject prefab;

    private int count;

    [SerializeField]
    private List<PointSettingData> settingDatas = new List<PointSettingData>();


    // �Ȃ���Ƃ��Ɏg��
    private List<Point> pointList = new List<Point>();
    private List<Dictionary<BasePoint, Point>> wallPoints = new List<Dictionary<BasePoint, Point>>();
    //private float maxDintance;
    // �ŏI�I�ɂ�ChaserController�ɐݒ肵���l����|�C���g��u���ʒu�����߂���
    private ChaserController chaser;

    Vector2 maxDistance;


    //�X�N���[���ʒu
    private Vector2 _scrollPosition = Vector2.zero;

    // �E�B���h�E
    [MenuItem("Window/Editor extention/SetWaiPoint", false, 1)]
    private static void ShowSetWaiPointWindow()
    {
        SetWaiPoint window = GetWindow<SetWaiPoint>();
        window.titleContent = new GUIContent("SetWaiPoint Window");
        window.Init();

    }

    // �����l�Z�b�g
    private void Init()
    {
        parent = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath.stageRoute);
        prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath.waiPoint);
        if (settingDatas.Count <= 0)
        {

            PointSettingData data = new PointSettingData();
            data.obj = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath.jumpRamp);
            data.pointPosition = new List<PointPosition>();
            data.pointPosition.Add(new PointPosition(Vector2.zero, BasePoint.Center));

            settingDatas.Add(data);

            PointSettingData wallData = new PointSettingData();
            wallData.obj = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath.wall);
            wallData.pointPosition = new List<PointPosition>();
            wallData.pointPosition.Add(new PointPosition(Vector2.zero, BasePoint.TopLeft));
            wallData.pointPosition.Add(new PointPosition(Vector2.zero, BasePoint.TopRight));
            wallData.pointPosition.Add(new PointPosition(new Vector2(0.5f, 0.5f), BasePoint.BottomRight));
            wallData.pointPosition.Add(new PointPosition(new Vector2(-0.5f, 0.5f), BasePoint.BottomLeft));

            settingDatas.Add(wallData);

        }

        // �f�t�H���g�̋���������
        if (maxDistance == Vector2.zero)
        {
            maxDistance = new Vector2(10.0f, 5.0f);
        }

    }

    private void OnGUI()
    {

        EmptyCheck();

        maxDistance = EditorGUILayout.Vector2Field("maxDistance", maxDistance);
        //�`��͈͂�����Ȃ���΃X�N���[���o����悤��
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);


        parent = EditorGUILayout.ObjectField("Parent", parent, typeof(GameObject), true) as GameObject;
        prefab = EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), true) as GameObject;

        EditorGUILayout.Space();
        EditorGUILayout.Space();


        SerializedObject so = new SerializedObject(this);

        so.Update();

        SerializedProperty stringsProperty = so.FindProperty("settingDatas");

        EditorGUILayout.PropertyField(stringsProperty, true);

        so.ApplyModifiedProperties();


       

        //�X�N���[���ӏ��I��
        EditorGUILayout.EndScrollView();

        EditorGUILayout.BeginHorizontal();
        // �{�^���������ꂽ��
        if (GUILayout.Button("WaiPoint�ݒu�I", GUILayout.Height(64)))
        {

            count = 1;
            if (!IsObjInScene(parent))
            {
                // �e�I�u�W�F�N�g�̃��[�g�̐���
                parent = PrefabUtility.InstantiatePrefab(parent) as GameObject;
            }
            else
            {
                // �q�I�u�W�F�N�g�̍폜
                
                for (int i = parent.transform.childCount-1; i >= 0; i--)
                {

                    DestroyImmediate(parent.transform.GetChild(i).gameObject);
                }


            }


            // Ctr+Z�Ŗ߂���悤��undo�ɒǉ�
            Undo.RegisterCreatedObjectUndo(parent, "Create Route");

            Selection.activeObject = parent;

            // �q�I�u�W�F�N�g�̃E�F�C�|�C���g�̐���

            SettingWaiPoints();

            
        }

        if (GUILayout.Button("WaiPoint�Ȃ���I", GUILayout.Height(64)))
        {
            foreach (Transform child in parent.transform)
            {

                ConnectWaiPoint(child.GetComponent<Point>());

            }
        }

        EditorGUILayout.EndHorizontal();


    }
    

    private void EmptyCheck()
    {
        if (parent == null)
        {
            parent = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath.stageRoute);

        }
        if (prefab == null)
        {
            prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath.waiPoint);

        }

    }

    // �|�C���g�̐���
    private void SettingWaiPoints()
    {

        var objects = Resources.FindObjectsOfTypeAll<GameObject>();
        pointList = new List<Point>();

        // ���ׂẴI�u�W�F�N�g���`�F�b�N
        foreach (var obj in objects)
        {

            // �V�[�����̃I�u�W�F�N�g����Ȃ�������Ώۂ�����
            if (!IsObjInScene(obj)) continue;


            for (int i = 0; i < settingDatas.Count; i++)
            {
                if (isEqualBasePrefab(settingDatas[i].obj, obj) && obj.GetComponent<SpriteRenderer>())
                {

                    PositionSetting(i, obj.GetComponent<SpriteRenderer>());


                    break;
                }

            }
        }
    }


    // ��̃I�u�W�F�N�g�ɑ΂��āA�f�[�^���畡���̃|�C���g�𐶐�
    private void PositionSetting(int index, SpriteRenderer spriteRenderer)
    {
        Dictionary<BasePoint, Point> connectPoints = new Dictionary<BasePoint, Point>();
        List<PointPosition> posList = settingDatas[index].pointPosition;

        for (int j = 0; j < posList.Count; j++)
        {
            Vector2 move = spriteRenderer.bounds.center;
            switch (posList[j].basePoint)
            {
                case BasePoint.TopRight:
                    move = spriteRenderer.bounds.max;
                    break;
                case BasePoint.TopLeft:
                    move.x = spriteRenderer.bounds.min.x;
                    move.y = spriteRenderer.bounds.max.y;

                    break;
                case BasePoint.BottomRight:
                    move.x = spriteRenderer.bounds.max.x;
                    move.y = spriteRenderer.bounds.min.y;

                    break;
                case BasePoint.BottomLeft:
                    move = spriteRenderer.bounds.min;
                    break;
            }

            // �����ʒu�ɖ�肪�Ȃ����`�F�b�N
            Vector2 instantiatePosition = move + posList[j].position;
            if (CollisionCheck(instantiatePosition, spriteRenderer.gameObject))
            {
                Debug.Log("���܂邽�ߐ������܂���ł���");
                continue;
            }
            Point point = InstantiateWaiPoint(instantiatePosition, settingDatas[index].pointCategory);
            connectPoints.Add(posList[j].basePoint, point);


        }

        ConnectWaiPoins(connectPoints);

    }



    // �E�F�C�|�C���g�𐶐�����
    private Point InstantiateWaiPoint(Vector2 pos, PointCategory category)
    {

        var pointObj = PrefabUtility.InstantiatePrefab(prefab, parent.transform) as GameObject;
        pointObj.transform.position = pos;

        pointObj.name += "_"+count;
        Point point = pointObj.GetComponent<Point>();
        point.category = category;
        pointList.Add(point);


        Debug.Log(pointObj.name);

        count++;
        Undo.RegisterCreatedObjectUndo(pointObj, "Create WaiPoint");

        return point;
    }


    // �V�[�����ɂ���I�u�W�F�N�g���ǂ���
    private bool IsObjInScene(GameObject obj)
    {
        string path = AssetDatabase.GetAssetOrScenePath(obj);
        return path.Contains(".unity");
    }


    // �M�~�b�N�ȊO�̃I�u�W�F�N�g�ɖ��܂��Ă��邩�ǂ���
    private bool CollisionCheck( Vector2 position , GameObject baseObj)
    {
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.right);

        Debug.DrawRay(position,Vector2.right, Color.blue,0.5f);
        if (hit)
        {
            if (Vector2.Distance( hit.point, position) <= 0)
            {
                Debug.Log(hit.collider.gameObject.name);

                if (baseObj == hit.collider.gameObject)
                {
                    return false;
                }


                return true;
            }
        }
        return false;
    }


    // �Q�[���I�u�W�F�N�g���󂯎��A�����v���n�u�����ƂɂȂ��Ă����true��Ԃ�
    private bool isEqualBasePrefab(GameObject gameObjectA, GameObject gameObjectB)
    {
        return PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObjectA) ==
            PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObjectB);

    }

    // �E�F�C�|�C���g���Ȃ���
    private void ConnectWaiPoint(Point waiPoint)
    {

        // foreach (Point toPoint in pointList)
        foreach (Transform toPointTransform in parent.transform)
        {
            Point toPoint = toPointTransform.GetComponent<Point>();
            Vector2 pos = waiPoint.transform.position;

            Vector2 toPos = toPoint.transform.position;

            // ��r�|�C���g�����g���A��������ꍇ�̓p�X
            //if (toPoint.gameObject == waiPoint.gameObject ||Vector2.Distance(toPos, pos) > )
            {
                Debug.Log(waiPoint.name + "����" + toPoint.name + "�͎������g�ł��邩�������������܂�");

                continue;
            }


            if (waiPoint.category == PointCategory.Normal && toPos.y - pos.y >1.0f)
            {
                Debug.Log(waiPoint.name + "����" + toPoint.name + "�̓W�����v�ł��Ȃ��̂œ͂��܂���ł���");
                continue;

            }



            // ray���΂��āA�Ȃ��邩�ǂ�������
            waiPoint.gameObject.layer = LayerNumber.ignoreRaycast;


            int layer = 1 << LayerNumber.waiPoint | 1 << LayerNumber.wall;
            if (toPos.y - pos.y < 0)
            {
                layer |= 1 << LayerNumber.scaffold;
            }
            Debug.DrawRay(pos, toPos - pos, Color.blue, 0.5f);

            RaycastHit2D hit = Physics2D.Raycast(pos, toPos - pos, Mathf.Infinity, layer);

            Debug.Log(hit.collider.gameObject.name);
            if (hit)
            {

                if (hit.collider.GetComponent<Point>())
                {
                    // Ctr+Z�Ŗ߂���悤��undo�ɒǉ����������ACtr+Y�ŃG���[���o��̂ŕۗ�
                    //Undo.RecordObject(waiPoint, "add point to AdjacentList");
                    waiPoint.adjacentList.Add(hit.collider.transform);

                }

            }

            waiPoint.gameObject.layer = LayerNumber.waiPoint;



        }







    }

    // �l���ɂ���|�C���g���Ȃ���
    private void ConnectWaiPoins(Dictionary<BasePoint,Point> points)
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
