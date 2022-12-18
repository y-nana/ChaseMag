using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// �����N���b�N�őI�������I�u�W�F�N�g��y���Ŕ��]����G�f�B�^�[�g��
public class FlipHorizontal : EditorWindow
{

    // �E�B���h�E
    [MenuItem("Window/Editor extention/FlipHorizontal", false, 1)]
    private static void ShowFlipHorizontalWindow()
    {
        FlipHorizontal window = GetWindow<FlipHorizontal>();
        window.titleContent = new GUIContent("FlipHorizontal Window");
    }

    private void OnGUI()
    {
        
        // �{�^���������ꂽ��
        if (GUILayout.Button("�I�u�W�F�N�g�̈ʒu��X�����]�I�I"))
        {
            // �I������Ă���I�u�W�F�N�g���擾��
            var gameObjects = Selection.gameObjects;
            if (gameObjects != null)
            {
                foreach (var obj in gameObjects)
                {
                    // y���Ŕ��]������
                    Vector2 pos = obj.transform.position;
                    pos.x *= -1;
                    obj.transform.position = pos;
                    Debug.Log(obj.name+"�𔽓]���܂���");
                }
            }
        }
        
        
    }
}
