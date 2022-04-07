using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum DistrictType
    {
        CityCenter,
        OfficeDistrict,
        ResidentialDistrict,
        Forest,
        Default
    }
public class VoronoiElement
{
    public List<Vector3> points;
    public Vector3 center;
    public DistrictType type;

    public VoronoiElement(Vector3 c)
    {
        center = c;
        points = new List<Vector3>();
        type = DistrictType.Default;
    }
    public void AddPoint(Vector3 point)
    {
        bool flag = false;
        for(int i = 0; i < points.Count; ++i)
        {
            if(points[i] == point)
            {
                flag = true;
            }
        }
        if (!flag)
        {
            points.Add(point);
        }
    }
}
