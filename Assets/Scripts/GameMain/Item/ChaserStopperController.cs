using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaserStopperController : MonoBehaviour
{

    [SerializeField, Min(0.0f)]
    private float stopTime;         // �~�߂Ă��鎞��

    private bool isChaserStopped;   // ���~�߂Ă��邩�ǂ���
    private float stopTimer;        // �^�C�}�[

    private GameObject chaser;
    private Transform chaserTransform;
    private Rigidbody2D chaserRigid;


    private Vector2 stopPosition;

    // �擾�p�^�O��
    private readonly string chaserTagName = "Chaser";   // �`�F�C�T�[

    // Start is called before the first frame update
    void Start()
    {
        chaser = GameObject.FindGameObjectWithTag(chaserTagName);
        chaserTransform = chaser.GetComponent<Transform>();
        chaserRigid = chaser.GetComponent<Rigidbody2D>();
        stopPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (isChaserStopped)
        {
            stopTimer -= Time.deltaTime;
            if (stopTimer < 0.0f)
            {
                EndStop();
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == chaser&&!isChaserStopped)
        {
            StartStop();
        }
    }

    // �`�F�C�T�[��߂܂���
    private void StartStop()
    {
        isChaserStopped = true;
        stopTimer = stopTime;
        chaserTransform.position = stopPosition;
        chaserRigid.constraints = RigidbodyConstraints2D.FreezeAll;


    }

    // �`�F�C�T�[���J�����Ď���������
    private void EndStop()
    {
        chaserRigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        Destroy(this.gameObject);
    }


}
