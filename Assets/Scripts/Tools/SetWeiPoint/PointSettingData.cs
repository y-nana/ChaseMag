using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PointSettingData
{
    [field: SerializeField]
    public GameObject obj { get; set; }

    [field: SerializeField]
    public List<PointPosition> pointPosition { get; set; }

}