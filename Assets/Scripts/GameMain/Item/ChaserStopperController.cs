using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaserStopperController : MonoBehaviour
{

    [SerializeField, Min(0.0f)]
    private float stopTime;         // 止めている時間

    private bool isChaserStopped;   // 今止めているかどうか
    private float stopTimer;        // タイマー

    private GameObject chaser;
    private Transform chaserTransform;
    private Rigidbody2D chaserRigid;


    private Vector2 stopPosition;

    // 取得用タグ名
    private readonly string chaserTagName = "Chaser";   // チェイサー

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

    // チェイサーを捕まえる
    private void StartStop()
    {
        isChaserStopped = true;
        stopTimer = stopTime;
        chaserTransform.position = stopPosition;
        chaserRigid.constraints = RigidbodyConstraints2D.FreezeAll;


    }

    // チェイサーを開放して自分を消す
    private void EndStop()
    {
        chaserRigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        Destroy(this.gameObject);
    }


}
