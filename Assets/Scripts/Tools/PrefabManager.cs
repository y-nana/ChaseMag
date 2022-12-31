using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
public class PrefabManager : EditorWindow
{
    
    // シーン内にいるオブジェクトかどうか
    public static bool IsObjInScene(GameObject obj)
    {
        string path = AssetDatabase.GetAssetOrScenePath(obj);
        return path.Contains(".unity");
    }


    // ゲームオブジェクトを二つ受け取り、同じプレハブがもとになっていればtrueを返す
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
