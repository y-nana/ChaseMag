using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialTryManage : MonoBehaviour
{

    [SerializeField]
    private Image achievementGauge;     // 達成ゲージ

    [SerializeField]
    PlayerController player;
    [SerializeField]
    PoleController poleCnt;

    // walk用
    [SerializeField]
    private Transform reachingPoint;    // 到達地点

    // poleRotation用
    private bool[] poleChanged = new bool[4];

    

    

    // Start is called before the first frame update
    void Start()
    {
        achievementGauge.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator TryStart(TutorialState tutorialState)
    {
        switch (tutorialState)
        {
            case TutorialState.none:
                break;
            case TutorialState.walk:
                yield return StartCoroutine(WalkEvent());
                break;
            case TutorialState.jump:
                break;
            case TutorialState.wall:
                break;
            case TutorialState.poleRotation:
                yield return StartCoroutine(PoleRotationEvent());
                break;
            case TutorialState.item:
                break;
            case TutorialState.rule:
                break;
            default:
                break;
        }
    }

    // チュートリアル　歩く
    private IEnumerator WalkEvent()
    {
        achievementGauge.gameObject.SetActive(true);
        achievementGauge.fillAmount = 0.0f;
        Transform playerTransform = player.transform;
        float startPos = playerTransform.position.x;
        GameStateManager.instance.ToPlaying();

        while (true)
        {

            yield return null;
            float movingDistance = playerTransform.position.x - startPos;
            if (movingDistance < 0)
            {
                achievementGauge.fillAmount = Mathf.Abs(movingDistance / (reachingPoint.position.x - startPos));

            }

            if (reachingPoint.position.x >= player.transform.position.x)
            {
                GameStateManager.instance.ToEvent();

                break;
            }
        }
        GameStateManager.instance.ToEvent();

    }

    // チュートリアル　極の向きを変える
    private IEnumerator PoleRotationEvent()
    {
        achievementGauge.gameObject.SetActive(true);
        achievementGauge.fillAmount = 0.0f;
        poleChanged.Initialize();
        poleCnt.tutorialAction = TryChangePole;
        GameStateManager.instance.ToPlaying();

        while (true)
        {

            yield return null;
            int finished = 0;
            for (int i = 0; i < poleChanged.Length; i++)
            {
                if (poleChanged[i])
                {
                    finished++;
                }
            }
            achievementGauge.fillAmount = Mathf.Abs((float)finished / poleChanged.Length);


            if (finished>=poleChanged.Length)
            {
                GameStateManager.instance.ToEvent();

                break;
            }
        }
        GameStateManager.instance.ToEvent();
        poleCnt.tutorialAction = null;
    }

    private void TryChangePole(Pole.PoleOrientation orientation)
    {
        poleChanged[(int)orientation] = true;
    }

}
