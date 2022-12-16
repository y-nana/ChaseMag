using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// ワンクリックで選択したオブジェクトをy軸で反転するエディター拡張
public class FlipHorizontal : EditorWindow
{

    // ウィンドウ
    [MenuItem("Window/Editor extention/FlipHorizontal", false, 1)]
    private static void ShowFlipHorizontalWindow()
    {
        FlipHorizontal window = GetWindow<FlipHorizontal>();
        window.titleContent = new GUIContent("FlipHorizontal Window");
    }

    private void OnGUI()
    {
        
        // ボタンを押されたら
        if (GUILayout.Button("オブジェクトの位置をX軸反転！！"))
        {
            // 選択されているオブジェクトを取得し
            var gameObjects = Selection.gameObjects;
            if (gameObjects != null)
            {
                foreach (var obj in gameObjects)
                {
                    // y軸で反転させる
                    Vector2 pos = obj.transform.position;
                    pos.x *= -1;
                    obj.transform.position = pos;
                    Debug.Log(obj.name+"を反転しました");
                }
            }
        }
        
        
    }
}
