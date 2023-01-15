using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
// 自作クラスの情報を使いたいためeditorフォルダには入れない
#if UNITY_EDITOR
using UnityEditor;

using Path;
public class StageDataManager : EditorWindow
{
    [SerializeField]
    private StageData stageData;
    private Transform parent;
    private Transform celling;
    private Transform rightWall;
    private Transform leftWall;

    private int partsCount;

    string filePath;

    //スクロール位置
    private Vector2 dataScrollPosition = Vector2.zero;
    private Vector2 buttonScrollPosition = Vector2.zero;

    private Dictionary<StagePartsCategory, string> pathList;

    // ウィンドウ
    [MenuItem("Window/Editor extention/StageDataManager", false, 1)]
    private static void ShowSetWaiPointWindow()
    {
        StageDataManager window = GetWindow<StageDataManager>();
        window.titleContent = new GUIContent("StageDataManager Window");
        window.Init();
    }

    private void Init()
    {
        pathList = new Dictionary<StagePartsCategory, string>();
        pathList.Add(StagePartsCategory.Scaffold, PrefabPath.scaffold);
        pathList.Add(StagePartsCategory.JumpRamp, PrefabPath.jumpRamp);
        pathList.Add(StagePartsCategory.Wall, PrefabPath.wall);
        pathList.Add(StagePartsCategory.NormalWall, PrefabPath.normalWall);
        pathList.Add(StagePartsCategory.ItemBox, PrefabPath.itemBox);



    }

    private void OnGUI()
    {
        //描画範囲が足りなければスクロール出来るように
        dataScrollPosition = EditorGUILayout.BeginScrollView(dataScrollPosition);
        parent = EditorGUILayout.ObjectField("Parent", parent, typeof(Transform), true) as Transform;
        celling = EditorGUILayout.ObjectField("celling", celling, typeof(Transform), true) as Transform;
        rightWall = EditorGUILayout.ObjectField("rightWall", rightWall, typeof(Transform), true) as Transform;
        leftWall = EditorGUILayout.ObjectField("leftWall", leftWall, typeof(Transform), true) as Transform;


        SerializedObject so = new SerializedObject(this);

        so.Update();

        SerializedProperty stringsProperty = so.FindProperty("stageData");

        EditorGUILayout.PropertyField(stringsProperty, true);

        so.ApplyModifiedProperties();

        

        //スクロール箇所終了
        EditorGUILayout.EndScrollView();

        buttonScrollPosition = EditorGUILayout.BeginScrollView(buttonScrollPosition);

        if (GUILayout.Button("ステージ生成", GUILayout.Height(40)))
        {
            CheckParent();
            GenerateStage();
            if (parent)
            {
                Selection.activeObject = parent;
            }
        }
        if (GUILayout.Button("読み込みファイル選択", GUILayout.Height(40)))
        {

            filePath = EditorUtility.OpenFilePanel("select ", "Assets", "json");
            LoadFromJson();
        }
        if (GUILayout.Button("保存フォルダ選択", GUILayout.Height(40)))
        {

            SaveToJson(EditorUtility.SaveFilePanel("select ", "Assets", "StageData.json", "json"));
        }

        if (GUILayout.Button("シーン内からデータにセット", GUILayout.Height(40)))
        {

            SceneToData();
        }
        //スクロール箇所終了
        EditorGUILayout.EndScrollView();

    }



    // シーン内のオブジェクトからステージデータへ流し込む
    private void SceneToData()
    {

        var allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        stageData = new StageData();
        stageData.stageParts.Clear();

        // すべてのオブジェクトをチェック
        foreach (var obj in allObjects)
        {

            // シーン内のオブジェクトじゃなかったら対象を次へ
            if (!PrefabManager.IsObjInScene(obj)) continue;

            foreach (StagePartsCategory value in Enum.GetValues(typeof(StagePartsCategory)))
            {
                
                if (PrefabManager.isEqualBasePrefab(GetPrefab(value), obj))
                {
                    if (obj.CompareTag("GoStop"))
                    {
                        break;
                    }
                    // 階層のあるprefabで重複するのを防ぐ
                    if (obj.transform.parent)
                    {
                        if (PrefabManager.isEqualBasePrefab(obj.transform.parent.gameObject, obj))
                        {
                            break;
                        }
                    }


                    StagePart part = new StagePart();
                    part.category = value;
                    part.position = obj.transform.position;
                    part.sizeMagnification = GetSizeMagnification(obj.transform.localScale ,GetPrefab(value).transform.localScale);
                    if (IsSetTag(value))
                    {
                        part.isNorth = obj.tag == Tag.PoleTag.north;
                    }
                    stageData.stageParts.Add(part);
                }
            }

        }


        if (celling && rightWall && leftWall)
        {
            stageData.height = celling.position.y;
            stageData.width = rightWall.position.x + Mathf.Abs(leftWall.position.x);
        }

        OnGUI();

    }

    // jsonファイルとして出力
    private void SaveToJson(string path)
    {
        if (path == string.Empty)
        {
            Debug.Log("フォルダが選択されていません");
            return;
        }

        string jsonstr = JsonUtility.ToJson(stageData);
        StreamWriter writer = new StreamWriter(path, false);
        writer.WriteLine(jsonstr);
        writer.Flush();
        writer.Close();
    }

    // jsonファイルから読み込み
    private void LoadFromJson()
    {
        if (filePath == string.Empty)
        {
            Debug.Log("ファイルが選択されていません");
            return;
        }

        // ファイルを読み込んでデータに流す
        StreamReader reader = new StreamReader(filePath);
        string datastr = reader.ReadToEnd();
        reader.Close();
        JsonUtility.FromJsonOverwrite(datastr, stageData);
    }

    // 親オブジェクトの生成または子要素の削除
    private void CheckParent()
    {
        if (parent)
        {
            // 子オブジェクトの削除
            for (int i = parent.transform.childCount - 1; i >= 0; i--)
            {

                DestroyImmediate(parent.transform.GetChild(i).gameObject);
            }

        }
        else
        {
            // 親オブジェクトのルートの生成
            parent = new GameObject("StageGimmicks").transform;
        }
    }

    // ステージデータからシーン上へ生成する
    private void GenerateStage()
    {
        partsCount = 0;
        foreach (var part in stageData.stageParts)
        {

            InstantiateStagePart(part);
        }


        if (celling && rightWall && leftWall)
        {
            float half = 0.5f;
            celling.position = new Vector2( 0.0f, stageData.height);
            rightWall.position = new Vector2(stageData.width * half, stageData.height * half);
            leftWall.position = new Vector2(-stageData.width * half, stageData.height * half);
        }
    }

    // パーツの生成をする
    private void InstantiateStagePart(StagePart partData)
    {
        GameObject baseObject = GetPrefab(partData.category);
        if (!baseObject) return;

        GameObject gameObject = PrefabUtility.InstantiatePrefab(baseObject,parent) as GameObject;
        gameObject.transform.position = partData.position;
        gameObject.transform.localScale *= partData.sizeMagnification;

        if (IsSetTag(partData.category))
        {
            if (partData.isNorth)
            {
                gameObject.tag = Tag.PoleTag.north;
            }
            else
            {
                gameObject.tag = Tag.PoleTag.south;

            }
        }

        partsCount++;
        gameObject.name += "_" + partsCount;

        Undo.RegisterCreatedObjectUndo(gameObject, "Create stageObject");


    }

    // パーツのプレハブを取得する
    private GameObject GetPrefab(StagePartsCategory category)
    {
        string path = null;
        switch (category)
        {
            case StagePartsCategory.Scaffold:
                path = PrefabPath.scaffold;
                break;
            case StagePartsCategory.JumpRamp:
                path = PrefabPath.jumpRamp;

                break;
            case StagePartsCategory.Wall:
                path = PrefabPath.wall;

                break;
            case StagePartsCategory.NormalWall:
                path = PrefabPath.normalWall;

                break;
            case StagePartsCategory.ItemBox:
                path = PrefabPath.itemBox;

                break;
            case StagePartsCategory.PoleScaffold:
                path = PrefabPath.poleScaffold;

                break;
        }
        return AssetDatabase.LoadAssetAtPath<GameObject>(path);

    }

    // 磁力の向きを設定するパーツかどうか
    private bool IsSetTag(StagePartsCategory categry)
    {
        return categry == StagePartsCategory.JumpRamp 
            || categry == StagePartsCategory.Wall
            || categry == StagePartsCategory.PoleScaffold;
    }

    // スケールから倍率へ
    private Vector2 GetSizeMagnification(Vector2 scale, Vector2 baseScale)
    {

        return new Vector2(scale.x / baseScale.x, scale.y / baseScale.y);
    }


}
#endif