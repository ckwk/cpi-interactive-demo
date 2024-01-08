using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Color clearFog,
        hazyFog;
    public float minHaze,
        maxHaze;

    float targetHaze;

    void Start()
    {
        targetHaze = maxHaze;
    }

    void Update()
    {
        var dt = Time.deltaTime;
        UpdateFog(dt);
    }

    void UpdateFog(float deltaTime)
    {
        var currentHaze = RenderSettings.fogDensity;
        if (targetHaze == maxHaze)
        {
            RenderSettings.fogDensity =
                currentHaze < maxHaze ? currentHaze + (0.001f * deltaTime) : currentHaze;
            targetHaze = currentHaze < maxHaze ? maxHaze : minHaze;
            return;
        }

        RenderSettings.fogDensity =
            currentHaze > minHaze ? currentHaze - (0.001f * deltaTime) : currentHaze;
        targetHaze = currentHaze > minHaze ? minHaze : maxHaze;
    }
}
