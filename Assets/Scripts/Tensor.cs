using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tensor 
{
    public Vector3 major;
    public Vector3 minor;

    public Tensor(Vector3 p1, Vector3 p2)
    {
        major = p1;
        minor = p2;
    }
}
