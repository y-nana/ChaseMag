using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{

    public bool selected { get; set; }
    public float value { get; set; }

    public Action<float> changeAct { get; set; }

    [SerializeField, Range(0f, 1.0f)]
    private float moveValue;
    [SerializeField]
    private RectTransform handle;

    
    private float minPosition;
    private float maxPosition;

    [SerializeField]
    private float maxValue = 1.0f;
    [SerializeField]
    private float minValue = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        RectTransform slider = this.GetComponent<RectTransform>();

        minPosition = -handle.sizeDelta.x / 2.0f;
        maxPosition = slider.sizeDelta.x - handle.sizeDelta.x / 2.0f;
        MoveHandle();
    }

    // Update is called once per frame
    void Update()
    {
        if (selected)
        {
            // 右
            if (Input.GetKey(KeyCode.D) || Input.GetAxis("Horizontal") > 1.0f)
            {
                AddValue(moveValue * Time.unscaledDeltaTime);

            }
            // 左
            if (Input.GetKey(KeyCode.A) || Input.GetAxis("Horizontal") < -1.0f)
            {
                AddValue(-moveValue * Time.unscaledDeltaTime);

            }

            MoveHandle();

        }



    }

    private void MoveHandle()
    {
        float deltaPos = maxPosition - minPosition;
        float ratio = (value-minValue) / (maxValue - minValue);
        deltaPos *= ratio;
        // ハンドルを動かす
        handle.anchoredPosition = new Vector2(minPosition + deltaPos, handle.localPosition.y);
    }


    // 値の加算、範囲チェック
    private void AddValue(float addValue)
    {
        float temp = addValue * (maxValue - minValue);
        this.value += temp;


        if (this.value > maxValue)
        {
            this.value = maxValue;
        }
        if (this.value < minValue)
        {
            this.value = minValue;
        }


        if (changeAct != null)
        {
            changeAct.Invoke(value);
        }

    }

}
