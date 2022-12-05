using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// �Q�[���̏��
public enum GameState
{
    Playing,    // �v���C��
    InEvent,    // �C�x���g���i�`���[�g���A���Ȃǁj
    Pause       // �|�[�Y��
}

// �Q�[���̏�Ԃ��Ǘ�����N���X
// �V���O���g����DontDestroyOnLoad
public class GameStateManager : MonoBehaviour
{
    public static GameStateManager instance;    // ���̃N���X�̃C���X�^���X

    public GameState gameState { get; private set; }    // ���݂̏��

    private GameState preGameState;                     // �O�̏��

    private void Awake()
    {
        // �V���O���g���ɂ���
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            instance.gameState = GameState.Playing;
            // �V�[���J�ڌ�͐�΂�Playing
            SceneManager.sceneLoaded += ToNextScene;

        }
        else
        {
            Destroy(gameObject);
        }
    }

    // �|�[�Y���ɂ���
    public void ToPause()
    {
        preGameState = instance.gameState;
        instance.gameState = GameState.Pause;
    }

    // �v���C���ɂ���
    public void ToPlaying()
    {
        if (instance.gameState == GameState.Pause)
        {
            instance.gameState = instance.preGameState;
        }
        else
        {
            instance.gameState = GameState.Playing;
        }
    }

    // �V�[���J�ڎ��ɍs������
    public void ToNextScene(Scene nextScene, LoadSceneMode mode)
    {
        // ��Ԃ��v���C���ɂ���
        instance.gameState = GameState.Playing;
        // ���Ԃ�i�߂�
        Time.timeScale = 1.0f;
    }

    // �C�x���g���ɂ���
    public void ToEvent()
    {
        instance.gameState = GameState.InEvent;
    }

    // ���͂��󂯕t�����Ԃ��ǂ������擾
    public bool IsInputtable()
    {
        // �v���C������������͂��󂯕t����
        return gameState == GameState.Playing;
    }

    

}
