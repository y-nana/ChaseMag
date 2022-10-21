using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CursorManager : MonoBehaviour
{

    [SerializeField]
    private EventSystem eventSystem;

    [SerializeField]
    private Color outlineColor;
    [SerializeField]
    private float speed = 2.0f;

    private GameObject preSelectedObj;

    private Outline outline;

    private bool isPulus;

    // Start is called before the first frame update
    void Start()
    {

        
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            var selected = eventSystem.currentSelectedGameObject.gameObject;
            if (preSelectedObj != selected)
            {
                if (preSelectedObj != null)
                {
                    Debug.Log(selected);
                    Destroy(outline);
                    outline = selected.AddComponent<Outline>();
                    OutLineInit();
                }
                else
                {
                    

                    outline = selected.AddComponent<Outline>();
                    OutLineInit();
                }
                preSelectedObj = selected;
            }
        }
        catch (NullReferenceException ex)
        {

            if (preSelectedObj!=null)
            {
                eventSystem.SetSelectedGameObject(preSelectedObj);

            }
        }

        if (outline != null)
        {
            float value = speed * Time.deltaTime;
            if (!isPulus)
            {
                value *= -1;
            }
            Color temp = outline.effectColor;
            temp.a += value;
            outline.effectColor =temp;
            if (outline.effectColor.a <= 0)
            {
                isPulus = true;
            }
            else if(outline.effectColor.a >=1)
            {
                isPulus = false;
            }

        }
        if (GameStateManager.instance.gameState == GameState.pause)
        {
            outline.effectColor = outlineColor;

        }

    }


    private void OutLineInit()
    {
        outline.effectDistance = new Vector2(-20, -20);
        outline.effectColor = outlineColor;
    }


    
}
