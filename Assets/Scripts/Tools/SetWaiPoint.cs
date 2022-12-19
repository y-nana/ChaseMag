using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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


    // 振り分ける用
    private List<GameObject> jumpRamps;
    private List<GameObject> walls;

    private Dictionary<GameObject, List<GameObject>> objectLists;

    private ChaserController chaser;


    //スクロール位置
    private Vector2 _scrollPosition = Vector2.zero;

    // ウィンドウ
    [MenuItem("Window/Editor extention/SetWaiPoint", false, 1)]
    private static void ShowSetWaiPointWindow()
    {
        SetWaiPoint window = GetWindow<SetWaiPoint>();
        window.titleContent = new GUIContent("SetWaiPoint Window");
        window.Init();

    }

    // 初期値セット
    private void Init()
    {
        parent = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath.stageRoute);
        prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath.waiPoint);
        if (settingDatas.Count <= 0)
        {

            PointSettingData data = new PointSettingData();
            data.obj = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath.jumpRamp);
            data.pointPosition = new List<PointPosition>();
            data.pointPosition.Add(new PointPosition(Vector2.zero, BasePoint.TopLeft));

            settingDatas.Add(data);
        }

    }

    private void OnGUI()
    {

        EmptyCheck();

        //描画範囲が足りなければスクロール出来るように
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


       

        //スクロール箇所終了
        EditorGUILayout.EndScrollView();

        // ボタンを押されたら
        if (GUILayout.Button("WaiPoint設置！", GUILayout.Height(64)))
        {

            count = 0;
            if (!IsObjInScene(parent))
            {
                // 親オブジェクトのルートの生成
                parent = PrefabUtility.InstantiatePrefab(parent) as GameObject;
            }
            else
            {
                Debug.Log(parent.transform.childCount);
                // 子オブジェクトの削除
                
                for (int i = parent.transform.childCount-1; i >= 0; i--)
                {
                    Debug.Log(parent.transform.GetChild(i).gameObject.name);
                    DestroyImmediate(parent.transform.GetChild(i).gameObject);
                }


            }


            // Ctr+Zで戻せるようにundoに追加
            Undo.RegisterCreatedObjectUndo(parent, "Create Route");


            // 子オブジェクトのウェイポイントの生成

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

    // シーン内のステージギミックを種類分けする
    private void InSceneCategorizeObjects()
    {

        var objects = Resources.FindObjectsOfTypeAll<GameObject>();

        jumpRamps = new List<GameObject>();
        walls = new List<GameObject>();
        objectLists = new Dictionary<GameObject, List<GameObject>>();

        // ディクショナリーの初期化
        for (int i = 0; i < settingDatas.Count; i++)
        {
            objectLists.Add(settingDatas[i].obj, new List<GameObject>());
        }

        // すべてのオブジェクトをチェック
        foreach (var obj in objects)
        {

            if (IsObjInScene(obj))
            {

                for (int i = 0; i < settingDatas.Count; i++)
                {
                    if (PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(settingDatas[i].obj) == PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(obj))
                    {

                        if (obj.GetComponent<SpriteRenderer>())
                        {
                            PositionSetting(i, obj.GetComponent<SpriteRenderer>());

                        }


                        break;
                    }

                }



            }
        }
    }


    // 一つのオブジェクトに対して、データから複数のポイントを生成
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

            InstantiateWaiPoint(move + posList[j].position);

        }
    }



    // ウェイポイントを生成する
    private void InstantiateWaiPoint(Vector2 pos)
    {
        var point = PrefabUtility.InstantiatePrefab(prefab, parent.transform) as GameObject;
        point.transform.position = pos;
        point.name += count;
        count++;
        Undo.RegisterCreatedObjectUndo(point, "Create WaiPoint");
    }


    // シーン内にいるオブジェクトかどうか
    private bool IsObjInScene(GameObject obj)
    {
        string path = AssetDatabase.GetAssetOrScenePath(obj);
        return path.Contains(".unity");
    }


}
