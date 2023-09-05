using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveDataManager : MonoBehaviour
{

    public static SaveDataManager instance;    // ���̃N���X�̃C���X�^���X

   
    private readonly string fileName = "/savedata.json";

    private string path;

    private SaveData saveData;

    private void Awake()
    {

        // �V���O���g���ɂ���
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            ResetData();


            //path = Application.dataPath;

            //ImportSaveData();

        }
        else
        {
            Destroy(gameObject);
        }
    }


    private void ImportSaveData()
    {
        string datastr = "";
        StreamReader reader;
        if (!File.Exists(path + fileName))
        {
            Debug.Log("�t�@�C�����Ȃ�");

            ResetData();
            return;
        }
        reader = new StreamReader(path + fileName);
        datastr = reader.ReadToEnd();
        reader.Close();
        Debug.Log("��݂��݂܂���");
        saveData = JsonUtility.FromJson<SaveData>(datastr);
    }

    private void ExportSaveData()
    {
        StreamWriter writer;

        string jsonstr = JsonUtility.ToJson(saveData);
        //Debug.Log(jsonstr);
        writer = new StreamWriter(path + fileName, false);
        writer.Write(jsonstr);
        writer.Flush();
        writer.Close();
    }

    public void ResetData()
    {

        saveData = new SaveData();

        //ExportSaveData();
    }

    // �N���A����
    public void ClearStage(StageLevelState level)
    {
        if (saveData == null)
        {
            ResetData();
        }
        saveData.ClearStage(level);
        //ExportSaveData();

    }

    public void SetClearData(StageLevelState level, int score)
    {
        if (saveData == null)
        {
            ResetData();
        }
        saveData.SetClearData(level, score);
        //ExportSaveData();

    }


    // �N���A�������ǂ���
    public bool IsClear(StageLevelState level)
    {
        if (saveData == null)
        {
            ResetData();
            return false;
        }

        return saveData.IsClear(level) ;
    }

    public int GetMaxScore(StageLevelState level)
    {

        return saveData.GetMaxScore(level);
    }
}
