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

    public float minX;
    public float maxX;
    public float minY;
    public float maxY;
    public int pointsNumber = 5;
    public bool deluanay;
    //public bool cityCenter;
    public int lloyd;
    //public Sprite pointTexture;
    //public Material roadTexture;
    public Sprite[] buildingTextures32x32 = new Sprite[9];
    public Sprite[] buildingTextures16x16 = new Sprite[8];
    public Sprite[] buildingTextures16x32 = new Sprite[4];  
    public Vector3 cityCenter;

    public Aglomeration aglomeration;
    public DeluanayTriangulation deluanayTriangulation;
    public VoronoiDiagram voronoiDiagram;
    public CityArea cityArea;
    public MainRoadGenerator mainRoadGenerator;
  

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
            CalculateLimits();
            cityArea.DrawArea();
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

            mainRoadGenerator.GenerateMainRoads();

            aglomeration.CreateRoads();
            aglomeration.CreateBuildings();
        }
    }
    private void CalculateLimits()
    {
        minX = Mathf.Sqrt(pointsNumber) * -1;
        minY = Mathf.Sqrt(pointsNumber) * -1;
        maxX = Mathf.Sqrt(pointsNumber);
        maxY = Mathf.Sqrt(pointsNumber);

        Debug.Log("minX " + minX + " maxX " + maxX + " minY " + minY + " maxY " + maxY);
    }
}
