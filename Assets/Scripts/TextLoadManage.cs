using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TextLoadManage : MonoBehaviour
{
    
    private string dataPath;
    public TutorialJsonWrapper tutorialDatas;

    private void Awake()
    {
        // Assets�t�H���_����
        dataPath = Application.dataPath + "/tutorialData.json";
    }
    // Start is called before the first frame update
    void Start()
    {

        tutorialDatas = LoadTest();

        //Debug.Log(tutorialDatas.tutorialDataList[0].viewText);
        //TutorialJsonWrapper tutorialJsonWrapper = new TutorialJsonWrapper();
        //tutorialJsonWrapper.tutorialJsonDatas = new TutorialJsonData[3];

        //SaveTest(tutorialJsonWrapper);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SaveTest(TutorialJsonWrapper data)
    {
        // json�ɕϊ�
        string jsonstr = JsonUtility.ToJson(data);
        // �ۑ�����J��
        StreamWriter writer = new StreamWriter(dataPath, false);
        // json�f�[�^��������
        writer.WriteLine(jsonstr);
        // �o�b�t�@�̃N���A�A�N���[�Y
        writer.Flush();
        writer.Close();
    }


    public TutorialJsonWrapper LoadTest()
    {
        StreamReader reader = new StreamReader(dataPath);
        string datastr = reader.ReadToEnd();
        reader.Close();
        tutorialDatas = JsonUtility.FromJson<TutorialJsonWrapper>(datastr);

        return JsonUtility.FromJson<TutorialJsonWrapper>(datastr);
    }

}
