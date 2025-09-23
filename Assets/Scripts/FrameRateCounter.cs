using System;
using TMPro;
using UnityEngine;

public class FrameRateCounter : MonoBehaviour
{
    [SerializeField]  TextMeshProUGUI display;
    [SerializeField, Range(0.1f, 2f)] float sampleDuration = 1f;
    [SerializeField] DisplayMode displayMode = DisplayMode.FPS;

    public enum DisplayMode
    {
        FPS,
        MS
    }
    int frameCount;
    float duration, bestDuration = float.MaxValue, worstDuration = float.MinValue;
    private void Update()
    {
        float frameDuration = Time.unscaledDeltaTime;
        frameCount++;
        duration += frameDuration;
        
        if (frameDuration < bestDuration) {
            bestDuration = frameDuration;
        }
        if (frameDuration > worstDuration) {
            worstDuration = frameDuration;
        }
        
        if (duration >= sampleDuration)
        {
            if (displayMode == DisplayMode.MS)
            {
                display.SetText("MS\n{0:1}\n{1:1}\n{2:1}",
                    1000f * bestDuration,
                    1000f * duration / frameCount,
                    1000f * worstDuration);
            }
            else
            {
                display.SetText("FPS\n{0:0}\n{1:0}\n{2:0}", 
                    1f/bestDuration,
                    frameCount / duration,
                    1f/worstDuration);
            }
            frameCount = 0;
            duration = 0;
            bestDuration = float.MaxValue;
            worstDuration = float.MinValue;
        }
        
    }
}
