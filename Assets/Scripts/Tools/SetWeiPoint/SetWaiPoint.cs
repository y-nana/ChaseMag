using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// ����N���X�̏����g����������editor�t�H���_�ɂ͓���Ȃ�
#if UNITY_EDITOR
using UnityEditor;

using Path;
using Layer;

public class SetWaiPoint : EditorWindow
{

    private GameObject parent;
    private GameObject prefab;

    private int count;

    [SerializeField]
    private List<PointSettingData> settingDatas = new List<PointSettingData>();

    // �|�C���g��ǉ��ݒu����Ƃ��Ɏg��
    private Dictionary<GameObject, List<float>> objectOnPoints = new Dictionary<GameObject, List<float>>();

    // �Ȃ���Ƃ��Ɏg��
    private List<Point> pointList = new List<Point>();

    private ConnectWaiPoint connector;

    //private float maxDintance;
    // �ŏI�I�ɂ�ChaserController�ɐݒ肵���l����|�C���g��u���ʒu�����߂���
    private ChaserController chaser;

    Vector2 maxDistance;

    //�X�N���[���ʒu
    private Vector2 _scrollPosition = Vector2.zero;

    // �f�t�H���g�̒l
    private readonly Vector2 defaultMaxDistance = new Vector2(10.0f, 5.0f);
    private readonly Vector2 defaultWallBottomPosition = new Vector2(1.0f, 0.5f);
    private readonly Vector2 defaultWallMiddlePosition = new Vector2(1.0f, 0);
    private readonly Vector2 defaultPoleScaffoldPosition = new Vector2(0, 1.0f);
    private readonly Vector2 defaultScaffoldPosition = new Vector2(0, 0.4f);



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
            // �f�t�H���g�̐����ʒu�̏�������

            PointSettingData jumpRampData = new PointSettingData();
            jumpRampData.obj = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath.jumpRamp);
            jumpRampData.pointPosition = new List<PointPosition>();
            jumpRampData.pointPosition.Add(new PointPosition(Vector2.zero, BasePoint.Center,PointCategory.CanJump));
            settingDatas.Add(jumpRampData);

            PointSettingData wallData = new PointSettingData();
            wallData.obj = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath.wall);
            wallData.pointPosition = new List<PointPosition>();
            wallData.pointPosition.Add(new PointPosition(Vector2.zero, BasePoint.TopLeft, PointCategory.Normal));
            wallData.pointPosition.Add(new PointPosition(Vector2.zero, BasePoint.TopRight, PointCategory.Normal));
            wallData.pointPosition.Add(new PointPosition(defaultWallBottomPosition, BasePoint.BottomRight, PointCategory.Normal));
            wallData.pointPosition.Add(new PointPosition(new Vector2( -defaultWallBottomPosition.x, defaultWallBottomPosition.y), BasePoint.BottomLeft, PointCategory.Normal));

            //wallData.pointPosition.Add(new PointPosition(defaultWallMiddlePosition, BasePoint.Center, PointCategory.Floating));
            //wallData.pointPosition.Add(new PointPosition(-defaultWallMiddlePosition, BasePoint.Center, PointCategory.Floating));

            settingDatas.Add(wallData);

            PointSettingData poleScaffoldData = new PointSettingData();
            poleScaffoldData.obj = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath.poleScaffold);
            poleScaffoldData.pointPosition = new List<PointPosition>();
            poleScaffoldData.pointPosition.Add(new PointPosition(defaultPoleScaffoldPosition, BasePoint.Center, PointCategory.Normal));
            settingDatas.Add(poleScaffoldData);


            PointSettingData itemBoxData = new PointSettingData();
            itemBoxData.obj = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath.itemBox);
            itemBoxData.pointPosition = new List<PointPosition>();
            itemBoxData.pointPosition.Add(new PointPosition(Vector2.zero, BasePoint.Center, PointCategory.Normal));
            settingDatas.Add(itemBoxData);


            PointSettingData scaffoldData = new PointSettingData();
            scaffoldData.obj = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath.scaffold);
            scaffoldData.pointPosition = new List<PointPosition>();
            scaffoldData.pointPosition.Add(new PointPosition(defaultScaffoldPosition, BasePoint.Top, PointCategory.Normal));
            settingDatas.Add(scaffoldData);

        }

        // �f�t�H���g�̋���������
        if (maxDistance == Vector2.zero)
        {
            maxDistance = defaultMaxDistance;
        }

        connector = new ConnectWaiPoint();
        connector.maxDistance = defaultMaxDistance;

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
        if (GUILayout.Button("���[�g����", GUILayout.Height(40)))
        {
            RouteGenerate();
        }


        EditorGUILayout.EndHorizontal();


    }

    // �����ɕK�v�ȏ�񂪓����Ă��Ȃ���΃f�t�H���g������
    private void EmptyCheck()
    {

        if (maxDistance == Vector2.zero)
        {
            maxDistance = defaultMaxDistance;
        }
        if (parent == null)
        {
            parent = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath.stageRoute);

        }
        if (prefab == null)
        {
            prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath.waiPoint);

        }

    }

    // �o�H����
    private void RouteGenerate()
    {
        // null�`�F�b�N
        if (connector == null)
        {
            connector = new ConnectWaiPoint();
        }
        connector.maxDistance = maxDistance;

        count = 0;
        // �e�I�u�W�F�N�g�̏���
        CheckParent();

        // �q�I�u�W�F�N�g�̃E�F�C�|�C���g�̐���
        SettingWaiPoints();
        AddSetWeiPoint();
        // �|�C���g�Ԃ��Ȃ���
        foreach (Transform child in parent.transform)
        {
            connector.Connect(child.GetComponent<Point>(), parent);
            Debug.Log(child.name);
        }
    }


    // �e�I�u�W�F�N�g�̐����܂��͎q�v�f�̍폜
    private void CheckParent()
    {
        if (!IsObjInScene(parent))
        {
            // �e�I�u�W�F�N�g�̃��[�g�̐���
            parent = PrefabUtility.InstantiatePrefab(parent) as GameObject;
        }
        else
        {
            // �q�I�u�W�F�N�g�̍폜
            for (int i = parent.transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(parent.transform.GetChild(i).gameObject);
            }

        }

        // Ctr+Z�Ŗ߂���悤��undo�ɒǉ�
        Undo.RegisterCreatedObjectUndo(parent, "Create Route");
        Selection.activeObject = parent;
    }



    // �|�C���g�̐���
    private void SettingWaiPoints()
    {

        var objects = Resources.FindObjectsOfTypeAll<GameObject>();
        pointList = new List<Point>();
        objectOnPoints = new Dictionary<GameObject, List<float>>();



        // ���ׂẴI�u�W�F�N�g���`�F�b�N
        foreach (var obj in objects)
        {

            // �V�[�����̃I�u�W�F�N�g����Ȃ�������Ώۂ�����
            if (!IsObjInScene(obj)) continue;

            // �K�w�̂���prefab�ŏd������̂�h��
            if (obj.transform.parent)
            {
                if (PrefabManager.isEqualBasePrefab(obj.transform.parent.gameObject, obj))
                {
                    continue;
                }
            }

            

            for (int i = 0; i < settingDatas.Count; i++)
            {
                // �|�C���g�����f�[�^�Ŏw�肳��Ă���I�u�W�F�N�g�������琶��
                if (PrefabManager.isEqualBasePrefab(settingDatas[i].obj, obj) && obj.GetComponent<SpriteRenderer>())
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

                case BasePoint.Top:
                    move.y = spriteRenderer.bounds.max.y;
                    break;
            }

            // �����ʒu�ɖ�肪�Ȃ����`�F�b�N
            Vector2 instantiatePosition = move + posList[j].position;
            if (CollisionCheck(instantiatePosition, spriteRenderer.gameObject))
            {
                //Debug.Log("���܂邽�ߐ������܂���ł���");
                continue;
            }
            Point point = InstantiateWaiPoint(instantiatePosition, posList[j].pointCategory);
            connectPoints[posList[j].basePoint] = point;
            //connectPoints.Add(posList[j].basePoint, point);


        }
        // �l���̃|�C���g�͐�ɂȂ��Ă���
        connector.ConnectCorners(connectPoints);

    }



    // �E�F�C�|�C���g�𐶐�����
    private Point InstantiateWaiPoint(Vector2 pos, PointCategory category)
    {
        // ����
        var pointObj = PrefabUtility.InstantiatePrefab(prefab, parent.transform) as GameObject;
        pointObj.transform.position = pos;

        count++;
        pointObj.name += "_"+count;

        Point point = pointObj.GetComponent<Point>();
        point.category = category;
        pointList.Add(point);

        // ����Ɗ֘A�t��
        RecordScaffold(pos);

        Undo.RegisterCreatedObjectUndo(pointObj, "Create WaiPoint");

        return point;
    }


    // ���ɂ��鑫��Ɗ֘A�t���ċL�^
    private void RecordScaffold(Vector2 position)
    {

        int layer = 1 << LayerNumber.floor | 1 << LayerNumber.scaffold;
        // �ǂ̃I�u�W�F�N�g�̏�ɐ������������L�^
        RaycastHit2D hit = Physics2D.Raycast(position, Vector2.down, 1.0f, layer);

        if (hit)
        {

            if (objectOnPoints.ContainsKey(hit.collider.gameObject))
            {

                objectOnPoints[hit.collider.gameObject].Add(position.x);

            }
            else
            {
                List<float> tempList = new List<float>();
                tempList.Add(position.x);
                objectOnPoints.Add(hit.collider.gameObject, tempList);
            }

        }
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

    // ���ԂɃE�F�C�|�C���g��ǉ�����
    private void AddSetWeiPoint()
    {
        // �|�C���g�����ʒu
        float downMargin = 0.4f;
        // �[����̈ʒu
        float sideMargin = 1.0f;

        float distanceRatio = 0.7f;

        Debug.Log("���Ԃɂ��|�C���g��ǉ����܂�");
        foreach (var item in objectOnPoints)
        {
            Debug.Log(item.Key.gameObject.name);
            Debug.Log(item.Value.Count);
            float[] posXarray = item.Value.ToArray();
            // ���ѕς���
            Array.Sort(posXarray);
            // �Ԋu�����ȏ�󂢂Ă�����|�C���g��ǉ�����
            SpriteRenderer spriteRenderer = item.Key.GetComponent<SpriteRenderer>();

            // ���ꂾ������[�����ׂ�
            if (item.Key.layer == LayerNumber.scaffold)
            {
                Debug.Log("����̒[���󂢂ĂȂ����`�F�b�N���܂�");
                if (Mathf.Abs( spriteRenderer.bounds.min.x - posXarray[0]) > maxDistance.x* distanceRatio)
                {
                    Debug.Log("���[�ɒǉ����܂�");
                    Vector2 leftPosition = new Vector2(spriteRenderer.bounds.max.x - sideMargin, spriteRenderer.bounds.max.y + downMargin);
                    InstantiateWaiPoint(leftPosition,PointCategory.Normal);
                }

                if (Mathf.Abs(spriteRenderer.bounds.max.x - posXarray[posXarray.Length-1]) > maxDistance.x* distanceRatio)
                {
                    Debug.Log("�E�[�ɒǉ����܂�");
                    Vector2 rightPosition = new Vector2(spriteRenderer.bounds.max.x - sideMargin, spriteRenderer.bounds.max.y + downMargin);
                    InstantiateWaiPoint(rightPosition, PointCategory.Normal);

                }

            }
      
            // �|�C���g�Ԃ��J���Ă��Ȃ����`�F�b�N
            for (int i = 0; i < posXarray.Length-1; i++)
            {
                if (Mathf.Abs(posXarray[i] - posXarray[i+1]) > maxDistance.x)
                {
                    Debug.Log("�|�C���g�Ԃ��󂫂����Ă���̂Œǉ����܂�");

                    Vector2 halfPosition = new Vector2((posXarray[i] + posXarray[i + 1]) / 2.0f, spriteRenderer.bounds.max.y + downMargin);
                    InstantiateWaiPoint(halfPosition, PointCategory.Normal);


                }
            }
        }

    }

}
#endif
