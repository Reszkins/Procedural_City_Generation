using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetecter : MonoBehaviour
{
    public ContactFilter2D filter;
    private BoxCollider2D boxCollider;
    private GameObject go;
    private Collider2D[] hits = new Collider2D[10];

    public void SetParameters(BoxCollider2D collider, GameObject gameObject)
    {
        boxCollider = collider;
        go = gameObject;
    }

    public void CheckCollision()
    {
        boxCollider.OverlapCollider(filter, hits);
        for (int i = 0; i < hits.Length; ++i)
        {
            if(hits[i] == null)
            {
                continue;
            }
            if (hits[i].name == "Semi-Main Road" || hits[i].name == "Main Road")
            {
                Destroy(go);
            }
        }
    }
}
