using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        if(GameManager.instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public float minX = -5f;
    public float maxX = 5f;
    public float minY = -5f;
    public float maxY = 5f;
    public int pointsNumber = 5;
    public bool deluanay;
    public bool cityCenter;
    public int lloyd;
    public Sprite pointTexture;
    public Material roadTexture;

    public Aglomeration aglomeration;
    public DeluanayTriangulation deluanayTriangulation;
    public VoronoiDiagram voronoiDiagram;

    [HideInInspector]
    public List<Vector3> points = new List<Vector3>();
    [HideInInspector]
    public List<Triangle> triangles = new List<Triangle>();
    [HideInInspector]
    public List<VoronoiElement> districts = new List<VoronoiElement>();

    private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            deluanayTriangulation.Setup();
            deluanayTriangulation.DrawPoints();
            deluanayTriangulation.Deluanay();
            while (lloyd > 0)
            {
                lloyd--;
                voronoiDiagram.Setup();
                voronoiDiagram.Construct(triangles, points);
                voronoiDiagram.CalculateNewPoints();
                deluanayTriangulation.ResetData();
                deluanayTriangulation.Deluanay();
            }
            voronoiDiagram.Setup();
            voronoiDiagram.ConstructAndDisplay(triangles, points);
            voronoiDiagram.GetDistricts();
            aglomeration.CreateRoads();
        }
    }
}
