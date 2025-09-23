using System;
using UnityEngine;

public class Graph : MonoBehaviour
{
    [SerializeField] Transform pointPrefab;
    [SerializeField, Range(10,100)] int resolution = 10;
    [SerializeField] FunctionLibrary.FunctionName function;
    [SerializeField, Min(0f)] float functionDuration = 1f;
    Transform [] points;
    float duration;

    private void Awake()
    {
        points = new Transform[resolution * resolution];
        float step = 2f / resolution;
        var scale = Vector3.one * step;
        for (int i = 0, x = 0, z = 0; i < points.Length; i++, x++)
        {
            Transform point = points[i] = Instantiate(pointPrefab);
            point.localScale =  scale;
            point.SetParent(transform, false);
        }
    }

    private void Update()
    {
        duration += Time.deltaTime;
        if (duration >= functionDuration)
        {
            duration -= functionDuration;
            function = FunctionLibrary.GetNextFunctionName(function);
        }
        UpdateFunction();
    }

    void UpdateFunction()
    {
        FunctionLibrary.Function f = FunctionLibrary.GetFunction(function);
        float time = Time.time;
        float step = 2f / resolution;
        float v = 0.5f * step - 1f;
        for (int i = 0, x = 0, z = 0 ; i < points.Length; i++, x++)
        {
            if (x == resolution)
            {
                x = 0;
                z += 1;
                v = (z + 0.5f) * step - 1f;

            }
            float u = (x + 0.5f) * step - 1f;
            points[i].localPosition = f(u, v, time);
        }
    }
}
