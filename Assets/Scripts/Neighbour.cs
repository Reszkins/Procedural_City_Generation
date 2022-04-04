using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neighbour
{
    public Triangle triangle;
    public Edge edge;

    public Neighbour(Triangle triangle, Edge edge)
    {
        this.triangle = triangle;
        this.edge = edge;
    }
}
