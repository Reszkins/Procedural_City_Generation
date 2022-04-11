using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue
{
    private List<Area> pq = new List<Area>();
    
    public void Add(Area a)
    {
        pq.Add(a);
        pq.Sort((x, y) => y.area.CompareTo(x.area));
        //Debug.Log("POSORTOWANE: ");
        //for(int i = 0; i < pq.Count; ++i)
        //{
        //    Debug.Log(pq[i].area);
        //}
        //Debug.Log("KONIEC ");
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
    }
}
