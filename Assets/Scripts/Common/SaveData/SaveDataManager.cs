using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveDataManager : MonoBehaviour
{

    public static SaveDataManager instance;    // このクラスのインスタンス

   
    private readonly string fileName = "/savedata.json";

    private string path;

    public SaveData saveData { get; private set; }

    private void Awake()
    {

        // シングルトンにする
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            // buildしたときどうなる
            path = Application.dataPath;
            ImportSaveData();

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
        reader = new StreamReader(path + fileName);
        if (reader == null)
        {
            Debug.Log("ファイルがない");

            ResetData();
            return;
        }
        datastr = reader.ReadToEnd();
        reader.Close();
        Debug.Log("よみこみました");
        saveData = JsonUtility.FromJson<SaveData>(datastr);
    }

    private void ExportSaveData()
    {
        StreamWriter writer;

        string jsonstr = JsonUtility.ToJson(saveData);
        Debug.Log(saveData.stageClearDatas[0].isClear.ToString());
        Debug.Log(jsonstr);

        writer = new StreamWriter(path + fileName, false);
        writer.Write(jsonstr);
        writer.Flush();
        writer.Close();
    }

    public void ResetData()
    {

        saveData = new SaveData();

        for (int i = 0; i < System.Enum.GetValues(typeof(StageLevelState)).Length; i++)
        {
            StageClearData clearData = new StageClearData();
            clearData.stageLevel = (StageLevelState)System.Enum.ToObject(typeof(StageLevelState), i);
            clearData.isClear = false;
            saveData.stageClearDatas.Add(clearData);
        }
        ExportSaveData();
    }

    // クリアした
    public void ClearStage(StageLevelState level)
    {
        if (saveData == null)
        {
            ResetData();
        }
        for (int i = 0; i < saveData.stageClearDatas.Count; i++)
        {
            if (saveData.stageClearDatas[i].stageLevel == level)
            {
                saveData.stageClearDatas[i].isClear = true;
                return;
            }
        }

        StageClearData data = new StageClearData();
        data.stageLevel = level;
        data.isClear = true;
        saveData.stageClearDatas.Add(data);
        ExportSaveData();

    }

    // クリアしたかどうか
    public bool IsClear(StageLevelState level)
    {
        if (saveData == null)
        {
            ResetData();
        }
        for (int i = 0; i < saveData.stageClearDatas.Count; i++)
        {
            if (saveData.stageClearDatas[i].stageLevel == level)
            {
                return saveData.stageClearDatas[i].isClear;
            }
        }

        return false;
    }
}
