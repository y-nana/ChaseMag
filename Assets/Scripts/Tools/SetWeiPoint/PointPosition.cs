using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum BasePoint
{
    Center,
    TopRight,
    TopLeft,
    BottomRight,
    BottomLeft,
    Top
}


[System.Serializable]
public class PointPosition
{
    [field: SerializeField]
    public Vector2 position { get; set; }
    [field: SerializeField]
    public BasePoint basePoint { get; set; }
    [field: SerializeField]
    public PointCategory pointCategory { get; set; }


    public PointPosition()
    {
        this.position = Vector2.zero;
        this.basePoint = BasePoint.Center;
        this.pointCategory = PointCategory.Normal;
    }


    public PointPosition(Vector2 pos, BasePoint basePoint, PointCategory category)
    {
        this.position = pos;
        this.basePoint = basePoint;
        this.pointCategory = category;
    }


}