using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// 自作クラスの情報を使いたいためeditorフォルダには入れない
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

    // ポイントを追加設置するときに使う
    private Dictionary<GameObject, List<float>> objectOnPoints = new Dictionary<GameObject, List<float>>();

    // つなげるときに使う
    private List<Point> pointList = new List<Point>();

    private ConnectWaiPoint connector;

    //private float maxDintance;
    // 最終的にはChaserControllerに設定した値からポイントを置く位置を求めたい
    private ChaserController chaser;

    Vector2 maxDistance;

    //スクロール位置
    private Vector2 _scrollPosition = Vector2.zero;

    // デフォルトの値
    private readonly Vector2 defaultMaxDistance = new Vector2(10.0f, 5.0f);
    private readonly Vector2 defaultWallBottomPosition = new Vector2(1.0f, 0.5f);
    private readonly Vector2 defaultWallMiddlePosition = new Vector2(1.0f, 0);
    private readonly Vector2 defaultPoleScaffoldPosition = new Vector2(0, 1.0f);
    private readonly Vector2 defaultScaffoldPosition = new Vector2(0, 0.4f);



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
            // デフォルトの生成位置の情報を入れる

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

        // デフォルトの距離を入れる
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

        EditorGUILayout.BeginHorizontal();
        // ボタンを押されたら
        if (GUILayout.Button("ルート生成", GUILayout.Height(40)))
        {
            RouteGenerate();
        }


        EditorGUILayout.EndHorizontal();


    }

    // 生成に必要な情報が入っていなければデフォルトを入れる
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

    // 経路生成
    private void RouteGenerate()
    {
        // nullチェック
        if (connector == null)
        {
            connector = new ConnectWaiPoint();
        }
        connector.maxDistance = maxDistance;

        count = 0;
        // 親オブジェクトの準備
        CheckParent();

        // 子オブジェクトのウェイポイントの生成
        SettingWaiPoints();
        AddSetWeiPoint();
        // ポイント間をつなげる
        foreach (Transform child in parent.transform)
        {
            connector.Connect(child.GetComponent<Point>(), parent);
            Debug.Log(child.name);
        }
    }


    // 親オブジェクトの生成または子要素の削除
    private void CheckParent()
    {
        if (!IsObjInScene(parent))
        {
            // 親オブジェクトのルートの生成
            parent = PrefabUtility.InstantiatePrefab(parent) as GameObject;
        }
        else
        {
            // 子オブジェクトの削除
            for (int i = parent.transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(parent.transform.GetChild(i).gameObject);
            }

        }

        // Ctr+Zで戻せるようにundoに追加
        Undo.RegisterCreatedObjectUndo(parent, "Create Route");
        Selection.activeObject = parent;
    }



    // ポイントの生成
    private void SettingWaiPoints()
    {

        var objects = Resources.FindObjectsOfTypeAll<GameObject>();
        pointList = new List<Point>();
        objectOnPoints = new Dictionary<GameObject, List<float>>();



        // すべてのオブジェクトをチェック
        foreach (var obj in objects)
        {

            // シーン内のオブジェクトじゃなかったら対象を次へ
            if (!IsObjInScene(obj)) continue;

            // 階層のあるprefabで重複するのを防ぐ
            if (obj.transform.parent)
            {
                if (PrefabManager.isEqualBasePrefab(obj.transform.parent.gameObject, obj))
                {
                    continue;
                }
            }

            

            for (int i = 0; i < settingDatas.Count; i++)
            {
                // ポイント生成データで指定されているオブジェクトだったら生成
                if (PrefabManager.isEqualBasePrefab(settingDatas[i].obj, obj) && obj.GetComponent<SpriteRenderer>())
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

            // 生成位置に問題がないかチェック
            Vector2 instantiatePosition = move + posList[j].position;
            if (CollisionCheck(instantiatePosition, spriteRenderer.gameObject))
            {
                //Debug.Log("埋まるため生成しませんでした");
                continue;
            }
            Point point = InstantiateWaiPoint(instantiatePosition, posList[j].pointCategory);
            connectPoints[posList[j].basePoint] = point;
            //connectPoints.Add(posList[j].basePoint, point);


        }
        // 四隅のポイントは先につなげておく
        connector.ConnectCorners(connectPoints);

    }



    // ウェイポイントを生成する
    private Point InstantiateWaiPoint(Vector2 pos, PointCategory category)
    {
        // 生成
        var pointObj = PrefabUtility.InstantiatePrefab(prefab, parent.transform) as GameObject;
        pointObj.transform.position = pos;

        count++;
        pointObj.name += "_"+count;

        Point point = pointObj.GetComponent<Point>();
        point.category = category;
        pointList.Add(point);

        // 足場と関連付け
        RecordScaffold(pos);

        Undo.RegisterCreatedObjectUndo(pointObj, "Create WaiPoint");

        return point;
    }


    // 下にある足場と関連付けて記録
    private void RecordScaffold(Vector2 position)
    {

        int layer = 1 << LayerNumber.floor | 1 << LayerNumber.scaffold;
        // どのオブジェクトの上に生成したかを記録
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

    // 隙間にウェイポイントを追加する
    private void AddSetWeiPoint()
    {
        // ポイント生成位置
        float downMargin = 0.4f;
        // 端からの位置
        float sideMargin = 1.0f;

        float distanceRatio = 0.7f;

        Debug.Log("隙間にもポイントを追加します");
        foreach (var item in objectOnPoints)
        {
            Debug.Log(item.Key.gameObject.name);
            Debug.Log(item.Value.Count);
            float[] posXarray = item.Value.ToArray();
            // 並び変える
            Array.Sort(posXarray);
            // 間隔が一定以上空いていたらポイントを追加する
            SpriteRenderer spriteRenderer = item.Key.GetComponent<SpriteRenderer>();

            // 足場だったら端から比べる
            if (item.Key.layer == LayerNumber.scaffold)
            {
                Debug.Log("足場の端が空いてないかチェックします");
                if (Mathf.Abs( spriteRenderer.bounds.min.x - posXarray[0]) > maxDistance.x* distanceRatio)
                {
                    Debug.Log("左端に追加します");
                    Vector2 leftPosition = new Vector2(spriteRenderer.bounds.max.x - sideMargin, spriteRenderer.bounds.max.y + downMargin);
                    InstantiateWaiPoint(leftPosition,PointCategory.Normal);
                }

                if (Mathf.Abs(spriteRenderer.bounds.max.x - posXarray[posXarray.Length-1]) > maxDistance.x* distanceRatio)
                {
                    Debug.Log("右端に追加します");
                    Vector2 rightPosition = new Vector2(spriteRenderer.bounds.max.x - sideMargin, spriteRenderer.bounds.max.y + downMargin);
                    InstantiateWaiPoint(rightPosition, PointCategory.Normal);

                }

            }
      
            // ポイント間が開いていないかチェック
            for (int i = 0; i < posXarray.Length-1; i++)
            {
                if (Mathf.Abs(posXarray[i] - posXarray[i+1]) > maxDistance.x)
                {
                    Debug.Log("ポイント間が空きすぎているので追加します");

                    Vector2 halfPosition = new Vector2((posXarray[i] + posXarray[i + 1]) / 2.0f, spriteRenderer.bounds.max.y + downMargin);
                    InstantiateWaiPoint(halfPosition, PointCategory.Normal);


                }
            }
        }

    }

}
#endif
