using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TensorFieldRoadGenerator : MonoBehaviour
{
    public Material majorTexture;
    public Material minorTexture;

    public List<Candidate> majorPoints = new List<Candidate>();
    public List<Candidate> minorPoints = new List<Candidate>();

    private Vector3 direction;
    private Vector3 center;
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
    public void GenerateRoads()
    {
        Vector3 dir = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f));

        while (dir.x == 0f || dir.y == 0f)
        {
            Debug.Log("XDDDDDDDDDDDDDDD");
            dir = new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f));
        }
        center = GameManager.instance.cityCenter;
        direction = dir;
        Debug.Log("ojæ");
        RadialStreamline(new Vector3(1, 1, 0));
    }

    private void RadialStreamline(Vector3 startPoint)
    {
        bool tracking = true; // true - major, false - minor

        Comparer<Vector3> comparator = Comparer<Vector3>.Create((x, y) => Vector3.Magnitude(x - GameManager.instance.cityCenter).CompareTo(Vector3.Magnitude(y - GameManager.instance.cityCenter)));
        PriorityQueue<Vector3> majorCandidates = new PriorityQueue<Vector3>();
        PriorityQueue<Vector3> minorCandidates = new PriorityQueue<Vector3>();

        List<Candidate> majorStreamline = new List<Candidate>();
        List<Candidate> minorStreamline = new List<Candidate>();

        //majorCandidates.Add(startPoint,comparator);
        
        Vector3 currentPoint = startPoint;
        bool stop = false;
        int licznik = 0;

        while(!stop)
        {
            //licznik++;
            if (tracking)
            {
                majorStreamline.Add(new Candidate(startPoint));
                // one direction
                currentPoint = startPoint;
                bool v = true;
                int pom = 0;
                while (Sorient(currentPoint + TensorField(currentPoint).major))
                {
                    v = true;
                    foreach (Candidate p in majorPoints)
                    {
                        if (Vector3.Magnitude(p.point - currentPoint + TensorField(currentPoint).major) < 0.4f)
                        {
                            v = false;
                        }
                    }
                    if (v)
                    {
                        DrawRoad(currentPoint, currentPoint + TensorField(currentPoint).major, false);
                        currentPoint += TensorField(currentPoint).major;
                        majorStreamline.Add(new Candidate(currentPoint));
                        minorCandidates.Add(currentPoint, comparator);
                    }
                    if (!v)
                    {
                        break;
                    }
                }
                // other direction
                currentPoint = startPoint;
                pom = 0;
                while (Sorient(currentPoint - TensorField(currentPoint).major))
                {
                    v = true;
                    foreach (Candidate p in majorPoints)
                    {
                        if (Vector3.Magnitude(p.point - currentPoint - TensorField(currentPoint).major) < 0.4f)
                        {
                            v = false;
                        }
                    }
                    if (v)
                    {
                        DrawRoad(currentPoint, currentPoint - TensorField(currentPoint).major, false);
                        currentPoint -= TensorField(currentPoint).major;
                        majorStreamline.Add(new Candidate(currentPoint));
                        minorCandidates.Add(currentPoint, comparator);
                    }
                    if (!v)
                    {
                        break;
                    }
                }  
            }
            else
            {
                bool v = true;
                minorStreamline.Add(new Candidate(startPoint));
                // one direction
                currentPoint = startPoint;
                int pom = 0;
                while (Sorient(currentPoint + TensorField(currentPoint).minor))
                {
                    if(currentPoint.x > center.x)
                    {
                        v = true;
                        foreach(Candidate p in minorPoints)
                        {
                            if(Vector3.Magnitude(p.point - currentPoint + TensorField(currentPoint).minor) < 0.4f)
                            {
                                v= false;
                            }
                        }
                        if (v)
                        {
                            DrawRoad(currentPoint, currentPoint + TensorField(currentPoint).minor, false);
                            currentPoint = currentPoint + TensorField(currentPoint).minor;
                            minorStreamline.Add(new Candidate(currentPoint));
                            majorCandidates.Add(currentPoint, comparator);
                        }   
                    }
                    else
                    {
                        v = true;
                        foreach (Candidate p in minorPoints)
                        {
                            if (Vector3.Magnitude(p.point - currentPoint - TensorField(currentPoint).minor) < 0.4f)
                            {
                                v = false;
                            }
                        }
                        if (v)
                        {
                            DrawRoad(currentPoint, currentPoint - TensorField(currentPoint).minor, false);
                            currentPoint = currentPoint - TensorField(currentPoint).minor;
                            minorStreamline.Add(new Candidate(currentPoint));
                            majorCandidates.Add(currentPoint, comparator);
                        }
                    }
                    if(!v)
                    {
                        break;
                    }                   
                }
            }
            foreach(Candidate c in majorStreamline)
            {
                majorPoints.Add(c);
            }
            foreach (Candidate c in minorStreamline)
            {
                minorPoints.Add(c);
            }
            //next point
            bool valid = true;
            if (tracking)
            {
                Vector3 candidate = Vector3.zero;
                tracking = false;
                do
                {
                    if (minorCandidates.Empty())
                    {
                        stop = true;
                        break;
                    }
                    candidate = minorCandidates.Get();
                    valid = true;
                    foreach(Candidate point in minorPoints)
                    {
                        if(Vector3.Magnitude(candidate - point.point) < 0.7f)
                        {   
                            valid = false;
                            break;
                        }
                    }
                } while (valid == false);

                startPoint = candidate;
            }
            else
            {
                Vector3 candidate = Vector3.zero;
                tracking = true;
                do
                {
                    if (majorCandidates.Empty())
                    {
                        stop = true;
                        break;
                    }
                    candidate = majorCandidates.Get();
                    valid = true;
                    foreach (Candidate point in majorPoints)
                    {
                        if (Vector3.Magnitude(candidate - point.point) < 0.7f)
                        {
                            valid = false;
                            break;
                        }
                    }
                } while (valid == false);

                startPoint = candidate;
            }
        }

    }

    private void GridStreamline(Vector3 startPoint)
    {
        bool tracking = true; // true - major, false - minor
        //majorPoints.Add(new Candidate(startPoint));
        Comparer<Vector3> comparator = Comparer<Vector3>.Create((x, y) => Vector3.Magnitude(x - GameManager.instance.cityCenter).CompareTo(Vector3.Magnitude(y - GameManager.instance.cityCenter)));
        PriorityQueue<Vector3> majorCandidates = new PriorityQueue<Vector3>();
        PriorityQueue<Vector3> minorCandidates = new PriorityQueue<Vector3>();
        majorCandidates.Add(startPoint, comparator);

        Vector3 currentPoint = Vector3.zero;
        bool stop = false;
        int licznik = 0;
        //Debug.Log("HALO KURWA? XD");
        while (!stop)
        {
            //Debug.Log(stop);
            if (tracking)
            {
                majorPoints.Add(new Candidate(startPoint));
                // one direction
                currentPoint = startPoint;
                while (Sorient(currentPoint + TensorField(currentPoint).major))
                {
                    //Debug.Log("co tam");
                    DrawRoad(currentPoint, currentPoint + TensorField(currentPoint).major, false);
                    currentPoint += TensorField(currentPoint).major;
                    majorPoints.Add(new Candidate(currentPoint));
                    minorCandidates.Add(currentPoint, comparator);
                }
                // other direction
                currentPoint = startPoint;
                while (Sorient(currentPoint - TensorField(currentPoint).major))
                {
                    //Debug.Log("co tam");
                    DrawRoad(currentPoint, currentPoint - TensorField(currentPoint).major, false);
                    currentPoint -= TensorField(currentPoint).major;
                    majorPoints.Add(new Candidate(currentPoint));
                    minorCandidates.Add(currentPoint, comparator);
                }
            }
            else
            {
                minorPoints.Add(new Candidate(startPoint));
                // one direction
                currentPoint = startPoint;
                while (Sorient(currentPoint + TensorField(currentPoint).minor))
                {
                    DrawRoad(currentPoint, currentPoint + TensorField(currentPoint).minor, false);
                    currentPoint += TensorField(currentPoint).minor;
                    minorPoints.Add(new Candidate(currentPoint));
                    majorCandidates.Add(currentPoint, comparator);
                }
                // other direction
                currentPoint = startPoint;
                while (Sorient(currentPoint - TensorField(currentPoint).minor))
                {
                    DrawRoad(currentPoint, currentPoint - TensorField(currentPoint).minor, false);
                    currentPoint -= TensorField(currentPoint).minor;
                    minorPoints.Add(new Candidate(currentPoint));
                    majorCandidates.Add(currentPoint, comparator);
                }
            }
            //next point
            bool valid = true;
            if (tracking)
            {
                Vector3 candidate = Vector3.zero;
                tracking = false;
                do
                {
                    //Debug.Log(valid);
                    if (minorCandidates.Empty())
                    {
                        stop = true;
                        break;
                    }
                    candidate = minorCandidates.Get();
                    valid = true;
                    foreach (Candidate point in minorPoints)
                    {
                        if (Vector3.Magnitude(candidate - point.point) < 0.5f)
                        {
                            //Debug.Log("XD JESTEM TU");
                            valid = false;
                            break;
                        }
                    }
                } while (valid == false);

                startPoint = candidate;
            }
            else
            {
                Vector3 candidate = Vector3.zero;
                tracking = true;
                do
                {
                    //Debug.Log(valid + "2");
                    if (majorCandidates.Empty())
                    {
                        stop = true;
                        break;
                    }
                    candidate = majorCandidates.Get();
                    valid = true;
                    foreach (Candidate point in majorPoints)
                    {
                        if (Vector3.Magnitude(candidate - point.point) < 0.5f)
                        {
                            //Debug.Log("XD JESTEM TU 2");
                            valid = false;
                            break;
                        }
                    }
                } while (valid == false);

                startPoint = candidate;
            }
            /*while(candidates.Empty() == false)
            {
                candidates.Get();
            }*/
        }

    }

    private Tensor TensorField(Vector3 point)
    {
        return RadialTensorField(point);
    }
    private Tensor RadialTensorField(Vector3 point)
    {
        float x = center.x - point.x;
        float y = center.y - point.y;


        float t = Mathf.Atan(y / x);

        float diag1 = y * y - x * x;
        float diag2 = -(y * y - x * x);
        float ndiag = -2 * x * y;

        //Debug.Log(diag1 + " " + diag2 + " " + ndiag + " point: " + point);

        float x1, x2;
        Quadratic(1f, -(diag1 * diag2), (diag1 * diag2) - ndiag * ndiag, out x1, out x2);
        //Debug.Log(x1 + " " + x2 + " t: " + t);
        Vector3 major = x1 * new Vector3(Mathf.Cos(t), Mathf.Sin(t));
        Vector3 minor = x2 * new Vector3(Mathf.Cos(t + Mathf.PI / 2), Mathf.Sin(t + Mathf.PI / 2));

        Tensor tensor = new Tensor(major.normalized * 0.2f, minor.normalized * 0.2f);
        return tensor;
    }
    private Tensor GridTensorField(Vector3 direction)
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

        Tensor tensor = new Tensor(major.normalized * 0.2f, minor.normalized * 0.2f);
        return tensor;
    }
    private void Quadratic(float a, float b, float c, out float x1, out float x2)
    {
        float deltaRoot = Mathf.Sqrt(b * b - 4 * a * c);
        x1 = (-b + deltaRoot) / 2 * a;
        x2 = (-b - deltaRoot) / 2 * a;
    }

    private bool Sorient(Vector3 point)
    {
        foreach(VoronoiElement district in GameManager.instance.districts)
        {
            if(district.type == DistrictType.CityCenter || district.type == DistrictType.ResidentialDistrict || district.type == DistrictType.OfficeDistrict)
            {
                if (Sorient(point, district))
                {
                    return true;
                }
            }    
        }
        return false;
    }
    private bool Sorient(Vector3 point, VoronoiElement district)
    {
        if (Orient(point, district.points[0], district.points[1]))
        {
            for (int i = 2; i < district.points.Count; ++i)
            {
                if (!Orient(point, district.points[i - 1], district.points[i]))
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
        if ((point.x - firstPoint.x) * (secondPoint.y - firstPoint.y) - (point.y - firstPoint.y) * (secondPoint.x - firstPoint.x) >= 0)
        {
            return true;
        }
        return false;
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
