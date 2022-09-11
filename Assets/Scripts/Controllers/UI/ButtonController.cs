using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    [SerializeField]
    private Button firstSelectButton;

    // Start is called before the first frame update
    void Start()
    {
        if (firstSelectButton != null)
        {
            firstSelectButton.Select();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
