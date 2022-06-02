using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Candidate
{
    public Vector3 point;
    public bool valid;

    public Candidate(Vector3 p)
    {
        point = p;
        valid = true;
    }
}
