using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// チュートリアルの挙動を制御するクラス
public class TutorialTryManage : MonoBehaviour
{

    // クリップ関係
    [SerializeField]
    private Image viewClipPanel;    // 取得したかどうかを表示するパネル

    [SerializeField]
    private Image viewClipPrefab;   // 表示するクリッププレハブ

    private List<Image> viewClipList = new List<Image>();               // 表示しているクリップ

    [SerializeField]
    private List<ClipManager> getClipList = new List<ClipManager>();    // ステージ上に存在するクリップのリスト
    private List<bool> isGetClip = new List<bool>();                    // クリップの獲得状況

    
    [SerializeField]
    PlayerController player;    // プレイヤー
    [SerializeField]
    PoleController poleCnt;     // プレイヤーの磁石
    [SerializeField]
    ChaserController chaser;    // 鬼

    [SerializeField]
    SpriteMask chaserMask;      // 暗転用マスク
    [SerializeField]
    GameObject spotSprite;      // 鬼にスポットを当てるためのマスク用スプライト

    [SerializeField]
    GameObject triggerCollider; // アイテムの説明に入るトリガー

    [SerializeField]
    SceneDirector sceneDirector;    // シーン遷移用

    [SerializeField]
    Transform chaserPoint;      // 鬼をセットする位置

    [SerializeField]
    CountDownManager countDown; // 追いかけられる前のカウントダウン用

    [SerializeField]
    GameObject cage;            // 鬼を閉じ込めるケージ

    [SerializeField]
    private float cagePositionY;// ケージのy座標

    // 同じオブジェクトにアタッチされていることが前提
    private TutrialManager manager; // チュートリアルの進行管理


    // Start is called before the first frame update
    void Start()
    {
        // クリップの数に応じて表示と獲得状況の初期化
        for (int i = 0; i < getClipList.Count; i++)
        {
            // 取得時の処理を登録
            getClipList[i].GetAction = GetClip;
            // テキストボックスのパネル上に表示する（未獲得時はシルエットで獲得時に色を付けて表示）
            Image viewClip = Instantiate(viewClipPrefab, viewClipPanel.transform);
            viewClipList.Add(viewClip);
            // 獲得状況を初期化
            isGetClip.Add(false);
        }
        // 進行に応じて行う処理の登録
        manager = GetComponent<TutrialManager>();
        manager.setChaser = SetChaserPos;
        manager.startChaser = StartChase;
        manager.tutorialFinish = TutorialFinish;
        // シーン遷移時に行う処理の登録
        sceneDirector.tutorialClearAct = TutorialClear;
        sceneDirector.tutorialOverAct = TutorialOver;

        // まだ使わないオブジェクトの無効化
        spotSprite.SetActive(false);
        countDown.gameObject.SetActive(false);


    }


    // クリップ取得時に呼び出される関数
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

    // クリップをすべて獲得したかどうかをチェックする
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
            // つぎの状態に進めるフラグを立て、コライダーを無効化
            manager.waitTrigger = true;
            triggerCollider.SetActive(false);

        }
    }

    // 鬼をセッティング
    public void SetChaserPos()
    {
        chaser.transform.position = chaserPoint.position;
        chaser.SearchRoute();
        chaser.GetComponent<Rigidbody2D>().constraints =
            RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        spotSprite.SetActive(true);
    }

    // ケージを動かし鬼をスタートさせる
    public void StartChase()
    {
        chaser.GetComponent<Rigidbody2D>().constraints =
            RigidbodyConstraints2D.FreezeRotation;
        countDown.gameObject.SetActive(true) ;
        cage.transform.position = new Vector2( chaser.transform.position.x,chaser.transform.position.y-cagePositionY);
        spotSprite.SetActive(false);
    }

    // チュートリアルをクリアした際の処理
    public void TutorialClear()
    {
        chaser.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        manager.waitTrigger = true;
        manager.clearFlag = true;
    }

    // チュートリアルで捕まった時の処理
    public void TutorialOver()
    {
        chaser.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        manager.waitTrigger = true;
        manager.clearFlag = false;
    }

    // チュートリアルの終了
    public void TutorialFinish()
    {
        // ステージセレクトシーンへ遷移
        sceneDirector.ToStageSelect();
    }


}
