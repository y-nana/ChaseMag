using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// �N���b�v�擾�����Ǘ�����N���X
public class ClipManager : MonoBehaviour
{

    public static int count { get; set; }
    private readonly string clipTag = "Clip";
    private GameObject[] clips;

    [SerializeField]
    private Text countText;

    [SerializeField]
    private float repopTime = 20.0f;

    [SerializeField]
    private float allRepopTime = 1.0f;


    // Start is called before the first frame update
    void Start()
    {
        count = 0;
        clips = GameObject.FindGameObjectsWithTag(clipTag);
        foreach (var clip in clips)
        {
            clip.SetActive(true);
            // �����̓o�^
            clip.GetComponent<ClipController>().GetAction = GetClip;
        }
    }

    // �N���b�v���擾����
    public void GetClip(ClipController clip)
    {
        count++;
        // �擾���\��
        if (countText != null)
        {
            countText.text = count.ToString();
        }
        StartCoroutine(Repop(clip.gameObject));
        // �c��`�F�b�N
        if (CheckClip())
        {
            StartCoroutine(ResetClip());
        }
    }

    // ��̃N���b�v�̃��|�b�v
    IEnumerator Repop(GameObject obj)
    {
        yield return new WaitForSeconds(repopTime);
        obj.SetActive(true);
    }

    // ���ׂĎ���Ă���true
    public bool CheckClip()
    {
        foreach (var clip in clips)
        {
            if (clip.activeSelf)
            {
                return false;
            }
        }
        return true;

    }

    //���ׂĎ������ݒ肵�Ȃ���
    IEnumerator ResetClip()
    {
        yield return new WaitForSeconds(allRepopTime);
        foreach (var clip in clips)
        {
            clip.SetActive(true);
        }
    }

}
