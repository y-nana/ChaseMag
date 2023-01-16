using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Pole;


public enum PoleScaffoldState
{
    Neutral,
    up,
    down
}

public class PoleScaffoldController : MonoBehaviour
{

    [SerializeField,Min(0.0f)]
    private float higherMove;       // �ō�
    [SerializeField, Min(0.0f)]
    private float lowerMove;        // �Œ�
    [SerializeField, Min(0.0f)]
    private float speed;            // �����X�s�[�h
    [SerializeField, Min(0.0f)]
    private float chaserSpeed;      // �`�F�C�T�[���߂Â������̃X�s�[�h
    [SerializeField, Min(0.0f)]
    private float playerSpeed;      // �v���C���[���߂Â������̃X�s�[�h
    [SerializeField, Min(0.0f)]
    private float margin;           // �V��

    private float defaultPosY;      // �����ʒu


    private GameObject player;
    private GameObject chaser;

    // �A�N�V����������p�̃R���|�[�l���g
    private PlayerController playerCnt;
    private Transform playerTransform;
    private ChaserController chaserCnt;
    private Transform chaserTransform;
    [SerializeField]
    private Transform moveTransform;    // ����������
    private Rigidbody2D moveRigid;    // ����������

    // �ɂ̌������m�F����p
    private PoleController poleCnt;


    // ���
    private PoleScaffoldState state;

    // �A�N�V��������͈͓��ɂ��邩�ǂ���
    private bool isPlayerInArea;
    private bool isChaserInArea;

    // �擾�p�^�O��
    private readonly string playerTagName = "Player";   // �v���C��
    private readonly string chaserTagName = "Chaser";   // �`�F�C�T�[
    private readonly string southTagName = "South";   // S��
    private readonly string northTagName = "North";   // N��

    // sprite�ύX�p
    private readonly string northPath = "Sprite/Stage/wallNorth";   // N��
    private readonly string southPath = "Sprite/Stage/wallSouth";   // S��


    // Start is called before the first frame update
    void Start()
    {

        //�v���C���[�A�`�F�C�T�[�^�O�̃I�u�W�F�N�g����ł��邱�ƑO��
        player = GameObject.FindGameObjectWithTag(playerTagName);
        chaser = GameObject.FindGameObjectWithTag(chaserTagName);
        playerCnt = player.GetComponent<PlayerController>();
        playerTransform = player.GetComponent<Transform>();
        chaserCnt = chaser.GetComponent<ChaserController>();
        chaserTransform = chaser.transform;
        poleCnt = player.GetComponentInChildren<PoleController>();

        defaultPosY = moveTransform.position.y;
        moveRigid = moveTransform.gameObject.GetComponent<Rigidbody2D>();
        SpriteRenderer spriteRenderer = moveTransform.gameObject.GetComponent<SpriteRenderer>();

        // �G�f�B�^�[��ł̖��O�Ƀ^�O����ǉ�
        tag = this.gameObject.tag;
        this.gameObject.name += tag;
        // ���g�̃^�O���m�F���āA�ɂ̌������ǂ����ɂȂ�ƃW�����v�ł���̂����肷�邽�߂̕ϐ��ɑ��
        if (tag == southTagName)
        {
            // ���g��sprite��ς���
            spriteRenderer.sprite = Resources.Load<Sprite>(southPath);
        }
        else if (tag == northTagName)
        {
            spriteRenderer.sprite = Resources.Load<Sprite>(northPath);
        }

        SetState();

    }

    // Update is called once per frame
    void Update()
    {

        switch (state)
        {
            case PoleScaffoldState.Neutral:
                // ���̈ʒu�ɖ߂낤�Ƃ���
                if (moveTransform.position.y < defaultPosY - margin)
                {
                    moveRigid.velocity = Vector2.up * speed;
                    break;

                }
                if (moveTransform.position.y > defaultPosY + margin)
                {
                    moveRigid.velocity = Vector2.down * speed;

                    break;
                }
                moveRigid.velocity = Vector2.zero;

                break;
                // ��ɏオ��
            case PoleScaffoldState.up:

                if (moveTransform.position.y > defaultPosY + higherMove - margin)
                {
                    moveTransform.position = new Vector2(moveTransform.position.x, defaultPosY + higherMove);

                    moveRigid.velocity = Vector2.zero;
                    break;
                }

                moveRigid.velocity = Vector2.up * GetVelocity();

                break;
                // ���ɉ�����
            case PoleScaffoldState.down:

                if (moveTransform.position.y < defaultPosY-lowerMove + margin)
                {
                    moveTransform.position = new Vector2(moveTransform.position.x, defaultPosY - lowerMove);
                    moveRigid.velocity = Vector2.zero;
                    break;

                }
                moveRigid.velocity = Vector2.down * GetVelocity();

                break;

        }


    }
    private void OnTriggerStay2D(Collider2D collision)
    {

        // �A�N�V�����ł���ʒu�ɃI�u�W�F�N�g������
        if (collision.gameObject == player)
        {
            isPlayerInArea = true;
            SetState();

        }
        if (collision.gameObject == chaser)
        {
            isChaserInArea = true;
            SetState();
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // �A�N�V�����ł���ʒu����I�u�W�F�N�g���o��
        if (collision.gameObject == player)
        {
            isPlayerInArea = false;
            SetState();

        }
        if (collision.gameObject == chaser)
        {
            isChaserInArea = false;
            SetState();
        }
    }

    // ��Ԃ��Z�b�g
    private void SetState()
    {
        // ��Ƀ`�F�C�T�[���D�悳���
        if (isChaserInArea)
        {
            state = GetStateForChaser();
            return;
        }
        if (isPlayerInArea)
        {
            state = GetStateForPlayer();
            return;
        }

        state = PoleScaffoldState.Neutral;

    }

    private PoleScaffoldState GetStateForChaser()
    {
        // �`�F�C�T�[�Ə��̑��΍��W�����߂�
        float relPosY = chaserTransform.position.y - moveTransform.position.y;
        // �`�F�C�T�[��������ɂ���Ƃ�
        if (relPosY >= 0)
        {
            if (chaserCnt.wantToJump)
            {
                return PoleScaffoldState.up;
            }
            if (chaserCnt.wantToDown)
            {
                return PoleScaffoldState.down;
            }

        }
        else
        {
            // ���ɂ���Ƃ�
            if (chaserCnt.wantToJump)
            {
                return PoleScaffoldState.down;
            }

        }
        return PoleScaffoldState.Neutral;
    }

    // ���g�̋ɂ̌���(�^�O)�ƃv���C���̈ʒu����
    // ��Ԃ��擾
    private PoleScaffoldState GetStateForPlayer()
    {
        // �v���C���[�Ə��̑��΍��W�����߂�
        float relPosY = playerTransform.position.y - moveTransform.position.y;
        // �v���C���[��������ɂ���Ƃ�
        if (relPosY >= 0)
        {
            if (tag == southTagName)
            {
                if (poleCnt.PoleCheck((int)PoleOrientation.Up))
                {
                    return PoleScaffoldState.up;
                }
                else if (poleCnt.PoleCheck((int)PoleOrientation.Down))
                {
                    return PoleScaffoldState.down;

                }
            }
            else
            {
                if (poleCnt.PoleCheck((int)PoleOrientation.Down))
                {
                    return PoleScaffoldState.up;
                }
                else if (poleCnt.PoleCheck((int)PoleOrientation.Up))
                {
                    return PoleScaffoldState.down;

                }
            }
        }
        else
        {
            // ���ɂ���Ƃ�
            if (tag == southTagName)
            {
                if (poleCnt.PoleCheck((int)PoleOrientation.Down))
                {
                    return PoleScaffoldState.down;
                }
                else if (poleCnt.PoleCheck((int)PoleOrientation.Up))
                {
                    return PoleScaffoldState.up;

                }
            }
            else
            {
                if (poleCnt.PoleCheck((int)PoleOrientation.Up))
                {
                    return PoleScaffoldState.down;
                }
                else if (poleCnt.PoleCheck((int)PoleOrientation.Down))
                {
                    return PoleScaffoldState.up;

                }
            }
        }
        return PoleScaffoldState.Neutral;

    }

    // �߂��̏󋵂ɂ���ĕԂ��l��ς���
    private float GetVelocity()
    {
        if (isChaserInArea)
        {
            return chaserSpeed;
        }
        if (isPlayerInArea)
        {
            return speed * poleCnt.PoleStrong * poleCnt.PoleStrong;
        }

        return speed;
    }

}
