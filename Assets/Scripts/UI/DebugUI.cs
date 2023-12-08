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
        var dt = Time.deltaTime;
        frameTimeCounter.text = "Frame Time: " + (dt * 1000).ToString() + " ms";
        fpsCounter.text = "FPS: " + (1.0 / dt).ToString();
    }
}
