using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
public class PrefabManager : EditorWindow
{
    
    // �V�[�����ɂ���I�u�W�F�N�g���ǂ���
    public static bool IsObjInScene(GameObject obj)
    {
        string path = AssetDatabase.GetAssetOrScenePath(obj);
        return path.Contains(".unity");
    }


    // �Q�[���I�u�W�F�N�g���󂯎��A�����v���n�u�����ƂɂȂ��Ă����true��Ԃ�
    public static bool isEqualBasePrefab(GameObject gameObjectA, GameObject gameObjectB)
    {
        bool AisInScene = IsObjInScene(gameObjectA);
        bool BisInScene = IsObjInScene(gameObjectB);

        if (AisInScene && BisInScene)
        {
            
            
            return PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObjectA) ==
                        PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObjectB);
            
        }

        if (AisInScene)
        {
            return AssetDatabase.GetAssetOrScenePath(gameObjectB) ==
                PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObjectA);
        }

        if (BisInScene)
        {
            return AssetDatabase.GetAssetOrScenePath(gameObjectA) ==
                PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObjectB);
        }

        return AssetDatabase.GetAssetOrScenePath(gameObjectA) ==
                AssetDatabase.GetAssetOrScenePath(gameObjectB);

    }

    public static bool isEqualBasePrefab(string path, GameObject gameObject)
    {
        return path == PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObject);
    }

}
#endif
