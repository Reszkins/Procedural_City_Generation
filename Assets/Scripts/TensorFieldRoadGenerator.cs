using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TensorFieldRoadGenerator : MonoBehaviour
{
    public Material majorTexture;
    public Material minorTexture;

    private Vector3 center = Vector3.zero;
    public void Test()
    {
        Vector3 direction = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f));

        while(direction.x == 0f || direction.y == 0f)
        {
            direction = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f));
        }

        Tensor tensor = GridTensorField(direction);

        for(int i = 0; i < 5; ++i)
        {
            for(int j = 0; j < 5; ++j)
            {
                Vector3 p = new Vector3(i, j, 0);
                Tensor tensor1 = RadialTensorField(p);

                DrawRoad(p, p + tensor1.major, true);
                DrawRoad(p, p + tensor1.minor, false);
                DrawRoad(p, p - tensor1.major, true);
                DrawRoad(p, p - tensor1.minor, false);

                Vector3 p1 = new Vector3(-i, -j, 0);
                Tensor tensor2 = RadialTensorField(p1);

                DrawRoad(p1, p1 + tensor2.major, true);
                DrawRoad(p1, p1 + tensor2.minor, false);
                DrawRoad(p1, p1 - tensor2.major, true);
                DrawRoad(p1, p1 - tensor2.minor, false);

                Vector3 p2 = new Vector3(i, -j, 0);
                Tensor tensor3 = RadialTensorField(p2);

                DrawRoad(p2, p2 + tensor3.major, true);
                DrawRoad(p2, p2 + tensor3.minor, false);
                DrawRoad(p2, p2 - tensor3.major, true);
                DrawRoad(p2, p2 - tensor3.minor, false);

                Vector3 p3 = new Vector3(-i, j, 0);
                Tensor tensor4 = RadialTensorField(p3);

                DrawRoad(p3, p3 + tensor4.major, true);
                DrawRoad(p3, p3 + tensor4.minor, false);
                DrawRoad(p3, p3 - tensor4.major, true);
                DrawRoad(p3, p3 - tensor4.minor, false);
            }
        }
    }

    public Tensor RadialTensorField(Vector3 point)
    {
        float t = Mathf.Atan(point.y / point.x);

        float x = center.x - point.x;
        float y = center.y - point.y;

        float diag1 = y * y - x * x;
        float diag2 = -(y * y - x * x);
        float ndiag = -2 * x * y;

        Debug.Log(diag1 + " " + diag2 + " " + ndiag + " point: " + point);

        float x1, x2;
        Quadratic(1f, -(diag1 * diag2), (diag1 * diag2) - ndiag * ndiag, out x1, out x2);

        Vector3 major = x1 * new Vector3(Mathf.Cos(t), Mathf.Sin(t));
        Vector3 minor = x2 * new Vector3(Mathf.Cos(t + Mathf.PI / 2), Mathf.Sin(t + Mathf.PI / 2));

        Tensor tensor = new Tensor(major.normalized * 0.2f, minor.normalized * 0.2f);
        return tensor;
    }
    public Tensor GridTensorField(Vector3 direction)
    {
        float l = Mathf.Sqrt(direction.x * direction.x + direction.y * direction.y);
        float t = Mathf.Atan(direction.y / direction.x);

        float diag1 = Mathf.Cos(2 * t) * l;
        float diag2 = -Mathf.Cos(2 * t) * l;
        float ndiag = Mathf.Sin(2 * t) * l;

        float x1, x2;
        Quadratic(1f, -(diag1 * diag2), (diag1 * diag2) - ndiag * ndiag, out x1, out x2);

        Vector3 major = x1 * new Vector3(Mathf.Cos(t), Mathf.Sin(t));
        Vector3 minor = x2 * new Vector3(Mathf.Cos(t + Mathf.PI/2), Mathf.Sin(t + Mathf.PI / 2));

        Tensor tensor = new Tensor(major, minor);
        return tensor;
    }
    private void Quadratic(float a, float b, float c, out float x1, out float x2)
    {
        float deltaRoot = Mathf.Sqrt(b * b - 4 * a * c);
        x1 = (-b + deltaRoot) / 2 * a;
        x2 = (-b - deltaRoot) / 2 * a;
    }



    private void DrawRoad(Vector3 a, Vector3 b, bool major)
    {
            GameObject road = new GameObject("Road");
            road = RoadSetup(road, a, b, major);
    }
    private GameObject RoadSetup(GameObject go, Vector3 a, Vector3 b, bool major)
    {
        var line = go.AddComponent<LineRenderer>();
        line.SetPosition(0, a);
        line.SetPosition(1, b);

        line.sortingOrder = 1;
        if (major)
        {
            line.material = majorTexture;
        }
        else
        {
            line.material = minorTexture;
        }
        
        line.textureMode = LineTextureMode.Tile;
        line.startWidth = 0.02f;
        line.endWidth = 0.02f;
        line.sortingLayerName = "Road";

        return go;
    }
}
