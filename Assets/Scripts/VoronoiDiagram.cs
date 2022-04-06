using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoronoiDiagram : MonoBehaviour
{
    private Material roadTexture;
    private bool draw = false;

    private List<VoronoiElement> voronoi;
    public Sprite pointTexture;

    public void Setup()
    {
        this.roadTexture = GameManager.instance.roadTexture;
        voronoi = new List<VoronoiElement>();
    }
    public void CalculateNewPoints()
    {
        List<Vector3> points = new List<Vector3>();
        Vector3 newPoint;
        Vector3 centerOfPoints; 
        CalculateCorners();
        foreach (VoronoiElement voronoi in voronoi)
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
                    alpha[i] = (voronoi.points[i].y-centerOfPoints.y) / (Mathf.Abs(voronoi.points[i].x - centerOfPoints.x) + Mathf.Abs((voronoi.points[i].y - centerOfPoints.y)));
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
            float a = 0;
            float cx = 0;
            float cy = 0;
            for (int i = 0; i < voronoi.points.Count - 1; ++i)
            {
                a += (voronoi.points[i].x * voronoi.points[i + 1].y) - (voronoi.points[i + 1].x * voronoi.points[i].y);
            }
            if(voronoi.points.Count - 1 > 0) a += (voronoi.points[voronoi.points.Count - 1].x * voronoi.points[0].y) - (voronoi.points[0].x * voronoi.points[voronoi.points.Count - 1].y);
            a /= 2;
            for (int i = 0; i < voronoi.points.Count - 1; ++i)
            {
                cx += (voronoi.points[i].x + voronoi.points[i + 1].x) * ((voronoi.points[i].x * voronoi.points[i + 1].y) - (voronoi.points[i + 1].x * voronoi.points[i].y));
            }
            if (voronoi.points.Count - 1 > 0) cx += (voronoi.points[voronoi.points.Count - 1].x + voronoi.points[0].x) * ((voronoi.points[voronoi.points.Count - 1].x * voronoi.points[0].y)
                - (voronoi.points[0].x * voronoi.points[voronoi.points.Count - 1].y));
            for (int i = 0; i < voronoi.points.Count - 1; ++i)
            {
                cy += (voronoi.points[i].y + voronoi.points[i + 1].y) * ((voronoi.points[i].x * voronoi.points[i + 1].y) - (voronoi.points[i + 1].x * voronoi.points[i].y));
            }
            if (voronoi.points.Count - 1 > 0) cy += (voronoi.points[voronoi.points.Count - 1].y + voronoi.points[0].y) * ((voronoi.points[voronoi.points.Count - 1].x * voronoi.points[0].y)
                - (voronoi.points[0].x * voronoi.points[voronoi.points.Count - 1].y));
            cx /= 6 * a;
            cy /= 6 * a;
            newPoint = new Vector3(cx,cy);
            points.Add(newPoint);
            GameManager.instance.points = points;
        }
    }
    public void ChooseCityCenterAndCalculateNewPoints()
    {
        float mini = 50f;
        int index = 0;
        for (int i = 0; i < GameManager.instance.pointsNumber; ++i)
        {
            if(Mathf.Abs(GameManager.instance.points[i].x)+ Mathf.Abs(GameManager.instance.points[i].y) < mini)
            {
                index = i;
                mini = Mathf.Abs(GameManager.instance.points[i].x) + Mathf.Abs(GameManager.instance.points[i].y);
            }
        }
        //Vector3 cityCenter = GameManager.instance.points[Random.Range(0, GameManager.instance.pointsNumber - 1)];
        Vector3 cityCenter = GameManager.instance.points[index];
        Debug.Log(cityCenter);
        Vector3 vector;
        for(int i=0;i< GameManager.instance.pointsNumber; ++i)
        {
            if(GameManager.instance.points[i] != cityCenter)
            {
                vector = cityCenter - GameManager.instance.points[i];
                vector = vector.normalized;
                /*if (vector.magnitude > 6)
                {
                    vector *= 0.05f;
                }
                else if (vector.magnitude > 5)
                {
                    vector *= 0.1f;
                }
                else if (vector.magnitude > 4)
                {
                    vector *= 0.2f;
                }
                else if (vector.magnitude > 3)
                {
                    vector *= 0.3f;
                }
                else if (vector.magnitude > 2)
                {
                    vector *= 0.4f;
                }
                else if (vector.magnitude > 1)
                {
                    vector *= 0.5f;
                }
                else if (vector.magnitude < 1)
                {
                    vector *= 0.6f;
                }*/
                vector *= 0.4f;
                GameManager.instance.points[i] = GameManager.instance.points[i] + vector;
            }
        }
    }
    private void DrawPoint(Vector3 position)
    {
        GameObject go = new GameObject("point");
        go.transform.position = position;
        var square = go.AddComponent<SpriteRenderer>();
        square.sprite = pointTexture;
        square.transform.localScale = new Vector3(0.6f, 0.6f, 0);
    }
    public void ConstructAndDisplay(List<Triangle> triangulation, List<Vector3> points)
    {
        draw = true;
        Construct(triangulation, points);
        for(int i = 0; i < points.Count; ++i)
        {
            DrawPoint(points[i]);
        }
    }
    public void Construct(List<Triangle> triangulation, List<Vector3> points)
    {
        for (int i = 0; i < points.Count; ++i)
        {
            voronoi.Add(new VoronoiElement(points[i]));
        }

        for(int i = 0; i < triangulation.Count-1; ++i)
        {
            for(int j = i+1; j < triangulation.Count; ++j)
            {
                foreach(Edge edge in triangulation[i].edges)
                {
                    if(edge.first == triangulation[j].ab.first && edge.second == triangulation[j].ab.second ||
                        edge.first == triangulation[j].ab.second && edge.second == triangulation[j].ab.first)
                    {
                        AddNeighbour(triangulation[i], triangulation[j], edge);
                    }
                    if (edge.first == triangulation[j].ac.first && edge.second == triangulation[j].ac.second ||
                        edge.first == triangulation[j].ac.second && edge.second == triangulation[j].ac.first)
                    {
                        AddNeighbour(triangulation[i], triangulation[j], edge);
                    }
                    if (edge.first == triangulation[j].bc.first && edge.second == triangulation[j].bc.second ||
                        edge.first == triangulation[j].bc.second && edge.second == triangulation[j].bc.first)
                    {
                        AddNeighbour(triangulation[i], triangulation[j], edge);
                    }
                }
            }
        }

        DisplayVoronoi(triangulation);

        foreach(Triangle triangle in triangulation)
        {
            if(triangle.neighbours.Count < 3)
            {
                CalculateThirdLine(triangle);
            }
        }
    }
    private void CalculateCorners()
    {
        bool topX, botX, topY, botY;
        foreach(VoronoiElement voronoi in voronoi)
        {
            topX = false;
            botX = false;
            topY = false;
            botY = false;
            foreach(Vector3 point in voronoi.points)
            {
                if(point.x == 5f)
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
            if(topX && topY)
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
    private void CalculateThirdLine(Triangle triangle)
    {
        if (Mathf.Abs(triangle.center.x) > 5 || Mathf.Abs(triangle.center.y) > 5)
        {
            return;
        }
        bool[] sharedEdges = {false,false,false};
        for(int i = 0; i < triangle.neighbours.Count; ++i)
        {
            if(triangle.ab.first == triangle.neighbours[i].edge.first && triangle.ab.second == triangle.neighbours[i].edge.second ||
                triangle.ab.second == triangle.neighbours[i].edge.first && triangle.ab.first == triangle.neighbours[i].edge.second)
            {
                sharedEdges[0] = true;
            }
            if (triangle.ac.first == triangle.neighbours[i].edge.first && triangle.ac.second == triangle.neighbours[i].edge.second ||
                triangle.ac.second == triangle.neighbours[i].edge.first && triangle.ac.first == triangle.neighbours[i].edge.second)
            {
                sharedEdges[1] = true;
            }
            if (triangle.bc.first == triangle.neighbours[i].edge.first && triangle.bc.second == triangle.neighbours[i].edge.second ||
                triangle.bc.second == triangle.neighbours[i].edge.first && triangle.bc.first == triangle.neighbours[i].edge.second)
            {
                sharedEdges[2] = true;
            }
        }
        Vector3 vector = Vector3.zero;
        Vector3 perpendicularVector = Vector3.zero;
        if (!sharedEdges[0])
        {
            vector = triangle.ab.first - triangle.ab.second;
            perpendicularVector.x = -vector.y;
            perpendicularVector.y = vector.x;

            while(Vector3.Magnitude(perpendicularVector) < 10.0f)
            {
                perpendicularVector *= 2.0f;
            }
            if (Vector3.Magnitude(triangle.center + perpendicularVector - triangle.c) < Vector3.Magnitude(triangle.center + perpendicularVector - triangle.a) &&
                Vector3.Magnitude(triangle.center + perpendicularVector - triangle.c) < Vector3.Magnitude(triangle.center + perpendicularVector - triangle.b))
            {
                perpendicularVector *= -1.0f;
            }
            float distance;
            Vector3 tmp;
            if (triangle.center.x + perpendicularVector.x > 5)
            {
                distance = 5 - triangle.center.x;
                tmp = new Vector3(distance, perpendicularVector.y * (distance / perpendicularVector.x));
                perpendicularVector = tmp;
            }
            if (triangle.center.y + perpendicularVector.y > 5)
            {
                distance = 5 - triangle.center.y;
                tmp = new Vector3(perpendicularVector.x * (distance / perpendicularVector.y),distance);
                perpendicularVector = tmp;
            }
            if (triangle.center.x + perpendicularVector.x < -5)
            {
                distance = - 5 - triangle.center.x;
                tmp = new Vector3(distance, perpendicularVector.y * (distance / perpendicularVector.x));
                perpendicularVector = tmp;
            } 
            if (triangle.center.y + perpendicularVector.y < -5)
            {
                distance = - 5 - triangle.center.y;
                tmp = new Vector3(perpendicularVector.x * (distance / perpendicularVector.y), distance);
                perpendicularVector = tmp;
            }
            DrawRoad(triangle.center, triangle.center + perpendicularVector, triangle.ab);

        }
        if (!sharedEdges[1])
        {
            vector = triangle.ac.first - triangle.ac.second;
            perpendicularVector.x = -vector.y;
            perpendicularVector.y = vector.x;

            while (Vector3.Magnitude(perpendicularVector) < 10.0f)
            {
                perpendicularVector *= 2.0f;
            }
            if (Vector3.Magnitude(triangle.center + perpendicularVector - triangle.b) < Vector3.Magnitude(triangle.center + perpendicularVector - triangle.a) &&
                Vector3.Magnitude(triangle.center + perpendicularVector - triangle.b) < Vector3.Magnitude(triangle.center + perpendicularVector - triangle.c))
            {
                perpendicularVector *= -1.0f;
            }
            float distance;
            Vector3 tmp;
            if (triangle.center.x + perpendicularVector.x > 5)
            {
                distance = 5 - triangle.center.x;
                tmp = new Vector3(distance, perpendicularVector.y * (distance / perpendicularVector.x));
                perpendicularVector = tmp;
            }
            if (triangle.center.y + perpendicularVector.y > 5)
            {
                distance = 5 - triangle.center.y;
                tmp = new Vector3(perpendicularVector.x * (distance / perpendicularVector.y), distance);
                perpendicularVector = tmp;
            }
            if (triangle.center.x + perpendicularVector.x < -5)
            {
                distance = -5 - triangle.center.x;
                tmp = new Vector3(distance, perpendicularVector.y * (distance / perpendicularVector.x));
                perpendicularVector = tmp;
            }
            if (triangle.center.y + perpendicularVector.y < -5)
            {
                distance = -5 - triangle.center.y;
                tmp = new Vector3(perpendicularVector.x * (distance / perpendicularVector.y), distance);
                perpendicularVector = tmp;
            }
            DrawRoad(triangle.center, triangle.center + perpendicularVector, triangle.ac);
        }
        if (!sharedEdges[2])
        {
            vector = triangle.bc.first - triangle.bc.second;
            perpendicularVector.x = -vector.y;
            perpendicularVector.y = vector.x;

            while (Vector3.Magnitude(perpendicularVector) < 10.0f)
            {
                perpendicularVector *= 2.0f;
            }
            if (Vector3.Magnitude(triangle.center + perpendicularVector - triangle.a) < Vector3.Magnitude(triangle.center + perpendicularVector - triangle.b) &&
                Vector3.Magnitude(triangle.center + perpendicularVector - triangle.a) < Vector3.Magnitude(triangle.center + perpendicularVector - triangle.c))
            {
                perpendicularVector *= -1.0f;
            }
            float distance;
            Vector3 tmp;
            if (triangle.center.x + perpendicularVector.x > 5)
            {
                distance = 5 - triangle.center.x;
                tmp = new Vector3(distance, perpendicularVector.y * (distance / perpendicularVector.x));
                perpendicularVector = tmp;
            }
            if (triangle.center.y + perpendicularVector.y > 5)
            {
                distance = 5 - triangle.center.y;
                tmp = new Vector3(perpendicularVector.x * (distance / perpendicularVector.y), distance);
                perpendicularVector = tmp;
            }
            if (triangle.center.x + perpendicularVector.x < -5)
            {
                distance = -5 - triangle.center.x;
                tmp = new Vector3(distance, perpendicularVector.y * (distance / perpendicularVector.x));
                perpendicularVector = tmp;
            }
            if (triangle.center.y + perpendicularVector.y < -5)
            {
                distance = -5 - triangle.center.y;
                tmp = new Vector3(perpendicularVector.x * (distance / perpendicularVector.y), distance);
                perpendicularVector = tmp;
            }
            DrawRoad(triangle.center, triangle.center + perpendicularVector, triangle.bc);
        }
    }
    private void AddNeighbour(Triangle t1, Triangle t2, Edge e)
    {
        bool flag = false;
        for(int i = 0; i < t2.neighbours.Count; ++i)
        {
            if(t2.neighbours[i].triangle.center == t1.center)
            {
                flag = true;
                break;
            }
        }
        if (!flag)
        {
            t2.neighbours.Add(new Neighbour(t1, e));
            t1.neighbours.Add(new Neighbour(t2, e));
        }
    }
    private void DisplayVoronoi(List<Triangle> triangulation)
    {
        foreach(Triangle triangle in triangulation)
        {
            foreach(Neighbour neighbour in triangle.neighbours)
            {
                float distance;
                Vector3 vector = Vector3.zero;
                Vector3 newV = Vector3.zero;  
                if ((Mathf.Abs(triangle.center.x) > 5 || Mathf.Abs(triangle.center.y) > 5) && (Mathf.Abs(neighbour.triangle.center.x) > 5 || Mathf.Abs(neighbour.triangle.center.y) > 5))
                {
                    if(Mathf.Abs(triangle.center.x) > 5 && Mathf.Abs(neighbour.triangle.center.x) > 5)
                    {
                        continue;
                    }
                    if(Mathf.Abs(triangle.center.y) > 5 && Mathf.Abs(neighbour.triangle.center.y) > 5)
                    {
                        continue;
                    }
                    bool flag = false;
                    Vector3 firstPoint = triangle.center;
                    Vector3 secondPoint = neighbour.triangle.center;
                    vector = triangle.center - neighbour.triangle.center;
                    if (Mathf.Abs(triangle.center.x) > 5)
                    {
                        distance = triangle.center.x > 0 ? 5 - neighbour.triangle.center.x : -5 - neighbour.triangle.center.x;
                        if(Mathf.Abs(vector.y*(distance/vector.x)) <= 5)
                        {
                            flag = true;
                            firstPoint.x = neighbour.triangle.center.x + distance;
                            firstPoint.y = neighbour.triangle.center.y + (vector.y * (distance / vector.x));
                        }
                    }
                    if (Mathf.Abs(triangle.center.y) > 5)
                    {
                        distance = triangle.center.y > 0 ? 5 - neighbour.triangle.center.y : -5 - neighbour.triangle.center.y;
                        if (Mathf.Abs(vector.x * (distance / vector.y)) <= 5)
                        {
                            flag = true;
                            firstPoint.x = neighbour.triangle.center.x + (vector.x * (distance / vector.y));
                            firstPoint.y = neighbour.triangle.center.y + distance;
                        }
                    }
                    vector = neighbour.triangle.center - triangle.center;
                    if (Mathf.Abs(neighbour.triangle.center.x) > 5)
                    {
                        distance = neighbour.triangle.center.x > 0 ? 5 - triangle.center.x : -5 - triangle.center.x;
                        if (Mathf.Abs(vector.y * (distance / vector.x)) <= 5)
                        {
                            flag = true;
                            secondPoint.x = triangle.center.x + distance;
                            secondPoint.y = triangle.center.y + (vector.y * (distance / vector.x));
                        }
                    }
                    if (Mathf.Abs(neighbour.triangle.center.y) > 5)
                    {
                        distance = neighbour.triangle.center.y > 0 ? 5 - triangle.center.y : -5 - triangle.center.y;
                        if (Mathf.Abs(vector.x * (distance / vector.y)) <= 5)
                        {
                            flag = true;
                            secondPoint.x = triangle.center.x + (vector.x * (distance / vector.y));
                            secondPoint.y = triangle.center.y + distance;
                        }
                    }
                    if (flag)
                    {
                        DrawRoad(firstPoint, secondPoint, neighbour.edge);
                    }
                    else
                    {
                        continue;
                    }
                }
                else if(Mathf.Abs(triangle.center.x) > 5 || Mathf.Abs(triangle.center.y) > 5)
                {
                    vector = triangle.center - neighbour.triangle.center;
                    if (Mathf.Abs(triangle.center.x) > 5)
                    {
                        distance = triangle.center.x > 0 ? 5 - neighbour.triangle.center.x : -5 - neighbour.triangle.center.x;
                        newV = new Vector3(distance, vector.y * (distance/vector.x));
                    }
                    if (Mathf.Abs(triangle.center.y) > 5)
                    {
                        distance = triangle.center.y > 0 ? 5 - neighbour.triangle.center.y : -5 - neighbour.triangle.center.y;
                        newV = new Vector3(vector.x * (distance / vector.y), distance);
                    }
                    DrawRoad(neighbour.triangle.center + newV, neighbour.triangle.center, neighbour.edge);
                }
                else if (Mathf.Abs(neighbour.triangle.center.x) > 5 || Mathf.Abs(neighbour.triangle.center.y) > 5)
                {
                    vector = neighbour.triangle.center - triangle.center;
                    if (Mathf.Abs(neighbour.triangle.center.x) > 5)
                    {
                        distance = neighbour.triangle.center.x > 0 ? 5 - triangle.center.x : -5 - triangle.center.x;
                        newV = new Vector3(distance, vector.y * (distance / vector.x));
                    }
                    if (Mathf.Abs(neighbour.triangle.center.y) > 5)
                    {
                        distance = neighbour.triangle.center.y > 0 ? 5 - triangle.center.y : -5 - triangle.center.y;
                        newV = new Vector3(vector.x * (distance / vector.y), distance);
                    }
                    DrawRoad(triangle.center + newV, triangle.center, neighbour.edge);
                } 
                else
                {
                    DrawRoad(triangle.center, neighbour.triangle.center, neighbour.edge);
                }
            }
        }
    }
    private void DrawRoad(Vector3 a, Vector3 b, Edge edge)
    {
        for (int i = 0; i < voronoi.Count; ++i)
        {
            if (voronoi[i].center == edge.first)
            {
                voronoi[i].AddPoint(a);
                voronoi[i].AddPoint(b);
            }
            if (voronoi[i].center == edge.second)
            {
                voronoi[i].AddPoint(a);
                voronoi[i].AddPoint(b);
            }
        }
        if (draw)
        {
            GameObject road = new GameObject("road");
            road = RoadSetup(road, a, b);
        }
    }
    private GameObject RoadSetup(GameObject go, Vector3 a, Vector3 b)
    {
        var line = go.AddComponent<LineRenderer>();
        line.SetPosition(0, a);
        line.SetPosition(1, b);
        
        line.sortingOrder = 1;
        line.material = roadTexture;
        line.textureMode = LineTextureMode.Tile;
        line.startWidth = 0.05f;
        line.endWidth = 0.05f;

        return go;
    }
}
