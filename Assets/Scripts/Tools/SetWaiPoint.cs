using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// 自作クラスの情報を使いたいためeditorフォルダには入れない
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
    [field: SerializeField]
    public PointCategory pointCategory { get; set; }


    public PointPosition()
    {
        this.position = Vector2.zero;
        this.basePoint = BasePoint.Center;
        this.pointCategory = PointCategory.Normal;
    }

    
    public PointPosition(Vector2 pos, BasePoint basePoint, PointCategory category)
    {
        this.position = pos;
        this.basePoint = basePoint;
        this.pointCategory = category;
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

    // ポイントを追加設置するときに使う
    private Dictionary<GameObject, List<float>> objectOnPoints = new Dictionary<GameObject, List<float>>();

    // つなげるときに使う
    private List<Point> pointList = new List<Point>();
    //private float maxDintance;
    // 最終的にはChaserControllerに設定した値からポイントを置く位置を求めたい
    private ChaserController chaser;

    Vector2 maxDistance;



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
            data.pointPosition.Add(new PointPosition(Vector2.zero, BasePoint.Center,PointCategory.CanJump));
            settingDatas.Add(data);

            PointSettingData wallData = new PointSettingData();
            wallData.obj = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath.wall);
            wallData.pointPosition = new List<PointPosition>();
            wallData.pointPosition.Add(new PointPosition(Vector2.zero, BasePoint.TopLeft, PointCategory.Normal));
            wallData.pointPosition.Add(new PointPosition(Vector2.zero, BasePoint.TopRight, PointCategory.Normal));
            wallData.pointPosition.Add(new PointPosition(new Vector2(1.0f, 0.5f), BasePoint.BottomRight, PointCategory.Normal));
            wallData.pointPosition.Add(new PointPosition(new Vector2(-1.0f, 0.5f), BasePoint.BottomLeft, PointCategory.Normal));

            wallData.pointPosition.Add(new PointPosition(new Vector2(1.0f, 0), BasePoint.Center, PointCategory.Floating));
            wallData.pointPosition.Add(new PointPosition(new Vector2(-1.0f, 0), BasePoint.Center, PointCategory.Floating));
            


            settingDatas.Add(wallData);

        }

        

        // デフォルトの距離を入れる
        if (maxDistance == Vector2.zero)
        {
            maxDistance = new Vector2(10.0f, 5.0f);
        }

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

            SettingWaiPoints();
            AddSetWeiPoint();
            foreach (Transform child in parent.transform)
            {

                ConnectWaiPoint(child.GetComponent<Point>());
                Debug.Log(child.name);
            }

        }


        EditorGUILayout.EndHorizontal();


    }
    

    private void EmptyCheck()
    {

        if (maxDistance == Vector2.zero)
        {
            maxDistance = new Vector2(10.0f, 5.0f);
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

        ConnectWaiPoins(connectPoints);

    }



    // ウェイポイントを生成する
    private Point InstantiateWaiPoint(Vector2 pos, PointCategory category)
    {

        var pointObj = PrefabUtility.InstantiatePrefab(prefab, parent.transform) as GameObject;
        pointObj.transform.position = pos;

        pointObj.name += "_"+count;
        Point point = pointObj.GetComponent<Point>();
        point.category = category;
        pointList.Add(point);

        int layer = 1 << LayerNumber.floor | 1 << LayerNumber.scaffold;
        // どのオブジェクトの上に生成したかを記録
        RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.down, 1.0f,layer);

        if (hit)
        {

            if (objectOnPoints.ContainsKey(hit.collider.gameObject))
            {

                objectOnPoints[hit.collider.gameObject].Add(pos.x);

            }
            else
            {
                List<float> tempList = new List<float>();
                tempList.Add(pos.x);
                objectOnPoints.Add(hit.collider.gameObject, tempList);
            }

            Debug.Log(pointObj.name);
        }


        count++;
        Undo.RegisterCreatedObjectUndo(pointObj, "Create WaiPoint");

        return point;
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


    // ゲームオブジェクトを二つ受け取り、同じプレハブがもとになっていればtrueを返す
    private bool isEqualBasePrefab(GameObject gameObjectA, GameObject gameObjectB)
    {
        return PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObjectA) ==
            PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObjectB);

    }

    // 隙間にウェイポイントを追加する
    private void AddSetWeiPoint()
    {

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
                if (Mathf.Abs( spriteRenderer.bounds.min.x - posXarray[0]) > maxDistance.x*0.7f)
                {
                    Debug.Log("左端に追加します");

                    InstantiateWaiPoint(new Vector2(spriteRenderer.bounds.min.x + 1.0f, spriteRenderer.bounds.max.y+0.5f),PointCategory.Normal);
                }

                if (Mathf.Abs(spriteRenderer.bounds.max.x - posXarray[posXarray.Length-1]) > maxDistance.x*0.7f)
                {
                    Debug.Log("右端に追加します");

                    InstantiateWaiPoint(new Vector2(spriteRenderer.bounds.max.x - 1.0f, spriteRenderer.bounds.max.y + 0.5f), PointCategory.Normal);

                }

            }
      

            for (int i = 0; i < posXarray.Length-1; i++)
            {
                if (Mathf.Abs(posXarray[i] - posXarray[i+1]) > maxDistance.x)
                {
                    Debug.Log("ポイント間が空きすぎているので追加します");

                    InstantiateWaiPoint(new Vector2((posXarray[i] + posXarray[i + 1])/2.0f, spriteRenderer.bounds.max.y + 0.5f), PointCategory.Normal);


                }
            }
        }
    }

    // ウェイポイントをつなげる
    private void ConnectWaiPoint(Point waiPoint)
    {

        // foreach (Point toPoint in pointList)
        foreach (Transform toPointTransform in parent.transform)
        {
            Point toPoint = toPointTransform.GetComponent<Point>();
            Vector2 pos = waiPoint.transform.position;

            Vector2 toPos = toPoint.transform.position;



            // 比較ポイントが自身か、遠すぎる場合はパス
            if (toPoint.gameObject == waiPoint.gameObject )
            {
                Debug.Log(waiPoint.name + "から" + toPoint.name + "は自分自身です");

                continue;
            }

            Vector2 dis = toPos - pos;
            if (waiPoint.category != PointCategory.Floating&& (Mathf.Abs(dis.x) > maxDistance.x || Mathf.Abs(dis.y) > maxDistance.y))
            {
                Debug.Log(waiPoint.name + "から" + toPoint.name + "は距離が遠すぎます");
                continue;

            }

            if (waiPoint.category == PointCategory.Floating && (Mathf.Abs(dis.x) > 1.0f || dis.y > 0.0f))
            {
                Debug.Log(waiPoint.name + "から" + toPoint.name + "は落下時たどり着けません");

                continue;
            }


            if (waiPoint.category == PointCategory.Normal && dis.y >1.0f)
            {
                Debug.Log(waiPoint.name + "から" + toPoint.name + "はジャンプできないので届きませんでした");
                continue;

            }



            // rayを飛ばして、つながるかどうか判定
            Debug.Log(waiPoint.name + "から" + toPoint.name + "にレイを飛ばしてみます");

            waiPoint.gameObject.layer = LayerNumber.ignoreRaycast;


            //int layer = 1 << LayerNumber.waiPoint | 1 << LayerNumber.wall;
            int layer = 1 << LayerNumber.wall;
            if (dis.y < 0)
            {
                layer |= 1 << LayerNumber.scaffold;
            }
            Debug.Log("レイヤー："+Convert.ToString(layer, 2));
            Debug.Log(toPoint.transform.position);
            

            RaycastHit2D hit = Physics2D.Raycast(pos, dis, dis.magnitude, layer);
            
            Debug.Log(dis);
            // 次のポイントまでレイを飛ばして障害物がなかった場合
            if (!hit)
            {
                if (Mathf.Abs(dis.y) < 1.0f && Mathf.Abs(dis.x) > maxDistance.x / 2.0f)
                {
                    // 到達までに足場がつながっていなかったら追加しない
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
                        Debug.Log(waiPoint.name + "から" + toPoint.name + "は足場がありません");
                        waiPoint.gameObject.layer = LayerNumber.waiPoint;

                        continue;
                    }
                }
                waiPoint.adjacentList.Add(toPointTransform);


            }
            else if (dis.y < -2.0f)
            {
                if (dis.y < -2.0f)
                {
                    Debug.Log(dis.y);
                    // 足場から降りることでたどりつけるポイントだったらつなげる

                    // rayを飛ばして、つながるかどうか判定
                    toPoint.gameObject.layer = LayerNumber.ignoreRaycast;

                    Debug.Log("降りれる？");
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

    // 四隅にあるポイントをつなげる
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
