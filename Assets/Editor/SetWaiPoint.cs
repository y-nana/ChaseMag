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
    private GameObject jumpRampPrefab;
    private List<GameObject> jumpRamps;


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
        jumpRampPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath.jumpRamp);
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
            // 親オブジェクトのルートの生成
            GameObject route = PrefabUtility.InstantiatePrefab(parent) as GameObject;
            // Ctr+Zで戻せるようにundoに追加
            Undo.RegisterCreatedObjectUndo(route, "Create Route");

            // 子オブジェクトのウェイポイントの生成

            InSceneCategorizeObjects();

            SetJumpRampPoint(route);
            
            


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

    private void InSceneCategorizeObjects()
    {
        var objects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (var obj in objects)
        {
            string path = AssetDatabase.GetAssetOrScenePath(obj);
            bool isScene = path.Contains(".unity");
            if (isScene)
            {

                if (isSetJumpRamp && PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(obj) == PrefabPath.jumpRamp)
                {

                    jumpRamps.Add(obj);


                }
                

            }
        }
    }


    private void SetJumpRampPoint(GameObject parent)
    {
        foreach (var item in jumpRamps)
        {
            var point = PrefabUtility.InstantiatePrefab(prefab, parent.transform) as GameObject;
            point.transform.position = item.transform.position;
            Undo.RegisterCreatedObjectUndo(point, "Create WaiPoint");

        }
    }


}
