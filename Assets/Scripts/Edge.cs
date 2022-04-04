using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge
{
    public Vector3 first;
    public Vector3 second;

    public Edge(Vector3 f, Vector3 s)
    {
        first = f;
        second = s;
    }
}
