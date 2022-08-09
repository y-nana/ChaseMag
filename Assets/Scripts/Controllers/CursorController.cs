using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorController : MonoBehaviour
{

    [SerializeField] List<GameObject> buttonObjects;
    List<Transform> buttonsTransform;
    List<Button> buttons;



    int selectNum;
    bool isDown;
    Transform cursorTransform;

    // Start is called before the first frame update
    void Start()
    {
        buttonsTransform = new List<Transform>();
        buttons = new List<Button>();
        for (int i = 0; i < buttonObjects.Count; i++)
        {
            buttonsTransform.Add(buttonObjects[i].GetComponent<Transform>());
            buttons.Add(buttonObjects[i].GetComponent<Button>());
        }
        cursorTransform = GetComponent<Transform>();
        isDown = true;
        selectNum = 0;
        cursorTransform.position = new Vector2(cursorTransform.position.x, buttonsTransform[selectNum].position.y);
        buttons[selectNum].Select();
    }

    // Update is called once per frame
    void Update()
    {
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
    }


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
}
