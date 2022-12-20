using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// 自作クラスの情報を使いたいためeditorフォルダには入れない
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
    // 最終的にはChaserControllerに設定した値からポイントを置く位置を求めたい
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

        // デフォルトの距離を入れる
        if (maxDintance <= 0.0f)
        {
            maxDintance = 3.0f;
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

            count = 1;
            if (!IsObjInScene(parent))
            {
                // 親オブジェクトのルートの生成
                parent = PrefabUtility.InstantiatePrefab(parent) as GameObject;
            }
            else
            {
                // 子オブジェクトの削除
                
                for (int i = parent.transform.childCount-1; i >= 0; i--)
                {

                    DestroyImmediate(parent.transform.GetChild(i).gameObject);
                }


            }


            // Ctr+Zで戻せるようにundoに追加
            Undo.RegisterCreatedObjectUndo(parent, "Create Route");

            Selection.activeObject = parent;

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

        // すべてのオブジェクトをチェック
        foreach (var obj in objects)
        {

            // シーン内のオブジェクトじゃなかったら対象を次へ
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

            // 生成位置に問題がないかチェック
            Vector2 instantiatePosition = move + posList[j].position;
            if (CollisionCheck(instantiatePosition, spriteRenderer.gameObject))
            {
                Debug.Log("埋まるため生成しませんでした");
                continue;
            }

            InstantiateWaiPoint(instantiatePosition);

        }
    }



    // ウェイポイントを生成する
    private void InstantiateWaiPoint(Vector2 pos)
    {

        var point = PrefabUtility.InstantiatePrefab(prefab, parent.transform) as GameObject;
        point.transform.position = pos;

        point.name += "_"+count;
        Debug.Log(point.name);

        count++;
        Undo.RegisterCreatedObjectUndo(point, "Create WaiPoint");
    }


    // シーン内にいるオブジェクトかどうか
    private bool IsObjInScene(GameObject obj)
    {
        string path = AssetDatabase.GetAssetOrScenePath(obj);
        return path.Contains(".unity");
    }


    // ギミック以外のオブジェクトに埋まっているかどうか
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


    // ゲームオブジェクトを二つ受け取り、同じプレハブがもとになっていればtrueを返す
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
