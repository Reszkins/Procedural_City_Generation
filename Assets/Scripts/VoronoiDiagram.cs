using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoronoiDiagram : MonoBehaviour
{ 
    public Material forestMaterial;
    public Material cityMaterial;
    private Material roadTexture;
    private bool draw = false;

    private List<VoronoiElement> voronoi;
    public Sprite pointTexture;
    public Sprite cityCenterTexture;
    public Sprite officeDistrictTexture;
    public Sprite residentialDistrictTexture;
    public Sprite forestTexture;

    public void Setup()
    {
        this.roadTexture = GameManager.instance.roadTexture;
        voronoi = new List<VoronoiElement>();
    }
    public void CalculateNewPoints()
    {
        List<Vector3> points = new List<Vector3>();
        Vector3 newPoint;
        foreach (VoronoiElement voronoi in voronoi)
        {
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
            if(cy > GameManager.instance.maxY || cx > GameManager.instance.maxX)
            {
                Debug.Log(cy + " " + cx);
            }
            newPoint = new Vector3(cx,cy);
            if(voronoi.points.Count == 0)
            {
                int seed = Random.seed;
                Debug.Log("ZERO!! " + seed);
                Debug.Log(voronoi.center);
            }
            points.Add(newPoint);
            GameManager.instance.points = points;
        }
    }
    private void ChooseCityCenterAndCalculateDistrictClasses()
    {
        float mini = float.MaxValue;
        int index = 0;
        for (int i = 0; i < voronoi.Count; ++i)
        {
            if (Mathf.Abs(voronoi[i].center.x) + Mathf.Abs(voronoi[i].center.y) < mini)
            {
                index = i;
                mini = Mathf.Abs(voronoi[i].center.x) + Mathf.Abs(voronoi[i].center.y);
            }
        }
        Vector3 cityCenter = GameManager.instance.points[index];
        //Debug.Log(cityCenter);
        Vector3 vector;
        for (int i = 0; i < voronoi.Count; ++i)
        {
            if (voronoi[i].center != cityCenter)
            {
                vector = cityCenter - voronoi[i].center;
                if (vector.magnitude > GameManager.instance.maxX * 0.85f)
                {
                    voronoi[i].type = DistrictType.Forest;
                }
                else if (vector.magnitude > GameManager.instance.maxX * 0.4f)
                {
                    voronoi[i].type = DistrictType.ResidentialDistrict;
                }
                else if (vector.magnitude < GameManager.instance.maxX * 0.4f)
                {
                    voronoi[i].type = DistrictType.OfficeDistrict;
                }
            }
            else
            {
                voronoi[i].type = DistrictType.CityCenter;
            }
        }
    }
    private void DrawPoint(VoronoiElement district)
    {
        GameObject go = new GameObject("point");
        go.transform.position = district.center;
        var square = go.AddComponent<SpriteRenderer>();
        if(district.type == DistrictType.Default)
        {
            square.sprite = pointTexture;
        }
        else if(district.type == DistrictType.CityCenter)
        {
            square.sprite = cityCenterTexture;
        }
        else if (district.type == DistrictType.OfficeDistrict)
        {
            square.sprite = officeDistrictTexture;
        }
        else if (district.type == DistrictType.ResidentialDistrict)
        {
            square.sprite = residentialDistrictTexture;
        }
        else if (district.type == DistrictType.Forest)
        {
            square.sprite = forestTexture;
        }

        square.transform.localScale = new Vector3(0.6f, 0.6f, 0);
    }
    public void ConstructAndDisplay(List<Triangle> triangulation, List<Vector3> points)
    {
        draw = true;
        Construct(triangulation, points);
        //if(GameManager.instance.cityCenter) ChooseCityCenterAndCalculateDistrictClasses();
        /*for (int i = 0; i < voronoi.Count; ++i)
        {
            DrawPoint(voronoi[i]);
        }*/
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
        CalculateCorners();
        PolarAngleSort();
        if (GameManager.instance.cityCenter) ChooseCityCenterAndCalculateDistrictClasses();
        if (draw)
        {
            CreateMeshes();
        }
    }
    public void GetDistricts()
    {
        GameManager.instance.districts = voronoi;
    }
    private void CreateMeshes()
    {
        foreach(VoronoiElement voronoi in voronoi)
        {
            GameObject districtMesh = new GameObject("district");
            var meshRenderer = districtMesh.AddComponent<MeshRenderer>();
            var meshFilter = districtMesh.AddComponent<MeshFilter>();
            Mesh mesh = new Mesh();
            Vector3[] vertices = new Vector3[voronoi.points.Count + 1];
            int[] triangles = new int[voronoi.points.Count * 3];
            for(int i = 0; i < voronoi.points.Count; ++i)
            {
                vertices[i] = voronoi.points[i];
            }
            vertices[voronoi.points.Count] = voronoi.center;
            int j = 0;
            for(int i=1;i<voronoi.points.Count + 1; ++i)
            {
                triangles[j++] = i - 1;
                triangles[j++] = i % voronoi.points.Count;
                triangles[j++] = voronoi.points.Count;
            }
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            meshFilter.mesh = mesh;
            if(voronoi.type == DistrictType.Forest)
            {
                meshRenderer.material = forestMaterial;
            }
            else
            {
                meshRenderer.material = cityMaterial;
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
                if(point.x == GameManager.instance.maxX)
                {
                    topX = true;
                }
                if (point.x == GameManager.instance.minX)
                {
                    botX = true;
                }
                if (point.y == GameManager.instance.maxY)
                {
                    topY = true;
                }
                if (point.y == GameManager.instance.minY)
                {
                    botY = true;
                }
            }
            if(topX && topY)
            {
                voronoi.points.Add(new Vector3(GameManager.instance.maxX, GameManager.instance.maxY));
            }
            if (topX && botY)
            {
                voronoi.points.Add(new Vector3(GameManager.instance.maxX, GameManager.instance.minY));
            }
            if (botX && topY)
            {
                voronoi.points.Add(new Vector3(GameManager.instance.minX, GameManager.instance.maxY));
            }
            if (botX && botY)
            {
                voronoi.points.Add(new Vector3(GameManager.instance.minX, GameManager.instance.minY));
            }
        }
    }
    private void PolarAngleSort()
    {
        Vector3 centerOfPoints;
        foreach (VoronoiElement voronoi in voronoi)
        {
            /*centerOfPoints = Vector3.zero;
            for (int i = 0; i < voronoi.points.Count; ++i)
            {
                centerOfPoints.x += voronoi.points[i].x;
                centerOfPoints.y += voronoi.points[i].y;
            }
            centerOfPoints.x /= voronoi.points.Count;
            centerOfPoints.y /= voronoi.points.Count;*/

            centerOfPoints = voronoi.center;
            
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
    private void CalculateThirdLine(Triangle triangle)
    {
        if (Mathf.Abs(triangle.center.x) > GameManager.instance.maxX || Mathf.Abs(triangle.center.y) > GameManager.instance.maxY)
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

            while(Vector3.Magnitude(perpendicularVector) < GameManager.instance.maxX * 2)
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
            if (triangle.center.x + perpendicularVector.x > GameManager.instance.maxX)
            {
                distance = GameManager.instance.maxX - triangle.center.x;
                tmp = new Vector3(distance, perpendicularVector.y * (distance / perpendicularVector.x));
                perpendicularVector = tmp;
            }
            if (triangle.center.y + perpendicularVector.y > GameManager.instance.maxY)
            {
                distance = GameManager.instance.maxY - triangle.center.y;
                tmp = new Vector3(perpendicularVector.x * (distance / perpendicularVector.y),distance);
                perpendicularVector = tmp;
            }
            if (triangle.center.x + perpendicularVector.x < GameManager.instance.minX)
            {
                distance = GameManager.instance.minX - triangle.center.x;
                tmp = new Vector3(distance, perpendicularVector.y * (distance / perpendicularVector.x));
                perpendicularVector = tmp;
            } 
            if (triangle.center.y + perpendicularVector.y < GameManager.instance.minY)
            {
                distance = GameManager.instance.minY - triangle.center.y;
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

            while (Vector3.Magnitude(perpendicularVector) < GameManager.instance.maxX * 2)
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
            if (triangle.center.x + perpendicularVector.x > GameManager.instance.maxX)
            {
                distance = GameManager.instance.maxX - triangle.center.x;
                tmp = new Vector3(distance, perpendicularVector.y * (distance / perpendicularVector.x));
                perpendicularVector = tmp;
            }
            if (triangle.center.y + perpendicularVector.y > GameManager.instance.maxY)
            {
                distance = GameManager.instance.maxY - triangle.center.y;
                tmp = new Vector3(perpendicularVector.x * (distance / perpendicularVector.y), distance);
                perpendicularVector = tmp;
            }
            if (triangle.center.x + perpendicularVector.x < GameManager.instance.minX)
            {
                distance = GameManager.instance.minX - triangle.center.x;
                tmp = new Vector3(distance, perpendicularVector.y * (distance / perpendicularVector.x));
                perpendicularVector = tmp;
            }
            if (triangle.center.y + perpendicularVector.y < GameManager.instance.minY)
            {
                distance = GameManager.instance.minY - triangle.center.y;
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

            while (Vector3.Magnitude(perpendicularVector) < GameManager.instance.maxX * 2)
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
            if (triangle.center.x + perpendicularVector.x > GameManager.instance.maxX)
            {
                distance = GameManager.instance.maxX - triangle.center.x;
                tmp = new Vector3(distance, perpendicularVector.y * (distance / perpendicularVector.x));
                perpendicularVector = tmp;
            }
            if (triangle.center.y + perpendicularVector.y > GameManager.instance.maxY)
            {
                distance = GameManager.instance.maxY - triangle.center.y;
                tmp = new Vector3(perpendicularVector.x * (distance / perpendicularVector.y), distance);
                perpendicularVector = tmp;
            }
            if (triangle.center.x + perpendicularVector.x < GameManager.instance.minX)
            {
                distance = GameManager.instance.minX - triangle.center.x;
                tmp = new Vector3(distance, perpendicularVector.y * (distance / perpendicularVector.x));
                perpendicularVector = tmp;
            }
            if (triangle.center.y + perpendicularVector.y < GameManager.instance.minY)
            {
                distance = GameManager.instance.minY - triangle.center.y;
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
                if ((Mathf.Abs(triangle.center.x) > GameManager.instance.maxX || Mathf.Abs(triangle.center.y) > GameManager.instance.maxY) && 
                    (Mathf.Abs(neighbour.triangle.center.x) > GameManager.instance.maxX || Mathf.Abs(neighbour.triangle.center.y) > GameManager.instance.maxY))
                {
                    if(Mathf.Abs(triangle.center.x) > GameManager.instance.maxX && Mathf.Abs(neighbour.triangle.center.x) > GameManager.instance.maxX)
                    {
                        continue;
                    }
                    if(Mathf.Abs(triangle.center.y) > GameManager.instance.maxY && Mathf.Abs(neighbour.triangle.center.y) > GameManager.instance.maxY)
                    {
                        continue;
                    }
                    bool flag = false;
                    Vector3 firstPoint = triangle.center;
                    Vector3 secondPoint = neighbour.triangle.center;
                    vector = triangle.center - neighbour.triangle.center;
                    if (Mathf.Abs(triangle.center.x) > GameManager.instance.maxX)
                    {
                        distance = triangle.center.x > 0 ? GameManager.instance.maxX - neighbour.triangle.center.x : GameManager.instance.minX - neighbour.triangle.center.x;
                        if(Mathf.Abs(vector.y*(distance/vector.x)) <= GameManager.instance.maxX)
                        {
                            flag = true;
                            firstPoint.x = neighbour.triangle.center.x + distance;
                            firstPoint.y = neighbour.triangle.center.y + (vector.y * (distance / vector.x));
                        }
                    }
                    if (Mathf.Abs(triangle.center.y) > GameManager.instance.maxY)
                    {
                        distance = triangle.center.y > 0 ? GameManager.instance.maxY - neighbour.triangle.center.y : GameManager.instance.minY - neighbour.triangle.center.y;
                        if (Mathf.Abs(vector.x * (distance / vector.y)) <= GameManager.instance.maxY)
                        {
                            flag = true;
                            firstPoint.x = neighbour.triangle.center.x + (vector.x * (distance / vector.y));
                            firstPoint.y = neighbour.triangle.center.y + distance;
                        }
                    }
                    vector = neighbour.triangle.center - triangle.center;
                    if (Mathf.Abs(neighbour.triangle.center.x) > GameManager.instance.maxX)
                    {
                        distance = neighbour.triangle.center.x > 0 ? GameManager.instance.maxX - triangle.center.x : GameManager.instance.minX - triangle.center.x;
                        if (Mathf.Abs(vector.y * (distance / vector.x)) <= GameManager.instance.maxX)
                        {
                            flag = true;
                            secondPoint.x = triangle.center.x + distance;
                            secondPoint.y = triangle.center.y + (vector.y * (distance / vector.x));
                        }
                    }
                    if (Mathf.Abs(neighbour.triangle.center.y) > GameManager.instance.maxY)
                    {
                        distance = neighbour.triangle.center.y > 0 ? GameManager.instance.maxY - triangle.center.y : GameManager.instance.minY - triangle.center.y;
                        if (Mathf.Abs(vector.x * (distance / vector.y)) <= GameManager.instance.maxY)
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
                else if(Mathf.Abs(triangle.center.x) > GameManager.instance.maxX || Mathf.Abs(triangle.center.y) > GameManager.instance.maxY)
                {
                    vector = triangle.center - neighbour.triangle.center;
                    if (Mathf.Abs(triangle.center.x) > GameManager.instance.maxX)
                    {
                        distance = triangle.center.x > 0 ? GameManager.instance.maxX - neighbour.triangle.center.x : GameManager.instance.minX - neighbour.triangle.center.x;
                        newV = new Vector3(distance, vector.y * (distance/vector.x));
                    }
                    if (Mathf.Abs(triangle.center.y) > GameManager.instance.maxY)
                    {
                        distance = triangle.center.y > 0 ? GameManager.instance.maxY - neighbour.triangle.center.y : GameManager.instance.minY - neighbour.triangle.center.y;
                        newV = new Vector3(vector.x * (distance / vector.y), distance);
                    }
                    DrawRoad(neighbour.triangle.center + newV, neighbour.triangle.center, neighbour.edge);
                }
                else if (Mathf.Abs(neighbour.triangle.center.x) > GameManager.instance.maxX || Mathf.Abs(neighbour.triangle.center.y) > GameManager.instance.maxY)
                {
                    vector = neighbour.triangle.center - triangle.center;
                    if (Mathf.Abs(neighbour.triangle.center.x) > GameManager.instance.maxX)
                    {
                        distance = neighbour.triangle.center.x > 0 ? GameManager.instance.maxX - triangle.center.x : GameManager.instance.minX - triangle.center.x;
                        newV = new Vector3(distance, vector.y * (distance / vector.x));
                    }
                    if (Mathf.Abs(neighbour.triangle.center.y) > GameManager.instance.maxY)
                    {
                        distance = neighbour.triangle.center.y > 0 ? GameManager.instance.maxY - triangle.center.y : GameManager.instance.minY - triangle.center.y;
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
