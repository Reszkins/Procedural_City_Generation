using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MSTRoadGenerator : MonoBehaviour
{
    public Material roadTexture;
    public Sprite pointTexture;

    private List<VoronoiElement> districts = new List<VoronoiElement>();
    private List<VoronoiElement> residentalDistricts = new List<VoronoiElement>();
    private List<Vector3> crossings = new List<Vector3>();
    private List<List<Node>> graph = new List<List<Node>>();
    public void GenerateRoads()
    {
        districts = GameManager.instance.districts;
        CreateArrayOfResidentalDistricts();
        DrawPoints();
        CreateRoads();
    }
    private void CreateRoads()
    {
        //GameManager.instance.deluanay = true;
        GameManager.instance.deluanayTriangulation.GetPoints(crossings);
        GameManager.instance.deluanayTriangulation.Deluanay();

        CreateGraph();

        MSTPriorityQueue pq = new MSTPriorityQueue();
        for(int i = 0; i < graph.Count; ++i)
        {
            for(int j = 0; j < graph[i].Count; ++j)
            {
                pq.Add(new Distance((crossings[graph[i][j].index] - crossings[i]).magnitude, new Node(i, i), graph[i][j]));
            }
        }
        int[] treeId = new int[crossings.Count];
        for(int i = 0; i < crossings.Count; ++i)
        {
            treeId[i] = i;
        }
        bool end = false;
        while (!end)
        {
            Distance lowest = pq.Get();
            if (treeId[lowest.firstNode.index] != treeId[lowest.secondNode.index])
            {
                int newId = treeId[lowest.firstNode.index];
                int oldId = treeId[lowest.secondNode.index];

                for (int i = 0; i < crossings.Count; ++i)
                {
                    if (treeId[i] == oldId)
                    {
                        treeId[i] = newId;
                    }
                }

                DisplayRoad(crossings[lowest.firstNode.index], crossings[lowest.secondNode.index]);
            }
            end = true;
            int id = treeId[0];
            for (int i = 1; i < crossings.Count; ++i)
            {
                if (treeId[i] != id)
                {
                    end = false;
                }
            }
        }
    }

    private void CreateGraph()
    {
        for(int i = 0; i < crossings.Count; ++i)
        {
            graph.Add(new List<Node>());
        }
        foreach(Triangle triangle in GameManager.instance.triangles)
        {
            int firstIndex = -1;
            int secondIndex = -1;
            int thirdIndex = -1;
            for(int i = 0; i < crossings.Count; ++i)
            {
                if(crossings[i] == triangle.a)
                {
                    Debug.Log("SERIO?");
                    firstIndex = i;
                }
                if (crossings[i] == triangle.b)
                {
                    Debug.Log("SERIO?2");
                    secondIndex = i;
                }
                if (crossings[i] == triangle.c)
                {
                    Debug.Log("SERIO?3");
                    thirdIndex = i;
                }
            }
            Node n1 = new Node(firstIndex, firstIndex);
            Node n2 = new Node(secondIndex, secondIndex);
            Node n3 = new Node(thirdIndex, thirdIndex);
            bool sec = false;
            bool thi = false;
            for(int i = 0; i < graph[firstIndex].Count; ++i)
            {
                if(graph[firstIndex][i].Equals(n2))
                {
                    sec = true;
                }
                if (graph[firstIndex][i].Equals(n3))
                {
                    thi = true;
                }
            }
            if (!sec)
            {
                graph[firstIndex].Add(n2);
            }
            if (!thi)
            {
                graph[firstIndex].Add(n3);
            }
            sec = false;
            thi = false;
            for (int i = 0; i < graph[secondIndex].Count; ++i)
            {
                if (graph[secondIndex][i].Equals(n1))
                {
                    sec = true;
                }
                if (graph[secondIndex][i].Equals(n3))
                {
                    thi = true;
                }
            }
            if (!sec)
            {
                graph[secondIndex].Add(n1);
            }
            if (!thi)
            {
                graph[secondIndex].Add(n3);
            }
            sec = false;
            thi = false;
            for (int i = 0; i < graph[thirdIndex].Count; ++i)
            {
                if (graph[thirdIndex][i].Equals(n1))
                {
                    sec = true;
                }
                if (graph[thirdIndex][i].Equals(n2))
                {
                    thi = true;
                }
            }
            if (!sec)
            {
                graph[thirdIndex].Add(n1);
            }
            if (!thi)
            {
                graph[thirdIndex].Add(n2);
            }
        }
    }
    private void DrawPoints()
    {
        Vector3 point = Vector3.zero;
        for(int i = 0; i < GameManager.instance.crossingNumber; ++i)
        { 
            bool isInside = false;
            while(!isInside)
            {
                point = new Vector3(Random.Range(GameManager.instance.minX, GameManager.instance.maxX),
                    Random.Range(GameManager.instance.minY, GameManager.instance.maxY));
                foreach(VoronoiElement district in residentalDistricts)
                {
                    if (Sorient(point, district))
                    {
                        isInside = true;
                        break;
                    }
                }
            }
            crossings.Add(point);
            //DisplayPoint(point);
        }
    }
    private void CreateArrayOfResidentalDistricts()
    {
        foreach(VoronoiElement district in districts)
        {
            if(district.type == DistrictType.ResidentialDistrict ||
                district.type == DistrictType.OfficeDistrict ||
                district.type == DistrictType.CityCenter)
            {
                residentalDistricts.Add(district);
            }
        }
    }
    private bool Sorient(Vector3 point, VoronoiElement district)
    {
        if (Orient(point, district.points[0], district.points[1]))
        {
            for (int i = 2; i < district.points.Count; ++i)
            {
                if (!Orient(point, district.points[i - 1], district.points[i]))
                {
                    return false;
                }
            }
            if (!Orient(point, district.points[district.points.Count - 1], district.points[0]))
            {
                return false;
            }
        }
        else
        {
            for (int i = 2; i < district.points.Count; ++i)
            {
                if (Orient(point, district.points[i - 1], district.points[i]))
                {
                    return false;
                }
            }
            if (Orient(point, district.points[district.points.Count - 1], district.points[0]))
            {
                return false;
            }
        }
        return true;
    }
    private bool Orient(Vector3 point, Vector3 firstPoint, Vector3 secondPoint)
    {
        if ((point.x - firstPoint.x) * (secondPoint.y - firstPoint.y) - (point.y - firstPoint.y) * (secondPoint.x - firstPoint.x) >= 0)
        {
            return true;
        }
        return false;
    }
    private void DisplayPoint(Vector3 position)
    {
        GameObject go = new GameObject("point");
        go.transform.position = position;
        var square = go.AddComponent<SpriteRenderer>();
        square.sprite = pointTexture;
        square.transform.localScale = new Vector3(0.6f, 0.6f, 0);
        square.sortingLayerName = "Building";
    }
    private void DisplayRoad(Vector3 a, Vector3 b)
    {
        GameObject road = new GameObject("Road");
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
