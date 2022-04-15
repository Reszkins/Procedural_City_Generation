using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aglomeration : MonoBehaviour
{
    public Material roadTexture;
    private List<VoronoiElement> districts = new List<VoronoiElement>();
    public void CreateRoads()
    {
        
        //Debug.Log("elos");
        districts = GameManager.instance.districts;
        CalculateCorners();
        PolarAngleSort();
        foreach(VoronoiElement district in districts)
        {
            if(district.type == DistrictType.Forest)
            {
                continue;
            }
            float maxX = -6;
            float minX = 6;
            float maxY = -6;
            float minY = 6;
        /*Debug.Log("NEW DISTRICT");
        for(int i = 0; i < district.points.Count; ++i)
        {
            Debug.Log(district.points[i]);
        }*/
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
        while(current.area > maxArea / 5)
        {
            //Debug.Log("AREA: " + current.area + " max/7: " + maxArea / 5 + " maxarea: " + maxArea);
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
                            pq.Add(newArea);
                        }
                        else
                        {
                            newArea = new Area(current.lines[i].secondPoint, point, new Vector3(current.lines[i].firstPoint.x, point.y), current.lines[i].firstPoint);
                            pq.Add(newArea);
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
                            pq.Add(newArea);
                        }
                        else
                        {
                            newArea = new Area(current.lines[i].secondPoint, point, new Vector3(point.x, current.lines[i].firstPoint.y), current.lines[i].firstPoint);
                            pq.Add(newArea);
                        }
                    }
                }
            }
            current = pq.Get();
        }
        pq.Add(current);

        while (!pq.Empty())
        {
            CalculateLine(pq.Get(),district);
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
                        //Debug.Log("posz³o1");
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
                        //Debug.Log("posz³o2");
                    }
                }
            }
            else if (!first && !second)
            {
                List<Vector3> points = CrossPoints(area.lines[i].firstPoint, area.lines[i].secondPoint, district.points);
                if(points != null)
                {
                    DrawRoad(points[0], points[1]);
                    //Debug.Log("posz³o3");
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
    private void PolarAngleSort()
    {
        Vector3 centerOfPoints;
        foreach (VoronoiElement voronoi in districts)
        {
            centerOfPoints = Vector3.zero;
            for (int i = 0; i < voronoi.points.Count; ++i)
            {
                centerOfPoints.x += voronoi.points[i].x;
                centerOfPoints.y += voronoi.points[i].y;
            }
            centerOfPoints.x /= voronoi.points.Count;
            centerOfPoints.y /= voronoi.points.Count;

            List<float> alpha = new List<float>();
            for (int i = 0; i < voronoi.points.Count; ++i)
            {
                alpha.Add(0);
            }
            for (int i = 0; i < voronoi.points.Count; ++i)
            {
                if (voronoi.points[i].x >= centerOfPoints.x && voronoi.points[i].y >= centerOfPoints.y)
                {
                    alpha[i] = (voronoi.points[i].y - centerOfPoints.y) / (Mathf.Abs(voronoi.points[i].x - centerOfPoints.x) + Mathf.Abs((voronoi.points[i].y - centerOfPoints.y)));
                }
                else if (voronoi.points[i].x < centerOfPoints.x && voronoi.points[i].y >= centerOfPoints.y)
                {
                    alpha[i] = 2 - ((voronoi.points[i].y - centerOfPoints.y) / (Mathf.Abs(voronoi.points[i].x - centerOfPoints.x) + Mathf.Abs((voronoi.points[i].y - centerOfPoints.y))));
                }
                else if (voronoi.points[i].x < centerOfPoints.x && voronoi.points[i].y < centerOfPoints.y)
                {
                    alpha[i] = 2 + (Mathf.Abs((voronoi.points[i].y - centerOfPoints.y)) / (Mathf.Abs(voronoi.points[i].x - centerOfPoints.x) + Mathf.Abs((voronoi.points[i].y - centerOfPoints.y))));
                }
                else if (voronoi.points[i].x >= centerOfPoints.x && voronoi.points[i].y < centerOfPoints.y)
                {
                    alpha[i] = 4 - (Mathf.Abs((voronoi.points[i].y - centerOfPoints.y)) / (Mathf.Abs(voronoi.points[i].x - centerOfPoints.x) + Mathf.Abs((voronoi.points[i].y - centerOfPoints.y))));
                }
            }
            for (int i = 0; i < voronoi.points.Count; ++i)
            {
                for (int j = 0; j < voronoi.points.Count - 1; ++j)
                {
                    if (alpha[j] > alpha[j + 1])
                    {
                        Vector3 tmp1 = voronoi.points[j];
                        voronoi.points[j] = voronoi.points[j + 1];
                        voronoi.points[j + 1] = tmp1;
                        float tmp2 = alpha[j];
                        alpha[j] = alpha[j + 1];
                        alpha[j + 1] = tmp2;
                    }
                }
            }
        }
    }
    private void CalculateCorners()
    {
        bool topX, botX, topY, botY;
        foreach (VoronoiElement voronoi in districts)
        {
            topX = false;
            botX = false;
            topY = false;
            botY = false;
            foreach (Vector3 point in voronoi.points)
            {
                if (point.x == 5f)
                {
                    topX = true;
                }
                if (point.x == -5f)
                {
                    botX = true;
                }
                if (point.y == 5f)
                {
                    topY = true;
                }
                if (point.y == -5f)
                {
                    botY = true;
                }
            }
            if (topX && topY)
            {
                voronoi.points.Add(new Vector3(5, 5));
            }
            if (topX && botY)
            {
                voronoi.points.Add(new Vector3(5, -5));
            }
            if (botX && topY)
            {
                voronoi.points.Add(new Vector3(-5, 5));
            }
            if (botX && botY)
            {
                voronoi.points.Add(new Vector3(-5, -5));
            }
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
