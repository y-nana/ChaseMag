using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClipManager : MonoBehaviour
{
    // �擾���ɔ���������A�N�V����
    public System.Action<ClipManager> GetAction { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GetAction?.Invoke(this);
        this.gameObject.SetActive(false);
    }

}
