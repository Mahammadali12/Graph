using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [SerializeField] Transform pointPrefab;
    [SerializeField, Range(10,100)] int resolution = 10;
    Transform[] points;
    void Awake()
    {
        points = new Transform[resolution * resolution];
        float step = 2f / resolution;
        var scale = Vector3.one * step;
        Vector3 position = Vector3.back;
        position.x = 0.5f * step - 1f;
        float time = 1000;
        position.y = Mathf.PerlinNoise(0, time);
        for (int i = 0, x = 0, z = 0; i < points.Length; i++, x++)
        {
            Transform point = points[i] = Instantiate(pointPrefab);
            points[i].localScale = scale;
            point.SetParent(transform, false);

            if (x == resolution)
            {
                x = 0;
                z++;
                position.z = z * step - 1f;
                position.y = Mathf.PerlinNoise(x, time);
            }
            time += 0.001f;
            position.x = x * step - 1f;
            points[i].localPosition = position;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
