using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Path;

public class SetWaiPoint : EditorWindow
{

    private GameObject parent;
    private GameObject prefab;
    bool isSetJumpRamp;

    // 振り分ける用
    private List<GameObject> jumpRamps;
    private List<GameObject> walls;


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
        isSetJumpRamp = true;
    }

    private void OnGUI()
    {

        EmptyCheck();
        
        parent = EditorGUILayout.ObjectField("Parent", parent, typeof(GameObject), true) as GameObject;
        prefab = EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), true) as GameObject;


        isSetJumpRamp = EditorGUILayout.Toggle("JumpRanp",isSetJumpRamp);


        // ボタンを押されたら
        if (GUILayout.Button("WaiPoint設置！"))
        {
            GameObject route = parent;
            if (!IsObjInScene(parent))
            {
                // 親オブジェクトのルートの生成
                route = PrefabUtility.InstantiatePrefab(parent) as GameObject;
            }


            // Ctr+Zで戻せるようにundoに追加
            Undo.RegisterCreatedObjectUndo(route, "Create Route");

            // 子オブジェクトのウェイポイントの生成

            InSceneCategorizeObjects();

            if (isSetJumpRamp) SetJumpRampPoint(route);
            
            
            


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
        foreach (var obj in objects)
        {

            if (IsObjInScene(obj))
            {
                switch (PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(obj))
                {
                    case PrefabPath.jumpRamp:
                        jumpRamps.Add(obj);
                        break;
                    case PrefabPath.wall:
                        walls.Add(obj);
                        break;

                    default:
                        break;
                }


                

            }
        }
    }

    // ジャンプ台のポイントをセットする
    private void SetJumpRampPoint(GameObject parent)
    {
        if (jumpRamps.Count <= 0)
        {
            return;
        }
        foreach (var item in jumpRamps)
        {
            InstantiateWaiPoint(parent.transform, item.transform.position);

        }
    }

    // ウェイポイントを生成する
    private void InstantiateWaiPoint(Transform parent, Vector2 pos)
    {
        var point = PrefabUtility.InstantiatePrefab(prefab, parent.transform) as GameObject;
        point.transform.position = pos;
        Undo.RegisterCreatedObjectUndo(point, "Create WaiPoint");
    }

    // シーン内にいるオブジェクトかどうか
    private bool IsObjInScene(GameObject obj)
    {
        string path = AssetDatabase.GetAssetOrScenePath(obj);
        return path.Contains(".unity");
    }


}
