using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using Path;

public class SetWaiPoint : EditorWindow
{

    private GameObject parent;
    private GameObject prefab;

    // ウィンドウ
    [MenuItem("Window/Editor extention/SetWaiPoint", false, 1)]
    private static void ShowSetWaiPointWindow()
    {
        SetWaiPoint window = GetWindow<SetWaiPoint>();
        window.titleContent = new GUIContent("SetWaiPoint Window");

    }

    private void OnGUI()
    {

        EmptyCheck();
        
        parent = EditorGUILayout.ObjectField("Parent", parent, typeof(GameObject), true) as GameObject;
        prefab = EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), true) as GameObject;


        // ボタンを押されたら
        if (GUILayout.Button("WaiPoint設置！"))
        {
            // 親オブジェクトのルートの生成
            GameObject route = PrefabUtility.InstantiatePrefab(parent) as GameObject;

            // 子オブジェクトのウェイポイントの生成
            var obj = PrefabUtility.InstantiatePrefab(prefab, route.transform);
            
            Undo.RegisterCreatedObjectUndo(route, "Create Route");
            Undo.RegisterCreatedObjectUndo(obj, "Create WaiPoint");

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



}
