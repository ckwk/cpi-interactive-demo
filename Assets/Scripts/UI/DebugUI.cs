using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugUI : MonoBehaviour
{
    [SerializeField]
    TMP_Text fpsCounter,
        frameTimeCounter;

    // Update is called once per frame
    void Update()
    {
        var dtInt = Time.deltaTime;
        frameTimeCounter.text = "Frame Time: " + (dtInt * 1000).ToString() + " ms";
        fpsCounter.text = "FPS: " + (1.0 / dtInt).ToString();
    }
}
