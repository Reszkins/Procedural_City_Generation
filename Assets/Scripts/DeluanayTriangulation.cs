using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeluanayTriangulation : MonoBehaviour
{
    private float minX;
    private float maxX;
    private float minY;
    private float maxY;
    private int pointsNumber;

    private bool deluanay;

    private Sprite pointTexture;

    private List<Vector3> points = new List<Vector3>();
    private List<Triangle> triangles = new List<Triangle>();

    public Material roadTexture;

    public void Setup()
    {
        this.minX = GameManager.instance.minX;
        this.maxX = GameManager.instance.maxX;
        this.minY = GameManager.instance.minY;
        this.maxY = GameManager.instance.maxY;
        this.pointsNumber = GameManager.instance.pointsNumber;
        this.deluanay = GameManager.instance.deluanay;
        //this.pointTexture = GameManager.instance.pointTexture;
        //this.roadTexture = GameManager.instance.roadTexture;
    }
    public void ResetData()
    {
        points = GameManager.instance.points;
        triangles = new List<Triangle>();
    }
    public void DrawPoints()
    {
        //Random.seed = -1793400948;
        for (int i = 0; i < pointsNumber; ++i)
        {
            CreatePoint(new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), 0));
        }
    }
    public void GetPoints(List<Vector3> p)
    {
        points = p;
        triangles = new List<Triangle>();
    }
    public void Deluanay()
    {
        triangles = BowyerWatson();
        //Debug.Log(GameManager.instance.deluanay + " " + triangles.Count);
        if (GameManager.instance.deluanay)
        {
            foreach (Triangle triangle in triangles)
            {
                //Debug.Log("EXCUSE ME");
                DrawTriangle(triangle);
            }
        }
        GameManager.instance.triangles = triangles;
        GameManager.instance.points = points;
        //Debug.Log(GameManager.instance.deluanay + " " + triangles.Count);
    }
    private void CreatePoint(Vector3 position)
    {
        points.Add(position);
        //DrawPoint(position);
    }
    private void DrawPoint(Vector3 position)
    {
        /*GameObject go = new GameObject("point");
        go.transform.position = position;
        var square = go.AddComponent<SpriteRenderer>();
        square.sprite = pointTexture;
        square.transform.localScale = new Vector3(0.6f, 0.6f, 0);*/
    }
    private void CreateTriangle(Vector3 a, Vector3 b, Vector3 c)
    {
        Triangle triangle = new Triangle(a, b, c);
        triangles.Add(triangle);
        CalculateCenterAndRadius(triangle);
    }
    private void DrawTriangle(Triangle t)
    {
        GameObject lineAB = new GameObject("line");
        GameObject lineAC = new GameObject("line");
        GameObject lineBC = new GameObject("line");

        lineAB = RoadSetup(lineAB, t.a, t.b);
        lineAC = RoadSetup(lineAC, t.a, t.c);
        lineBC = RoadSetup(lineBC, t.b, t.c);
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
    private void CalculateCenterAndRadius(Triangle t)
    {
        float d = 2 * (t.a.x * (t.b.y - t.c.y) + t.b.x * (t.c.y - t.a.y) + t.c.x*(t.a.y - t.b.y));
        Vector3 center;
        center.x = (1 / d) * 
            ((((t.a.x * t.a.x) + (t.a.y * t.a.y)) * (t.b.y - t.c.y)) + 
            (((t.b.x * t.b.x) + (t.b.y * t.b.y)) * (t.c.y - t.a.y)) + 
            (((t.c.x * t.c.x) + (t.c.y * t.c.y)) * (t.a.y - t.b.y)));

        center.y = (1 / d) * 
            ((((t.a.x * t.a.x) + (t.a.y * t.a.y)) * (t.c.x - t.b.x)) +
            (((t.b.x * t.b.x) + (t.b.y * t.b.y)) * (t.a.x - t.c.x)) +
            (((t.c.x * t.c.x) + (t.c.y * t.c.y)) * (t.b.x - t.a.x)));

        center.z = 0;

        t.center = center;

        Vector3 vector = new Vector3(center.x-t.a.x,center.y-t.a.y,0);
        t.radius = Vector3.Magnitude(vector);
    }
    private List<Triangle> BowyerWatson()
    {
        List<Triangle> triangulation = new List<Triangle>();
        List<Triangle> badTriangles = new List<Triangle>();
        List<Edge> polygon = new List<Edge>();
        List<Edge> badEdges = new List<Edge>();
        Triangle superTriangle = new Triangle(new Vector3(0, 100, 0), new Vector3(100, -50, 0), new Vector3(-100, -50, 0));
        CalculateCenterAndRadius(superTriangle);
        //DrawTriangle(superTriangle);

        triangulation.Add(superTriangle);
        int n = 0;
        foreach(Vector3 point in points)
        {
            badTriangles.Clear();
            n++;
            foreach(Triangle triangle in triangulation)
            {
                if(Vector3.Magnitude(new Vector3(point.x - triangle.center.x, point.y - triangle.center.y, 0)) < triangle.radius)
                {
                    badTriangles.Add(triangle);
                }
            }

            polygon.Clear();
            badEdges.Clear();

            foreach(Triangle triangle in badTriangles)
            {
                foreach(Edge edge in triangle.edges)
                {
                    Edge edgeToRemove = null;
                    bool flag = false;
                    foreach(Edge badEdge in badEdges)
                    {
                        if((edge.first == badEdge.first && edge.second == badEdge.second) || (edge.first == badEdge.second && edge.second == badEdge.first))
                        {
                            flag = true;
                            break;
                        }
                    }
                    foreach(Edge polyEdge in polygon)
                    {
                        if((edge.first == polyEdge.first && edge.second == polyEdge.second) || (edge.first == polyEdge.second && edge.second == polyEdge.first))
                        {
                            edgeToRemove = polyEdge;
                            badEdges.Add(polyEdge);
                            flag = true;
                        }
                    }
                    if (!flag)
                    {
                        polygon.Add(edge);
                    } else
                    {
                        polygon.Remove(edge);
                        polygon.Remove(edgeToRemove);
                    }
                }
            }

            foreach (Triangle triangle in badTriangles)
            {
                triangulation.Remove(triangle);
            }

            foreach(Edge edge in polygon)
            {
                Triangle newTri = new Triangle(point, edge.first, edge.second);
                CalculateCenterAndRadius(newTri);
                triangulation.Add(newTri);
            }
        }

        triangulation.RemoveAll(triangle => triangle.a == superTriangle.a || triangle.a == superTriangle.b || triangle.a == superTriangle.c);
        triangulation.RemoveAll(triangle => triangle.b == superTriangle.a || triangle.b == superTriangle.b || triangle.b == superTriangle.c);
        triangulation.RemoveAll(triangle => triangle.c == superTriangle.a || triangle.c == superTriangle.b || triangle.c == superTriangle.c); 

        return triangulation;
    }
}
