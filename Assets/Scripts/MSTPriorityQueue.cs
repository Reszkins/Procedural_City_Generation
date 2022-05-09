using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MSTPriorityQueue
{
    private List<Distance> pq = new List<Distance>();

    public void Add(Distance n)
    {
        pq.Add(n);
        pq.Sort((x, y) => x.distance.CompareTo(y.distance));
    }
    public Distance Get()
    {
        Distance result = pq[0];
        pq.Remove(result);
        return result;
    }
    public bool Empty()
    {
        if (pq.Count == 0)
        {
            return true;
        }
        return false;
    }
}
