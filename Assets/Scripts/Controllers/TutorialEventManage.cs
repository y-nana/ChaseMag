using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialEventManage : MonoBehaviour
{

    [SerializeField]
    private Image achievementGauge;     // �B���Q�[�W

    [SerializeField]
    PlayerController player;

    // walk�p
    [SerializeField]
    private Transform reachingPoint;    // ���B�n�_

    

    

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
                yield return StartCoroutine("WalkEvent");
                break;
            case TutorialState.jump:
                break;
            case TutorialState.wall:
                break;
            case TutorialState.poleRotation:
                break;
            case TutorialState.item:
                break;
            case TutorialState.rule:
                break;
            default:
                break;
        }
    }

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
    }


}
