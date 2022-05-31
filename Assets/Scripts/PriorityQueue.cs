using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class PriorityQueue<T>
{
    /*private List<Area> pq = new List<Area>();
    
    public void Add(Area a)
    {
        pq.Add(a);
        pq.Sort((x, y) => y.area.CompareTo(x.area));
    }
    public Area Get()
    {
        Area result = pq[0];
        pq.Remove(result);
        return result;
    }
    public bool Empty()
    {
        if(pq.Count == 0)
        {
            return true;
        }
        return false;
    }*/
    private List<T> pq = new List<T>();

    public void Add(T obj, Comparer<T> c)
    {
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
