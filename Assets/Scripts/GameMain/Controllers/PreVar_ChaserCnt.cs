using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// �S�̃X�N���v�g �O�̃o�[�W����
public class PreVar_ChaserCnt : MonoBehaviour
{

    // �A�N�V�����̃p�����[�^
    [SerializeField] private float jumpForce = 800.0f;  // �W�����v��
    [SerializeField] private float walkForce = 8.0f;    // �����X�s�[�h
    [SerializeField] private float upForce = 10.0f;     // �ǂ����X�s�[�h


    // �R���g���[���p�R���|�[�l���g
    private Rigidbody2D rigid2D;            // �ړ�
    private SpriteRenderer spriteRenderer;  // �g�̂̌����ύX
    private Animator animator;              // �A�j���[�V����
    [SerializeField] private GameObject sceneDirector;        // �V�[���J�ڗp
    private AudioSource audioSource;        // �T�E���h


    // �A�j���[�V������bool��
    private readonly string walk = "NowWalk";  // �����Ă��邩
    private readonly string jump = "NowJump";  // �W�����v����

    // �΃v���C���p
    [SerializeField] private GameObject player;      // �v���C���I�u�W�F�N�g
    [SerializeField] private GameObject playerPole;  // �����蔻��擾�p

    [SerializeField] private float getPosTime = 0.5f;   // �v���C���̈ʒu���擾����Ԋu
    [SerializeField] private float waitTime = 5.0f;     // �v���C������~���Ă���̂�҂���
    [SerializeField] private float stopDistance = 0.1f;// �v���C�������܂��Ă���Ɣ��肷�鋗��
    [SerializeField] private float throughTime = 0.5f;  // ���b����𓧂����Ԃɂ��邩
    [SerializeField]
    private Vector2 margin =
        new Vector2(0.5f, 2.0f);                // �k���h�~�̗]��

    private float getPosTimer;      // �v���C���̈ʒu���擾����Ԋu�̃^�C�}�[
    private Vector2 distance;       // �v���C���Ƃ̋���
    private Vector2 playerPrePos;   // �v���C���̑O�t���[���̈ʒu

    private bool playerIsRight;     // �v���C�����E�ɂ��邩(������������ǂ��Ă����Ԃ�)
    private bool isThrough;         // ����𓧂���

    private float waitTimer;        // �v���C������~���Ă����
    private float throughTimer;     // ���ꂪ�������Ԃ̊�

    // �Ǘp
    private Vector2 pPos;
    private Vector2 prePos;
    private Vector2 MyPos;

    // ���ʉ�
    [SerializeField]
    private AudioClip jumpSE;



    // ���C���̖��O�ƑΉ�����int
    private enum LayerName
    {
        Default = 0,    // �f�t�H���g���C��
        Through = 8,    // ���ȊO���蔲���郌�C��
    }

    void Start()
    {
        // �R���|�[�l���g�擾
        rigid2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        this.audioSource = GetComponent<AudioSource>();



        //�l�̏�����
        playerPrePos = new Vector2(0, 0);
        prePos = new Vector2(0, 0);
        waitTimer = 0.0f;
        throughTimer = 0.0f;
        getPosTimer = 0.0f;

    }


    void Update()
    {

        // �v���C���Ƃ̋����擾
        if (getPosTimer > getPosTime)
        {
            getPosTimer = 0.0f;
            distance = GetToPlayer();
        }
        getPosTimer += Time.deltaTime;


        float playerDis = Vector2.Distance(playerPrePos, pPos);
        // �v���C�������u�ɒ�~���Ă��Ȃ���
        if (playerDis < stopDistance)
        {
            waitTimer += Time.deltaTime;
        }
        else playerPrePos = pPos;

        // ���E�ړ�
        int key = 0;
        // ������ǂ��Ă�����
        if (playerIsRight)
        {
            // �v���C����]�����ʂ肷����悤�ɂ��鏈��
            if (distance.x > -margin.x)
            {
                key = 1;
                spriteRenderer.flipX = true;
            }
            else GoToPlayer();

        }
        // �E����ǂ��Ă���
        else
        {
            if (distance.x < margin.x)
            {
                key = -1;
                spriteRenderer.flipX = false;
            }
            else GoToPlayer();
        }

        // ��莞�Ԉȏ�v���C���������Ȃ��Ȃ�����
        if (waitTimer > waitTime)
        {
            waitTimer = 0f;
            // �^���őҋ@���ꂽ���̑Ή�
            Through(distance);
            // ��őҋ@���ꂽ���̑Ή� ��
        }

        // ���ꓧ�ߏ��
        if (isThrough)
        {
            // ���E�ړ��͂��Ȃ�
            key = 0;
            throughTimer -= Time.deltaTime;
            if (throughTimer < 0)
            {
                isThrough = false;
                this.gameObject.layer = (int)LayerName.Default;
            }
        }

        Vector2 vel = this.rigid2D.velocity;
        this.rigid2D.velocity = new Vector2(key * this.walkForce, vel.y);

        // �A�j���[�V����
        Animation();

    }


    // �v���C���Ƃ̑��Έʒu�����߂�
    private Vector2 GetToPlayer()
    {
        MyPos = transform.position;
        pPos = player.transform.position;
        Vector2 distance = pPos - MyPos;

        Turn();

        return distance;
    }

    //�v���C�����ǂ��������ɂ��邩
    private void GoToPlayer()
    {
        playerIsRight = distance.x > 0;
    }

    // ���ꓧ��
    private void Through(Vector2 distance)
    {

        // �v���C�������ɂ�����
        if (distance.y < 0 && distance.x < stopDistance && distance.x > -stopDistance)
        {

            isThrough = true;
            throughTimer = throughTime;
            // ���C���ύX�ɂ�葫��𓧉�
            this.gameObject.layer = (int)LayerName.Through;
        }

    }

    // �W�����v����
    public void Jump()
    {
        if (this.rigid2D.velocity.y == 0 && distance.y > margin.y)
        {
            this.rigid2D.velocity = new Vector2(this.rigid2D.velocity.x, 0f);
            this.rigid2D.AddForce(transform.up * this.jumpForce);
            audioSource.PlayOneShot(jumpSE);

        }
    }

    // ��Ɉړ�����(�Ǘp)
    public void MoveUp()
    {
 
        this.rigid2D.velocity = Vector2.up * this.upForce;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �v���C���[�ɐڐG������Q�[���I�[�o�[�֑J��
        if (collision.gameObject == player || collision.gameObject == playerPole)
        {
            ExecuteEvents.Execute<SceneCaller>(
                target: sceneDirector,
                eventData: null,
                functor: (reciever, eventData) => reciever.ToGameOver()
                );
        }

    }

    // �A�j���[�V�����p
    private void Animation()
    {
        this.animator.SetBool(walk, false);
        this.animator.SetBool(jump, false);

        // ���ړ��݂̂��Ă��������
        if (rigid2D.velocity.y == 0 && rigid2D.velocity.x != 0) this.animator.SetBool(walk, true);

        // �㉺�ړ����Ă�����W�����v
        if (rigid2D.velocity.y != 0) this.animator.SetBool(jump, true);

    }

    private void Turn()
    {

        if (prePos.x == MyPos.x)
        {
            playerIsRight = !playerIsRight;
        }

        prePos = MyPos;

    }

}
