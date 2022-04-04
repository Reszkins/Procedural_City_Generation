using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityArea : MonoBehaviour
{
    public float maxX = 4.5f;
    public float minX = -4.5f;
    public float maxY = 4.5f;
    public float minY = -4.5f;

    public Material borderTexture;
    private void Start()
    {
        DrawArea();
    }

    private void DrawArea()
    {
        GameObject go = new GameObject("border");
        var line = go.AddComponent<LineRenderer>();
        line.positionCount = 5;
        line.SetPosition(0, new Vector3(maxX, maxY, 0));
        line.SetPosition(1, new Vector3(maxX, minY, 0));
        line.SetPosition(2, new Vector3(minX, minY, 0));
        line.SetPosition(3, new Vector3(minX, maxY, 0));
        line.SetPosition(4, new Vector3(maxX, maxY, 0));

        line.sortingOrder = 1;
        line.material = borderTexture;
        line.textureMode = LineTextureMode.Tile;
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;
    }
}
