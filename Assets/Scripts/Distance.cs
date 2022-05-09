using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Distance 
{
    public float distance;
    public Node firstNode;
    public Node secondNode;
    public Distance(float d, Node f, Node s)
    {
        distance = d;
        firstNode = f;
        secondNode = s;
    }
}
