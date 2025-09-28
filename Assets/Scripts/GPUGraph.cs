using UnityEngine;

public class GPUGraph : MonoBehaviour
{
    [SerializeField] ComputeShader computeShader;
    [SerializeField] Mesh mesh;
    [SerializeField] Material material;
    const int  maxResolution = 1000;
    [SerializeField, Range(10,maxResolution)] int resolution = 10;
    [SerializeField] FunctionLibrary.FunctionName function;
    [SerializeField] FunctionLibrary.TransitionMode transitionMode;
    [SerializeField, Min(0f)] float functionDuration = 1f, transitionDuration = 1f;
    float duration;
    bool transitioning;
    FunctionLibrary.FunctionName transitionFunction;

    private ComputeBuffer positionsBuffer;

    private static readonly int
        positionsId = Shader.PropertyToID("_Positions"),
        resolutionId = Shader.PropertyToID("_Resolution"),
        stepId = Shader.PropertyToID("_Step"),
        timeId = Shader.PropertyToID("_Time"),
        transitionProgessId = Shader.PropertyToID("_TransitionProgress"); 

    private void OnEnable()
    {
        positionsBuffer = new ComputeBuffer(maxResolution * maxResolution, 3 * sizeof(float));
    }
    private void OnDisable()
    {
        positionsBuffer.Release();
        positionsBuffer = null;
    }
    private void Update()
    {
        duration += Time.deltaTime;
        if (transitioning)
        {
            if (duration >= transitionDuration)
            {
                transitioning = false;
                duration -= transitionDuration;
            }
            
        }
        else if (duration >= functionDuration)
        {
            duration -= functionDuration;
            transitioning = true;
            transitionFunction = function;
            PickNextFunction();
        }
        
        UpdateFunctionOnGpu();
    }
    void PickNextFunction()
    {
        function = transitionMode == FunctionLibrary.TransitionMode.Cycle ?
            FunctionLibrary.GetNextFunctionName(function) :
            FunctionLibrary.GetRandomFunctionName(function);
    }
    void UpdateFunctionOnGpu()
    {
        float step = 2f / resolution;
        computeShader.SetInt(resolutionId, resolution);
        computeShader.SetFloat(stepId, step);
        computeShader.SetFloat(timeId, Time.time);
        var kernelIndex = (int)function + (int)(transitioning ? transitionFunction : function)* FunctionLibrary.FunctionCount;
        computeShader.SetBuffer(kernelIndex, positionsId, positionsBuffer);

        if (transitioning)
        {
            computeShader.SetFloat(transitionProgessId, 
                Mathf.SmoothStep(0f,1f,duration/transitionDuration));
        }
        
        int groups = Mathf.CeilToInt(resolution / 8f);
        computeShader.Dispatch(kernelIndex, groups, groups, 1);
        
        material.SetBuffer(positionsId, positionsBuffer);
        material.SetFloat(stepId, step);
        
        var bounds = new Bounds(Vector3.zero, Vector3.one * (2f + 2f/ resolution));
        Graphics.DrawMeshInstancedProcedural(mesh, 0 ,material, bounds, resolution*resolution);
        
    }

}
