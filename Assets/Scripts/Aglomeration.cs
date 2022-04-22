using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aglomeration : MonoBehaviour
{
    public Material roadTexture;
    private List<VoronoiElement> districts = new List<VoronoiElement>();
    private List<Area> areas = new List<Area>();
    private List<VoronoiElement> areaDistrict = new List<VoronoiElement>();
    private Sprite[] buildingTextures16x16 = new Sprite[8];
    public void CreateRoads()
    {
        districts = GameManager.instance.districts;
        foreach(VoronoiElement district in districts)
        {
            if(district.type == DistrictType.Forest)
            {
                continue;
            }
            float maxX = float.MinValue;
            float minX = float.MaxValue;
            float maxY = float.MinValue;
            float minY = float.MaxValue;
            foreach(Vector3 point in district.points)
            {
                if(point.x > maxX)
                {
                    maxX = point.x;
                }
                if (point.x < minX)
                {
                    minX = point.x;
                }
                if (point.y > maxY)
                {
                    maxY = point.y;
                }
                if (point.y < minY)
                {
                    minY = point.y;
                }
            }
            CalculateAreas(maxX,minX,maxY,minY,district);
        }
    }
    private void CalculateAreas(float maxX, float minX, float maxY, float minY, VoronoiElement district)
    {
        //Debug.Log("siemaneczko");
        PriorityQueue pq = new PriorityQueue();
        pq.Add(new Area(new Vector3(maxX, maxY), new Vector3(maxX, minY), new Vector3(minX, minY), new Vector3(minX, maxY)));
        Area current = pq.Get();
        float maxArea = current.area;
        Line line;
        Vector3 point;
        Area newArea;
        float distance;
        bool vertical = false;
        bool flag = false;
        while(current.area > maxArea / 5f)
        {
            //Debug.Log("AREA: " + current.area + " max/7: " + maxArea / 5 + " maxarea: " + maxArea);
            flag = false;
            line = current.lines[Random.Range(0, 3)];
            while(line.vertical == vertical)
            {
                line = current.lines[Random.Range(0, 3)];
            }
            if (vertical)
            {
                vertical = false;
            }
            else
            {
                vertical = true;
            }
            if (line.vertical)
            {
                distance = line.secondPoint.y - line.firstPoint.y;
                point = new Vector3(line.firstPoint.x, Random.Range(line.firstPoint.y + 0.2f * distance, line.secondPoint.y - 0.2f * distance));
                for(int i = 0; i < current.lines.Count; ++i)
                {
                    if (!current.lines[i].vertical)
                    {
                        if(current.lines[i].firstPoint.x == point.x)
                        {
                            newArea = new Area(current.lines[i].firstPoint, point, new Vector3(current.lines[i].secondPoint.x,point.y), current.lines[i].secondPoint);
                            foreach(Line l in newArea.lines)
                            {
                                if (Vector3.Magnitude(l.firstPoint - l.secondPoint) < 0.165f)
                                {
                                    pq.Add(current);
                                    flag = true;
                                    break;
                                }
                            }
                            if (!flag)
                            {
                                pq.Add(newArea);
                            }       
                        }
                        else
                        {
                            newArea = new Area(current.lines[i].secondPoint, point, new Vector3(current.lines[i].firstPoint.x, point.y), current.lines[i].firstPoint);
                            foreach (Line l in newArea.lines)
                            {
                                if (Vector3.Magnitude(l.firstPoint - l.secondPoint) < 0.165f)
                                {
                                    pq.Add(current);
                                    flag = true;
                                    break;
                                }
                            }
                            if (!flag)
                            {
                                pq.Add(newArea);
                            }
                        }
                    }
                }
            }
            else
            {
                distance = line.secondPoint.x - line.firstPoint.x;
                point = new Vector3(Random.Range(line.firstPoint.x + 0.2f * distance, line.secondPoint.x - 0.2f * distance),line.firstPoint.y);
                for (int i = 0; i < current.lines.Count; ++i)
                {
                    if (current.lines[i].vertical)
                    {
                        if (current.lines[i].firstPoint.y == point.y)
                        {
                            newArea = new Area(current.lines[i].firstPoint, point, new Vector3(point.x, current.lines[i].secondPoint.y), current.lines[i].secondPoint);
                            foreach (Line l in newArea.lines)
                            {
                                if (Vector3.Magnitude(l.firstPoint - l.secondPoint) < 0.165f)
                                {
                                    pq.Add(current);
                                    flag = true;
                                    break;
                                }
                            }
                            if (!flag)
                            {
                                pq.Add(newArea);
                            }
                        }
                        else
                        {
                            newArea = new Area(current.lines[i].secondPoint, point, new Vector3(point.x, current.lines[i].firstPoint.y), current.lines[i].firstPoint);
                            foreach (Line l in newArea.lines)
                            {
                                if (Vector3.Magnitude(l.firstPoint - l.secondPoint) < 0.165f)
                                {
                                    pq.Add(current);
                                    flag = true;
                                    break;
                                }
                            }
                            if (!flag)
                            {
                                pq.Add(newArea);
                            }
                        }
                    }
                }
            }
            current = pq.Get();
        }
        pq.Add(current);

        while (!pq.Empty())
        {
            Area topArea = pq.Get();
            areas.Add(topArea);
            areaDistrict.Add(district);
            CalculateLine(topArea,district);
        }
    }
    private void CalculateLine(Area area,VoronoiElement district)
    {
        //Debug.Log(":)");
        bool first, second;
        Vector3 cp;
       // Debug.Log("NEW AREA  center: " + district.center);
        for(int i = 0; i < area.lines.Count; ++i)
        {
            //DrawRoad(area.lines[i].firstPoint, area.lines[i].secondPoint);
            //Debug.Log(area.lines[i].firstPoint + " " + area.lines[i].secondPoint);
            first = Sorient(area.lines[i].firstPoint,district);
            second = Sorient(area.lines[i].secondPoint,district);
            //Debug.Log(first + " " + second);
            if(first && second)
            {
                DrawRoad(area.lines[i].firstPoint, area.lines[i].secondPoint);
            }
            else if(first && !second)
            {
                for(int j = 1; j < district.points.Count+1; ++j)
                {
                    cp = CrossPoint(area.lines[i].firstPoint, area.lines[i].secondPoint, district.points[j - 1], district.points[j % district.points.Count]);
                    if(cp.x != 1000 && cp.y != 1000)
                    {
                        DrawRoad(area.lines[i].firstPoint, cp);
                        //Debug.Log("posz�o1");
                    } 
                }
            }
            else if (!first && second)
            {
                for (int j = 1; j < district.points.Count+1; ++j)
                {
                    cp = CrossPoint(area.lines[i].firstPoint, area.lines[i].secondPoint, district.points[j - 1], district.points[j % district.points.Count]);
                    if (cp.x != 1000 && cp.y != 1000)
                    {
                        DrawRoad(area.lines[i].secondPoint, cp);
                        //Debug.Log("posz�o2");
                    }
                }
            }
            else if (!first && !second)
            {
                List<Vector3> points = CrossPoints(area.lines[i].firstPoint, area.lines[i].secondPoint, district.points);
                if(points != null)
                {
                    DrawRoad(points[0], points[1]);
                    //Debug.Log("posz�o3");
                }
            }
        }
    }
    private List<Vector3> CrossPoints(Vector3 p, Vector3 p2, List<Vector3> points)
    {
        List<Vector3> result = new List<Vector3>();
        for(int i = 1; i < points.Count + 1; ++i)
        {
            Vector3 q = points[i - 1];
            Vector3 q2 = points[i % points.Count];
            Vector3 r = p2 - p;
            Vector3 s = q2 - q;
            Vector3 o = q - p;
            float t = (o.x * s.y - s.x * o.y) / (r.x * s.y - s.x * r.y);
            float u = (o.x * r.y - r.x * o.y) / (r.x * s.y - s.x * r.y);
            if ((r.x * s.y - s.x * r.y) != 0 && 0 <= t && t <= 1 && 0 <= u && u <= 1)
            {
                result.Add(p + t * r);
            }
        }
        //Debug.Log(result.Count);
        if(result.Count != 2)
        {
            return null;
        }
        return result;
    }
    private Vector3 CrossPoint(Vector3 p, Vector3 p2, Vector3 q, Vector3 q2)
    {
        Vector3 r = p2 - p;
        Vector3 s = q2 - q;
        Vector3 o = q - p;
        float t = (o.x * s.y - s.x * o.y) / (r.x * s.y - s.x * r.y);
        float u = (o.x * r.y - r.x * o.y) / (r.x * s.y - s.x * r.y);
        if ((r.x * s.y - s.x * r.y) != 0 && 0 <= t && t <= 1 && 0 <= u && u <= 1)
        {
            return p + t * r;
        }
        else return new Vector3(1000,1000);
    }
    private bool Sorient(Vector3 point, VoronoiElement district)
    {
        
        // Debug.Log("POINT: " + point + " DISTRICT CENTER: " + district.center);
        if(Orient(point, district.points[0], district.points[1]))
        {
            for(int i = 2; i < district.points.Count; ++i)
            {
                if(!Orient(point, district.points[i - 1], district.points[i]))
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
        /*Debug.Log((firstPoint.x * secondPoint.y) + (secondPoint.x * point.y) + (point.x * firstPoint.y) - (point.x * secondPoint.y) -
            (firstPoint.x * point.y) - (secondPoint.x * firstPoint.y));
        if((firstPoint.x * secondPoint.y) + (secondPoint.x * point.y) + (point.x * firstPoint.y) - (point.x * secondPoint.y) - 
            (firstPoint.x * point.y) - (secondPoint.x * firstPoint.y) >= 0)
        {
            return true;
        }
        return false;*/
        //Debug.Log((point.x - firstPoint.x) * (secondPoint.y - firstPoint.y) - (point.y - firstPoint.y) * (secondPoint.x - firstPoint.x));
        //Debug.Log("1: " + firstPoint + " 2: " + secondPoint + " P: " + point + " wynik: " + ((point.x - firstPoint.x) * (secondPoint.y - firstPoint.y) - (point.y - firstPoint.y) * (secondPoint.x - firstPoint.x)));
        if ((point.x - firstPoint.x) * (secondPoint.y - firstPoint.y) - (point.y - firstPoint.y) * (secondPoint.x - firstPoint.x)>= 0)
        {
            return true;
        }
        return false;
    }
    private void DrawRoad(Vector3 a, Vector3 b)
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
        line.startWidth = 0.02f;
        line.endWidth = 0.02f;
        line.sortingLayerName = "Road";

        return go;
    }
    public void CreateBuildings()
    {
        buildingTextures16x16 = GameManager.instance.buildingTextures16x16;
        Debug.Log("ILO�� AREA: " + areas.Count);
        for(int i = 0; i < areas.Count; ++i)
        {
            CalculateBuildings(areas[i],areaDistrict[i]);
        }
    }
    private void CalculateBuildings(Area area, VoronoiElement district)
    {
        float xMax = float.MinValue;
        float xMin = float.MaxValue;
        float yMax = float.MinValue;
        float yMin = float.MaxValue;

        foreach(Line l in area.lines)
        {
            if(l.firstPoint.x > xMax)
            {
                xMax = l.firstPoint.x;
            }
            if (l.firstPoint.x < xMin)
            {
                xMin = l.firstPoint.x;
            }
            if (l.firstPoint.y > yMax)
            {
                yMax = l.firstPoint.y;
            }
            if (l.firstPoint.y < yMin)
            {
                yMin = l.firstPoint.y;
            }
        }
        int q = (int)((yMax - yMin) / 0.172f);
        float diff = xMax - xMin;
        float qX = (int)((xMax - xMin) / 0.172f);
        if (q == 0) return;
        float m = (yMax - yMin) / q;
        float currentY = yMax - m/2;
        if (diff < 0.172) return;
        float mX = (xMax - xMin) / qX;
        float currentX = xMin + mX/2;
        for (int i = 0; i < q; ++i)
        {
            for(int j = 0; j < qX; ++j)
            {
                if(Sorient(new Vector3(currentX, currentY), district))
                {
                    DrawBuilding(new Vector3(currentX, currentY));
                }
                currentX = currentX + mX;
            }
            currentY = currentY - m;
            currentX = xMin + mX/2;
        }
    }
    private void DrawBuilding(Vector3 position)
    {
        GameObject building = new GameObject("Building");
        building = BuildingSetup(building,position,buildingTextures16x16[Random.Range(0,buildingTextures16x16.Length-1)]);
    }
    private GameObject BuildingSetup(GameObject building,Vector3 position, Sprite texture)
    {
        building.transform.position = position;
        var sprite = building.AddComponent<SpriteRenderer>();
        sprite.sprite = texture;
        sprite.sortingLayerName = "Building";
        var coll = building.AddComponent<BoxCollider2D>();
        var collDetecter = building.AddComponent<CollisionDetecter>();
        collDetecter.SetParameters(coll, building);
        collDetecter.CheckCollision();
        return building;
    }
}
