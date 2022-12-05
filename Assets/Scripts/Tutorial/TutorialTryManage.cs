using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// �`���[�g���A���̋����𐧌䂷��N���X
public class TutorialTryManage : MonoBehaviour
{

    // �N���b�v�֌W
    [SerializeField]
    private Image viewClipPanel;    // �擾�������ǂ�����\������p�l��

    [SerializeField]
    private Image viewClipPrefab;   // �\������N���b�v�v���n�u

    private List<Image> viewClipList = new List<Image>();               // �\�����Ă���N���b�v

    [SerializeField]
    private List<ClipManager> getClipList = new List<ClipManager>();    // �X�e�[�W��ɑ��݂���N���b�v�̃��X�g
    private List<bool> isGetClip = new List<bool>();                    // �N���b�v�̊l����

    
    [SerializeField]
    PlayerController player;    // �v���C���[
    [SerializeField]
    PoleController poleCnt;     // �v���C���[�̎���
    [SerializeField]
    ChaserController chaser;    // �S

    [SerializeField]
    SpriteMask chaserMask;      // �Ó]�p�}�X�N
    [SerializeField]
    GameObject spotSprite;      // �S�ɃX�|�b�g�𓖂Ă邽�߂̃}�X�N�p�X�v���C�g

    [SerializeField]
    GameObject triggerCollider; // �A�C�e���̐����ɓ���g���K�[

    [SerializeField]
    SceneDirector sceneDirector;    // �V�[���J�ڗp

    [SerializeField]
    Transform chaserPoint;      // �S���Z�b�g����ʒu

    [SerializeField]
    CountDownManager countDown; // �ǂ���������O�̃J�E���g�_�E���p

    [SerializeField]
    GameObject cage;            // �S������߂�P�[�W

    [SerializeField]
    private float cagePositionY;// �P�[�W��y���W

    // �����I�u�W�F�N�g�ɃA�^�b�`����Ă��邱�Ƃ��O��
    private TutrialManager manager; // �`���[�g���A���̐i�s�Ǘ�


    // Start is called before the first frame update
    void Start()
    {
        // �N���b�v�̐��ɉ����ĕ\���Ɗl���󋵂̏�����
        for (int i = 0; i < getClipList.Count; i++)
        {
            // �擾���̏�����o�^
            getClipList[i].GetAction = GetClip;
            // �e�L�X�g�{�b�N�X�̃p�l����ɕ\������i���l�����̓V���G�b�g�Ŋl�����ɐF��t���ĕ\���j
            Image viewClip = Instantiate(viewClipPrefab, viewClipPanel.transform);
            viewClipList.Add(viewClip);
            // �l���󋵂�������
            isGetClip.Add(false);
        }
        // �i�s�ɉ����čs�������̓o�^
        manager = GetComponent<TutrialManager>();
        manager.setChaser = SetChaserPos;
        manager.startChaser = StartChase;
        manager.tutorialFinish = TutorialFinish;
        // �V�[���J�ڎ��ɍs�������̓o�^
        sceneDirector.tutorialClearAct = TutorialClear;
        sceneDirector.tutorialOverAct = TutorialOver;

        // �܂��g��Ȃ��I�u�W�F�N�g�̖�����
        spotSprite.SetActive(false);
        countDown.gameObject.SetActive(false);


    }


    // �N���b�v�擾���ɌĂяo�����֐�
    public void GetClip(ClipManager clip)
    {
        for (int i = 0; i < getClipList.Count; i++)
        {
            if (clip == getClipList[i])
            {
                isGetClip[i] = true;
                viewClipList[i].color = Color.white;
                
                Invoke("ClipCompleteCheck", 0.2f);
            }
        }


    }

    // �N���b�v�����ׂĊl���������ǂ������`�F�b�N����
    private void ClipCompleteCheck()
    {
        int getClipCount = 0;
        for (int j = 0; j < isGetClip.Count; j++)
        {
            if (isGetClip[j])
            {
                getClipCount++;
                manager.waitTrigger = getClipCount >= isGetClip.Count;
            }
        }

    }

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == player.gameObject)
        {
            // ���̏�Ԃɐi�߂�t���O�𗧂āA�R���C�_�[�𖳌���
            manager.waitTrigger = true;
            triggerCollider.SetActive(false);

        }
    }

    // �S���Z�b�e�B���O
    public void SetChaserPos()
    {
        chaser.transform.position = chaserPoint.position;
        chaser.SearchRoute();
        chaser.GetComponent<Rigidbody2D>().constraints =
            RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        spotSprite.SetActive(true);
    }

    // �P�[�W�𓮂����S���X�^�[�g������
    public void StartChase()
    {
        chaser.GetComponent<Rigidbody2D>().constraints =
            RigidbodyConstraints2D.FreezeRotation;
        countDown.gameObject.SetActive(true) ;
        cage.transform.position = new Vector2( chaser.transform.position.x,chaser.transform.position.y-cagePositionY);
        spotSprite.SetActive(false);
    }

    // �`���[�g���A�����N���A�����ۂ̏���
    public void TutorialClear()
    {
        chaser.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        manager.waitTrigger = true;
        manager.clearFlag = true;
    }

    // �`���[�g���A���ŕ߂܂������̏���
    public void TutorialOver()
    {
        chaser.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        manager.waitTrigger = true;
        manager.clearFlag = false;
    }

    // �`���[�g���A���̏I��
    public void TutorialFinish()
    {
        // �X�e�[�W�Z���N�g�V�[���֑J��
        sceneDirector.ToStageSelect();
    }


}
