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
    public List<VoronoiElement> neighbours;
    public bool border;

    public VoronoiElement(Vector3 c)
    {
        center = c;
        points = new List<Vector3>();
        type = DistrictType.Default;
        neighbours = new List<VoronoiElement>();
        border = false;
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

    public void AddNeighbour(VoronoiElement neighbour)
    {
        neighbours.Add(neighbour);
    }
}
