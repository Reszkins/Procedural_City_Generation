using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainRoadGenerator : MonoBehaviour
{
    public Material roadTexture;

    private List<VoronoiElement> districts = new List<VoronoiElement>();
    private List<VoronoiElement> borderDistricts = new List<VoronoiElement>();
    private List<int> borderPointsIndexes = new List<int>();

    public void GenerateMainRoads()
    {
        districts = GameManager.instance.districts;
        if (!GameManager.instance.MST)
        {
            ChooseBorderDistricts();
            CreateMainRoads();
        }
    }

    private void ChooseBorderDistricts()
    {
        foreach(VoronoiElement district in districts)
        {
            for(int i = 0; i < district.points.Count; ++i)
            {
                if(district.points[i].x == GameManager.instance.maxX || district.points[i].x == GameManager.instance.minX ||
                    district.points[i].y == GameManager.instance.maxY || district.points[i].y == GameManager.instance.minY)
                {
                    borderPointsIndexes.Add(i);
                    borderDistricts.Add(district);
                }
            }
        }
    }
    private void CreateMainRoads()
    { 
        int index1 = Random.Range(0, borderPointsIndexes.Count - 1);
        int index2 = index1;
        while(index2 == index1)
        {
            index2 = Random.Range(0, borderPointsIndexes.Count - 1);
        }
        int index3 = index2;
        while (index3 == index2)
        {
           index3 = Random.Range(0, borderPointsIndexes.Count - 1); 
        }

        int[] indexes = new int[3];
        indexes[0] = index1;
        indexes[1] = index2;
        indexes[2] = index3;
        for (int m = 0; m < 3; ++m)
        {
        VoronoiElement district = borderDistricts[indexes[m]];
        bool p = false;
        int pointIndex = borderPointsIndexes[indexes[m]];
        float firstDistance, secondDistance, distance;
        int oldIndex = 0;
        bool stop = false;
        while (!stop)
        {
            oldIndex = pointIndex;
            if(pointIndex == 0)
            {
                firstDistance = (district.points[district.points.Count - 1] - GameManager.instance.cityCenter).magnitude;
            }
            else
            {
                firstDistance = (district.points[pointIndex - 1] - GameManager.instance.cityCenter).magnitude;
            }
            secondDistance = (district.points[(pointIndex + 1) % district.points.Count] - GameManager.instance.cityCenter).magnitude;
            distance = (district.points[pointIndex] - GameManager.instance.cityCenter).magnitude;

            if (firstDistance < secondDistance)
            {
                if(firstDistance < distance)
                {
                    pointIndex = pointIndex == 0 ? district.points.Count - 1 : pointIndex - 1;
                }
            }
            else
            {
                if(secondDistance < distance)
                {
                    pointIndex = (pointIndex + 1) % district.points.Count;
                }
            }
            if(pointIndex != oldIndex)
            {
                DrawRoad(district.points[oldIndex], district.points[pointIndex]);
            }
            int newIndex = pointIndex;
            VoronoiElement newDistrict = district;
            float mini = (district.center - GameManager.instance.cityCenter).magnitude;
            for(int i = 0; i < district.neighbours.Count; ++i)
            {
                for(int j = 0; j < district.neighbours[i].points.Count; ++j)
                {
                    if(district.neighbours[i].points[j] == district.points[pointIndex])
                    {
                        if((district.neighbours[i].center - GameManager.instance.cityCenter).magnitude < mini)
                        {
                            if(district.neighbours[i].center == GameManager.instance.cityCenter)
                            {
                                stop = true;
                            }
                            newIndex = j;
                            newDistrict = district.neighbours[i];
                            mini = (district.neighbours[i].center - GameManager.instance.cityCenter).magnitude;
                        }
                    }
                }
            }
            district = newDistrict;
            pointIndex = newIndex;
        }
        }
        
    }

    private void DrawRoad(Vector3 a, Vector3 b)
    {
        GameObject road = new GameObject("Main Road");
        road = RoadSetup(road, a, b);
    }
    private GameObject RoadSetup(GameObject go, Vector3 a, Vector3 b)
    {
        var line = go.AddComponent<LineRenderer>();
        line.SetPosition(0, a);
        line.SetPosition(1, b);

        line.sortingOrder = 1;
        line.material = roadTexture;
        line.textureMode = LineTextureMode.Tile;
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;
        line.sortingLayerName = "Main Road";

        return go;
    }
}
