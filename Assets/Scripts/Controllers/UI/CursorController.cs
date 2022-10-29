﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorController : MonoBehaviour
{

    //[SerializeField] List<GameObject> buttonObjects;
    //List<Transform> buttonsTransform;
    //List<Button> buttons;
    [SerializeField] 
    private Button firstSelect;



    //int selectNum;
    //bool isDown;
    Transform cursorTransform;

    // Start is called before the first frame update
    void Start()
    {
        /*
        buttonsTransform = new List<Transform>();
        buttons = new List<Button>();
        for (int i = 0; i < buttonObjects.Count; i++)
        {
            buttonsTransform.Add(buttonObjects[i].GetComponent<Transform>());
            buttons.Add(buttonObjects[i].GetComponent<Button>());
        }
        isDown = true;
        selectNum = 0;
        cursorTransform.position = new Vector2(cursorTransform.position.x, buttonsTransform[selectNum].position.y);
        buttons[selectNum].Select();
        */
        cursorTransform = GetComponent<Transform>();

        firstSelect.Select();
        //ChangeSelect(firstSelect.gameObject.transform);
        //Debug.Log(firstSelect.transform.position.y);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(firstSelect.transform.position.y);
        /*
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            isDown = false;
            ChangeSelect();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            isDown = true;
            ChangeSelect();
        }
        buttons[selectNum].Select();
        */
    }

    /*

    void ChangeSelect()
    {

        if (isDown)
        {
            selectNum++;
        }
        else
        {
            selectNum--;
        }
        if (selectNum < 0)
        {
            selectNum = buttonObjects.Count - 1;
        }
        else if (selectNum >= buttonObjects.Count)
        {
            selectNum = 0;
        }

        cursorTransform.position = new Vector2(cursorTransform.position.x, buttonsTransform[selectNum].position.y);
        buttons[selectNum].Select();
    }
    */

    public void ChangeSelect(Transform buttonTransform)
    {
        cursorTransform.position = new Vector2(cursorTransform.position.x, buttonTransform.position.y);
    }
}