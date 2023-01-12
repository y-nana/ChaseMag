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

    [SerializeField]
    private Transform maxTransform; // �ō�
    [SerializeField]
    private Transform minTransform; // �Œ�
    [SerializeField, Min(0.0f)]
    private float speed;            // �����X�s�[�h
    [SerializeField, Min(0.0f)]
    private float margin;           // �V��

    private float defaultPosY;     // �����ʒu


    private GameObject player;
    private GameObject chaser;

    // �A�N�V����������p�̃R���|�[�l���g
    private PlayerController playerCnt;
    private Transform playerTransform;
    private ChaserController chaserCnt;
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
        //chaser = GameObject.FindGameObjectWithTag(chaserTagName);
        playerCnt = player.GetComponent<PlayerController>();
        playerTransform = player.GetComponent<Transform>();
        //chaserCnt = chaser.GetComponent<ChaserController>();
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
        Debug.Log(state);
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

            case PoleScaffoldState.up:

                if (moveTransform.position.y > maxTransform.position.y - margin)
                {
                    moveTransform.position = new Vector2(moveTransform.position.x, maxTransform.position.y);

                    moveRigid.velocity = Vector2.zero;
                    break;
                }
                moveRigid.velocity = Vector2.up * speed;

                break;
            case PoleScaffoldState.down:

                if (moveTransform.position.y < minTransform.position.y + margin)
                {
                    moveTransform.position = new Vector2(moveTransform.position.x, minTransform.position.y);
                    moveRigid.velocity = Vector2.zero;
                    break;

                }
                moveRigid.velocity = Vector2.down * speed;

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
        }
    }

    // ��Ԃ��Z�b�g
    private void SetState()
    {
        if (isPlayerInArea)
        {
            state = GetScaffoldState();
            return;
        }

        state = PoleScaffoldState.Neutral;

    }

    // ���g�̋ɂ̌���(�^�O)�ƃv���C���̈ʒu����
    // ��Ԃ��擾
    private PoleScaffoldState GetScaffoldState()
    {
        // �v���C���[�Ə��̑��΍��W�����߂�
        float relPosY = playerTransform.position.y - moveTransform.position.y;
        // �v���C���[��������ɂ���Ƃ�
        if (relPosY <= 0)
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
        }
        return PoleScaffoldState.Neutral;

    }

}
