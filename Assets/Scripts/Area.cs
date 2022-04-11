using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area
{
    public List<Line> lines = new List<Line>();
    public float area;

    public Area(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        lines.Add(new Line(p1, p2));
        lines.Add(new Line(p2, p3));
        lines.Add(new Line(p3, p4));
        lines.Add(new Line(p4, p1));
        area = Vector3.Magnitude(p2 - p1) * Vector3.Magnitude(p3 - p2);
    }
}
