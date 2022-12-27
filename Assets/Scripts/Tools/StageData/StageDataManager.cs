using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
// 自作クラスの情報を使いたいためeditorフォルダには入れない
#if UNITY_EDITOR
using UnityEditor;

using Path;

public enum StagePartsCategory
{
    Scaffold,
    JumpRamp,
    Wall,
    NormalWall,
    ItemBox,

}

// ステージ内のオブジェクトのデータ
[System.Serializable]
public class StagePart
{
    public StagePartsCategory category;
    public Vector2 position;
    public Vector2 sizeMagnification = Vector2.one;
    public bool isNorth;
}

// 一つのステージのデータ
[System.Serializable]
public class StageData
{
    public List<StagePart> stageParts = new List<StagePart>();
    public Vector2 topRightPos;
    public Vector2 bottomLeftPos;
}


public class StageDataManager : EditorWindow
{
    [SerializeField]
    private StageData stageData;
    private Transform parent;

    string filePath;

    //スクロール位置
    private Vector2 _scrollPosition = Vector2.zero;


    // ウィンドウ
    [MenuItem("Window/Editor extention/StageDataManager", false, 1)]
    private static void ShowSetWaiPointWindow()
    {
        StageDataManager window = GetWindow<StageDataManager>();
        window.titleContent = new GUIContent("StageDataManager Window");

    }


    private void OnGUI()
    {

        parent = EditorGUILayout.ObjectField("Parent", parent, typeof(Transform), true) as Transform;
        //描画範囲が足りなければスクロール出来るように
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

        SerializedObject so = new SerializedObject(this);

        so.Update();

        SerializedProperty stringsProperty = so.FindProperty("stageData");

        EditorGUILayout.PropertyField(stringsProperty, true);

        so.ApplyModifiedProperties();

        

        //スクロール箇所終了
        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("ステージ生成！", GUILayout.Height(64)))
        {
            GenerateStage();
            if (parent)
            {
                Selection.activeObject = parent;
            }
        }
        if (GUILayout.Button("読み込みファイル選択", GUILayout.Height(64)))
        {

            filePath = EditorUtility.OpenFilePanel("select ", "Assets", "json");
            LoadFromJson();
        }
        if (GUILayout.Button("保存フォルダ選択", GUILayout.Height(64)))
        {

            SaveToJson(EditorUtility.SaveFilePanel("select ", "Assets", "StageData.json", "json"));
        }


    }

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

    private void GenerateStage()
    {
        // とりあえずオブジェクトの配置だけ

        foreach (var part in stageData.stageParts)
        {

            InstantiateStagePart(part);
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

        }
        return AssetDatabase.LoadAssetAtPath<GameObject>(path);
        
    }

    // 磁力の向きを設定するかどうか
    private bool IsSetTag(StagePartsCategory categry)
    {
        return categry == StagePartsCategory.JumpRamp || categry == StagePartsCategory.Wall;
    }





}
#endif