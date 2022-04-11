using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line
{
    public Vector3 firstPoint;
    public Vector3 secondPoint;
    public bool vertical;

    public Line(Vector3 f, Vector3 s)
    {
        firstPoint = f;
        secondPoint = s;
        if(firstPoint.x == secondPoint.x)
        {
            vertical = true;
        } else
        {
            vertical = false;
        }
    }
}
