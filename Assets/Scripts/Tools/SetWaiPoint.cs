using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// ����N���X�̏����g����������editor�t�H���_�ɂ͓���Ȃ�
#if UNITY_EDITOR
using UnityEditor;

using Path;

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

}

public class SetWaiPoint : EditorWindow
{

    private GameObject parent;
    private GameObject prefab;

    private int count;

    [SerializeField]
    private List<PointSettingData> settingDatas = new List<PointSettingData>();


    private float maxDintance;
    // �ŏI�I�ɂ�ChaserController�ɐݒ肵���l����|�C���g��u���ʒu�����߂���
    private ChaserController chaser;


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
        if (maxDintance <= 0.0f)
        {
            maxDintance = 3.0f;
        }

    }

    private void OnGUI()
    {

        EmptyCheck();

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

            InSceneCategorizeObjects();

            
        }

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

    // �V�[�����̃X�e�[�W�M�~�b�N����ޕ�������
    private void InSceneCategorizeObjects()
    {

        var objects = Resources.FindObjectsOfTypeAll<GameObject>();

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

            InstantiateWaiPoint(instantiatePosition);

        }
    }



    // �E�F�C�|�C���g�𐶐�����
    private void InstantiateWaiPoint(Vector2 pos)
    {

        var point = PrefabUtility.InstantiatePrefab(prefab, parent.transform) as GameObject;
        point.transform.position = pos;

        point.name += "_"+count;
        Debug.Log(point.name);

        count++;
        Undo.RegisterCreatedObjectUndo(point, "Create WaiPoint");
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


    private void ConnectWaiPoint(Point waiPoint)
    {
        Vector2 pos = waiPoint.transform.position;
        Gizmos.DrawWireCube(pos, Vector2.one);
        Physics2D.BoxCast(pos, Vector2.one, 0.0f, Vector2.right);
    }


}
#endif
