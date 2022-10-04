using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    
    playing,
    inEvent,
    pause

}


public class GameStateManager : MonoBehaviour
{
    public static GameStateManager instance;

    public GameState gameState { get; private set; }

    private GameState preGameState;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            instance.gameState = GameState.playing;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ToPause()
    {
        preGameState = instance.gameState;
        instance.gameState = GameState.pause;
    }

    public void ToPlaying()
    {
        if (instance.gameState == GameState.pause)
        {
            instance.gameState = instance.preGameState;
        }
        else
        {
            instance.gameState = GameState.playing;
        }
    }

    public void ToEvent()
    {
        instance.gameState = GameState.inEvent;
    }


    public bool IsInputtable()
    {
        switch (gameState)
        {
            case GameState.playing:
                return true;
            default:
                return false;
        }
    }

    

}
