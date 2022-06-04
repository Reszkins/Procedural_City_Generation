using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using System.Linq;

public class PriorityQueue<T>
{
    private List<T> pq = new List<T>();

    public void Add(T obj, Comparer<T> c)
    {
        if (pq.Contains(obj))
        {
            return;
        }
        pq.Add(obj);
        pq.Sort(c);
    }
    public T Get()
    {
        T obj = pq[0];
        pq.Remove(obj);
        return obj;
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
