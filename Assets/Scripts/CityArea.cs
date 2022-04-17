using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityArea : MonoBehaviour
{
    public Material borderTexture;
    public void DrawArea()
    {
        GameObject go = new GameObject("border");
        var line = go.AddComponent<LineRenderer>();
        line.positionCount = 5;
        line.SetPosition(0, new Vector3(GameManager.instance.maxX, GameManager.instance.maxY, 0));
        line.SetPosition(1, new Vector3(GameManager.instance.maxX, GameManager.instance.minY, 0));
        line.SetPosition(2, new Vector3(GameManager.instance.minX, GameManager.instance.minY, 0));
        line.SetPosition(3, new Vector3(GameManager.instance.minX, GameManager.instance.maxY, 0));
        line.SetPosition(4, new Vector3(GameManager.instance.maxX, GameManager.instance.maxY, 0));

        line.sortingOrder = 1;
        line.material = borderTexture;
        line.textureMode = LineTextureMode.Tile;
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;
    }
}
