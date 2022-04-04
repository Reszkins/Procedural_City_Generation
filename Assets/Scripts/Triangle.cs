using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangle
{
    public Vector3 a;
    public Vector3 b;
    public Vector3 c;
    public Vector3 center;
    public float radius;
    public List<Edge> edges = new List<Edge>();
    public Edge ab;
    public Edge ac;
    public Edge bc;
    public List<Neighbour> neighbours = new List<Neighbour>();

    public Triangle(Vector3 a, Vector3 b, Vector3 c)
    {
        this.a = a;
        this.b = b;
        this.c = c;
        ab = new Edge(a, b);
        ac = new Edge(a, c);
        bc = new Edge(b, c);
        edges.Add(ab);
        edges.Add(ac);
        edges.Add(bc);
    }

}
