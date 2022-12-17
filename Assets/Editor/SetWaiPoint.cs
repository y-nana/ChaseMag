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

    // �U�蕪����p
    private List<GameObject> jumpRamps;
    private List<GameObject> walls;


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
        isSetJumpRamp = true;
    }

    private void OnGUI()
    {

        EmptyCheck();
        
        parent = EditorGUILayout.ObjectField("Parent", parent, typeof(GameObject), true) as GameObject;
        prefab = EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), true) as GameObject;


        isSetJumpRamp = EditorGUILayout.Toggle("JumpRanp",isSetJumpRamp);


        // �{�^���������ꂽ��
        if (GUILayout.Button("WaiPoint�ݒu�I"))
        {
            GameObject route = parent;
            if (!IsObjInScene(parent))
            {
                // �e�I�u�W�F�N�g�̃��[�g�̐���
                route = PrefabUtility.InstantiatePrefab(parent) as GameObject;
            }


            // Ctr+Z�Ŗ߂���悤��undo�ɒǉ�
            Undo.RegisterCreatedObjectUndo(route, "Create Route");

            // �q�I�u�W�F�N�g�̃E�F�C�|�C���g�̐���

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

    // �V�[�����̃X�e�[�W�M�~�b�N����ޕ�������
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

    // �W�����v��̃|�C���g���Z�b�g����
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

    // �E�F�C�|�C���g�𐶐�����
    private void InstantiateWaiPoint(Transform parent, Vector2 pos)
    {
        var point = PrefabUtility.InstantiatePrefab(prefab, parent.transform) as GameObject;
        point.transform.position = pos;
        Undo.RegisterCreatedObjectUndo(point, "Create WaiPoint");
    }

    // �V�[�����ɂ���I�u�W�F�N�g���ǂ���
    private bool IsObjInScene(GameObject obj)
    {
        string path = AssetDatabase.GetAssetOrScenePath(obj);
        return path.Contains(".unity");
    }


}
