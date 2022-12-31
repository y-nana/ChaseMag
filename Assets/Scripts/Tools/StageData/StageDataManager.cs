using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
// ����N���X�̏����g����������editor�t�H���_�ɂ͓���Ȃ�
#if UNITY_EDITOR
using UnityEditor;

using Path;

public enum StagePartsCategory
{
    Scaffold,
    JumpRamp,
    Wall,
    NormalWall,
    ItemBox,

}

// �X�e�[�W���̃I�u�W�F�N�g�̃f�[�^
[System.Serializable]
public class StagePart
{
    [SerializeField]
    public StagePartsCategory category;
    [SerializeField]
    public Vector2 position;
    [SerializeField]
    public Vector2 sizeMagnification = Vector2.one;
    [SerializeField]
    public bool isNorth;
}

// ��̃X�e�[�W�̃f�[�^
[System.Serializable]
public class StageData
{
    [SerializeField]
    public List<StagePart> stageParts;
    [SerializeField]
    public float width;
    [SerializeField]
    public float height;
}


public class StageDataManager : EditorWindow
{
    [SerializeField]
    private StageData stageData;
    private Transform parent;

    string filePath;

    //�X�N���[���ʒu
    private Vector2 _scrollPosition = Vector2.zero;

    private Dictionary<StagePartsCategory, string> pathList;

    // �E�B���h�E
    [MenuItem("Window/Editor extention/StageDataManager", false, 1)]
    private static void ShowSetWaiPointWindow()
    {
        StageDataManager window = GetWindow<StageDataManager>();
        window.titleContent = new GUIContent("StageDataManager Window");
        window.Init();
    }

    private void Init()
    {
        pathList = new Dictionary<StagePartsCategory, string>();
        pathList.Add(StagePartsCategory.Scaffold, PrefabPath.scaffold);
        pathList.Add(StagePartsCategory.JumpRamp, PrefabPath.jumpRamp);
        pathList.Add(StagePartsCategory.Wall, PrefabPath.wall);
        pathList.Add(StagePartsCategory.NormalWall, PrefabPath.normalWall);
        pathList.Add(StagePartsCategory.ItemBox, PrefabPath.itemBox);



    }

    private void OnGUI()
    {

        parent = EditorGUILayout.ObjectField("Parent", parent, typeof(Transform), true) as Transform;
        //�`��͈͂�����Ȃ���΃X�N���[���o����悤��
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

        SerializedObject so = new SerializedObject(this);

        so.Update();

        SerializedProperty stringsProperty = so.FindProperty("stageData");

        EditorGUILayout.PropertyField(stringsProperty, true);

        so.ApplyModifiedProperties();

        

        //�X�N���[���ӏ��I��
        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("�X�e�[�W�����I", GUILayout.Height(64)))
        {
            GenerateStage();
            if (parent)
            {
                Selection.activeObject = parent;
            }
        }
        if (GUILayout.Button("�ǂݍ��݃t�@�C���I��", GUILayout.Height(64)))
        {

            filePath = EditorUtility.OpenFilePanel("select ", "Assets", "json");
            LoadFromJson();
        }
        if (GUILayout.Button("�ۑ��t�H���_�I��", GUILayout.Height(64)))
        {

            SaveToJson(EditorUtility.SaveFilePanel("select ", "Assets", "StageData.json", "json"));
        }

        if (GUILayout.Button("�V�[��������f�[�^�ɃZ�b�g", GUILayout.Height(64)))
        {

            SceneToData();
        }
    
    }




    private void SceneToData()
    {

        var allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        stageData.stageParts.Clear();

        // ���ׂẴI�u�W�F�N�g���`�F�b�N
        foreach (var obj in allObjects)
        {

            // �V�[�����̃I�u�W�F�N�g����Ȃ�������Ώۂ�����
            if (!PrefabManager.IsObjInScene(obj)) continue;

            foreach (StagePartsCategory value in Enum.GetValues(typeof(StagePartsCategory)))
            {
                
                if (PrefabManager.isEqualBasePrefab(GetPrefab(value), obj))
                {
                    if (obj.CompareTag("GoStop"))
                    {
                        break;
                    }
                    // �K�w�̂���prefab�ŏd������̂�h��
                    if (PrefabManager.isEqualBasePrefab(obj.transform.parent.gameObject, obj))
                    {
                        break;
                    }

                    StagePart part = new StagePart();
                    part.category = value;
                    part.position = obj.transform.position;
                    part.sizeMagnification = GetSizeMagnification(obj.transform.localScale ,GetPrefab(value).transform.localScale);
                    if (IsSetTag(value))
                    {
                        part.isNorth = obj.tag == Tag.PoleTag.north;
                    }
                    stageData.stageParts.Add(part);
                }
            }

        }


    }

    private void SaveToJson(string path)
    {
        if (path == string.Empty)
        {
            Debug.Log("�t�H���_���I������Ă��܂���");
            return;
        }

        string jsonstr = JsonUtility.ToJson(stageData);
        StreamWriter writer = new StreamWriter(path, false);
        writer.WriteLine(jsonstr);
        writer.Flush();
        writer.Close();
    }

    private void LoadFromJson()
    {
        if (filePath == string.Empty)
        {
            Debug.Log("�t�@�C�����I������Ă��܂���");
            return;
        }

        // �t�@�C����ǂݍ���Ńf�[�^�ɗ���
        StreamReader reader = new StreamReader(filePath);
        string datastr = reader.ReadToEnd();
        reader.Close();
        JsonUtility.FromJsonOverwrite(datastr, stageData);
    }

    private void GenerateStage()
    {
        // �Ƃ肠�����I�u�W�F�N�g�̔z�u����

        foreach (var part in stageData.stageParts)
        {

            InstantiateStagePart(part);
        }

    }

    // �p�[�c�̐���������
    private void InstantiateStagePart(StagePart partData)
    {
        GameObject baseObject = GetPrefab(partData.category);
        if (!baseObject) return;

        GameObject gameObject = PrefabUtility.InstantiatePrefab(baseObject,parent) as GameObject;
        gameObject.transform.position = partData.position;
        gameObject.transform.localScale *= partData.sizeMagnification;

        if (IsSetTag(partData.category))
        {
            if (partData.isNorth)
            {
                gameObject.tag = Tag.PoleTag.north;
            }
            else
            {
                gameObject.tag = Tag.PoleTag.south;

            }
        }


        Undo.RegisterCreatedObjectUndo(gameObject, "Create stageObject");


    }

    // �p�[�c�̃v���n�u���擾����
    private GameObject GetPrefab(StagePartsCategory category)
    {
        string path = null;
        switch (category)
        {
            case StagePartsCategory.Scaffold:
                path = PrefabPath.scaffold;
                break;
            case StagePartsCategory.JumpRamp:
                path = PrefabPath.jumpRamp;

                break;
            case StagePartsCategory.Wall:
                path = PrefabPath.wall;

                break;
            case StagePartsCategory.NormalWall:
                path = PrefabPath.normalWall;

                break;
            case StagePartsCategory.ItemBox:
                path = PrefabPath.itemBox;

                break;

        }
        return AssetDatabase.LoadAssetAtPath<GameObject>(path);
        
    }

    // ���͂̌�����ݒ肷�邩�ǂ���
    private bool IsSetTag(StagePartsCategory categry)
    {
        return categry == StagePartsCategory.JumpRamp || categry == StagePartsCategory.Wall;
    }

    // �X�P�[������{����
    private Vector2 GetSizeMagnification(Vector2 scale, Vector2 baseScale)
    {

        return new Vector2(scale.x / baseScale.x, scale.y / baseScale.y);
    }





}
#endif