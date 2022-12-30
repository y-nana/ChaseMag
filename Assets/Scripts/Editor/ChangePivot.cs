using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
// �J���r����sprite��pivot��ς����Ƃ�
// �Q�Ƃ��Ă���Q�[���I�u�W�F�N�g�̈ʒu�̂���𒼂��G�f�B�^�[�g��
// spriteRenderer�̃p�X����擾����΂悩����
public class ChangePivot : EditorWindow
{

    [SerializeField]
    private List<GameObject> prefabs;


    private bool centerToBottom;
    private bool centerToToTop;

    // �E�B���h�E
    [MenuItem("Window/Editor extention/ChangePivot", false, 1)]
    private static void ShowChangePivotWindow()
    {
        ChangePivot window = GetWindow<ChangePivot>();
        window.titleContent = new GUIContent("ChangePivot Window");

    }
    private void OnGUI()
    {
        SerializedObject so = new SerializedObject(this);

        so.Update();

        SerializedProperty stringsProperty = so.FindProperty("prefabs");

        EditorGUILayout.PropertyField(stringsProperty, true);

        so.ApplyModifiedProperties();
        centerToBottom = EditorGUILayout.ToggleLeft("isToBottom", centerToBottom);
        centerToToTop = EditorGUILayout.ToggleLeft("isToTop", centerToToTop);
        if (GUILayout.Button("�I�u�W�F�N�g�ړ�", GUILayout.Height(64)))
        {

            MoveGameobject();
        }
    }

    private void MoveGameobject()
    {
        List<GameObject> objects = GetGameObject();
        Debug.Log(objects.Count);
        foreach (var obj in objects)
        {

            if (!obj.GetComponent<SpriteRenderer>())
            {
                continue;
            }
            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();

            Vector3 temp = obj.transform.localEulerAngles;
            obj.transform.localEulerAngles  = Vector3.zero;

            Vector2 deltaMove = Vector2.zero;
            if (centerToBottom) 
            {
                deltaMove = new Vector2(0.0f, sr.bounds.size.y / 2.0f);
                deltaMove *= -1;
            }
            if (centerToToTop)
            {
                deltaMove = new Vector2(0.0f, sr.bounds.size.y / 2.0f);
            }
            Debug.Log(obj.name+deltaMove);
            deltaMove = Quaternion.Euler(0, 0, temp.z).normalized * deltaMove;


            Vector2 pos = obj.transform.position;
            pos += deltaMove;
            obj.transform.position = pos;
            obj.transform.localEulerAngles = temp;

            Undo.RegisterCompleteObjectUndo(obj.transform,"moveGameObject");
        }
    }




    private List<GameObject> GetGameObject()
    {
        List<GameObject> objects = new List<GameObject>();

        var allObjects = Resources.FindObjectsOfTypeAll<GameObject>();

        
        // ���ׂẴI�u�W�F�N�g���`�F�b�N
        foreach (var obj in allObjects)
        {

            // �V�[�����̃I�u�W�F�N�g����Ȃ�������Ώۂ�����
            if (!IsObjInScene(obj)) continue;

            for (int i = 0; i < prefabs.Count; i++)
            {
                if (isEqualBasePrefab(prefabs[i], obj))
                {
                    objects.Add(obj);
                }
            }

            
        }

        return objects;

    }

    // �V�[�����ɂ���I�u�W�F�N�g���ǂ���
    private bool IsObjInScene(GameObject obj)
    {
        string path = AssetDatabase.GetAssetOrScenePath(obj);
        return path.Contains(".unity");
    }

    // �Q�[���I�u�W�F�N�g���󂯎��A�����v���n�u�����ƂɂȂ��Ă����true��Ԃ�
    private bool isEqualBasePrefab(GameObject prefab, GameObject gameObjectB)
    {

        Debug.Log(AssetDatabase.GetAssetOrScenePath(prefab));
        Debug.Log(PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObjectB));

        return AssetDatabase.GetAssetOrScenePath(prefab) ==
            PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(gameObjectB);

    }
}
