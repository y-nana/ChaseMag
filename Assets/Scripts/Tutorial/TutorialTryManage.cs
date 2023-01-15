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
    private List<ClipController> getClipList = new List<ClipController>();    // ステージ上に存在するクリップのリスト
    private List<bool> isGetClip = new List<bool>();                    // クリップの獲得状況

    // チュートリアルに使うオブジェクト
    [SerializeField]
    PlayerController player;    // プレイヤー
    [SerializeField]
    PoleController poleCnt;     // プレイヤーの磁石
    [SerializeField]
    ChaserController chaser;    // 鬼

    Transform chaserTransform;  // 鬼のトランスフォーム
    Rigidbody2D chaserRigid2d;  // 鬼のrigidbody2d

    [SerializeField]
    GameObject triggerCollider; // アイテムの説明に入るトリガー

    [SerializeField]
    SceneDirector sceneDirector;    // シーン遷移用

    [SerializeField]
    SpriteMask chaserMask;      // 暗転用マスク
    [SerializeField]
    GameObject spotSprite;      // 鬼にスポットを当てるためのマスク用スプライト
    [SerializeField]
    Transform chaserPoint;      // 鬼をセットする位置
    [SerializeField]
    CountDownManager countDown; // 追いかけられる前のカウントダウン用
    [SerializeField]
    Transform cage;            // 鬼を閉じ込めるケージ
    [SerializeField]
    private float cagePositionY;// ケージのy座標

    // 同じオブジェクトにアタッチされていることが前提
    private TutrialManager manager; // チュートリアルの進行管理


    // Start is called before the first frame update
    void Start()
    {
        // コンポーネント取得
        chaserTransform = chaser.transform;
        chaserRigid2d = chaser.GetComponent<Rigidbody2D>();


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
    public void GetClip(ClipController clip)
    {
        // クリップリストから獲得したクリップを探す
        for (int i = 0; i < getClipList.Count; i++)
        {
            if (clip == getClipList[i])
            {
                // 獲得状況を変更
                isGetClip[i] = true;
                // クリップに色を付ける
                viewClipList[i].color = Color.white;
                // 少し時間を空けて次へ進むかどうかチェックする
                Invoke("ClipCompleteCheck", 0.2f);
            }
        }


    }

    // クリップをすべて獲得したかどうかをチェックする
    private void ClipCompleteCheck()
    {
        int getClipCount = 0;
        // 獲得状況を一つずつ確認する
        for (int j = 0; j < isGetClip.Count; j++)
        {
            if (isGetClip[j])
            {
                getClipCount++;
                // つぎの状態に進めるフラグを立てる
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
        // 位置の調整
        chaserTransform.position = chaserPoint.position;
        chaser.SearchRoute();
        // スポットを当てるためにy座標を停止させる
        chaserRigid2d.constraints =
            RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        // スポットオブジェクトを有効化
        spotSprite.SetActive(true);
    }

    // ケージを動かし鬼をスタートさせる
    public void StartChase()
    {
        // 座標の停止の解除
        chaserRigid2d.constraints =
            RigidbodyConstraints2D.FreezeRotation;
        // カウントダウンの開始
        countDown.gameObject.SetActive(true) ;
        // ケージに閉じ込める
        cage.position = new Vector2(chaserTransform.position.x, chaserTransform.position.y-cagePositionY);
        spotSprite.SetActive(false);
    }

    // チュートリアルをクリアした際の処理
    public void TutorialClear()
    {
        // 鬼を停止させる
        chaserRigid2d.constraints = RigidbodyConstraints2D.FreezeAll;
        // つぎの状態に進めるフラグを立てる
        manager.waitTrigger = true;
        // クリアしたフラグのセット
        manager.clearFlag = true;
    }

    // チュートリアルで捕まった時の処理
    public void TutorialOver()
    {
        // 鬼を停止させる
        chaserRigid2d.constraints = RigidbodyConstraints2D.FreezeAll;
        // つぎの状態に進めるフラグを立てる
        manager.waitTrigger = true;
        // クリアしたフラグのセット
        manager.clearFlag = false;
    }

    // チュートリアルの終了
    public void TutorialFinish()
    {
        // ステージセレクトシーンへ遷移
        sceneDirector.ToStageSelect();
    }


}
