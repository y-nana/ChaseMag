using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FlipHorizontal : EditorWindow
{

    //private string text = "";

    [MenuItem("Window/Editor extention/FlipHorizontal", false, 1)]
    private static void ShowWindow()
    {
        FlipHorizontal window = GetWindow<FlipHorizontal>();
        window.titleContent = new GUIContent("FlipHorizontal Window");
    }

    private void OnGUI()
    {
        /*
        GUILayout.Label("���̕�������o�͂����");
        text = EditorGUILayout.TextArea(text, GUILayout.Height(100));
        */
        
        if (GUILayout.Button("�I�u�W�F�N�g�̈ʒu��X�����]�I�I"))
        {
            var gameObjects = Selection.gameObjects;
            if (gameObjects != null)
            {
                foreach (var obj in gameObjects)
                {
                    Vector2 pos = obj.transform.position;
                    pos.x *= -1;
                    obj.transform.position = pos;
                    Debug.Log(obj.name+"�𔽓]���܂���");
                }
            }
        }
        
        
    }
}
