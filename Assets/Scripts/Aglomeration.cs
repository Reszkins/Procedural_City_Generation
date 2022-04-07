using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aglomeration : MonoBehaviour
{
    public Material roadTexture;
    private List<VoronoiElement> districts = new List<VoronoiElement>();
    public void CreateRoads()
    {
        districts = GameManager.instance.districts;

        foreach(VoronoiElement district in districts)
        {
            Vector3 maxX = new Vector3(-6, 0, 0);
            Vector3 minX = new Vector3(6, 0, 0);
            Vector3 maxY = new Vector3(0, -6, 0);
            Vector3 minY = new Vector3(0, 6, 0);

            foreach(Vector3 point in district.points)
            {
                if(point.x > maxX.x)
                {
                    maxX = point;
                }
                if (point.x < minX.x)
                {
                    minX = point;
                }
                if (point.y > maxY.y)
                {
                    maxY = point;
                }
                if (point.y < minY.y)
                {
                    minY = point;
                }
            }
            DrawRoad(maxX, minX);
            DrawRoad(minY, maxY);
        }
    }
    private void DrawRoad(Vector3 a, Vector3 b)
    {
        GameObject road = new GameObject("road");
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
        line.startWidth = 0.02f;
        line.endWidth = 0.02f;

        return go;
    }
}
