using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DebugUI : MonoBehaviour
{
    bool hidden = false;

    [SerializeField]
    GameObject debugUIContainer;

    [SerializeField]
    TMP_Text fpsCounter,
        frameTimeCounter;

    [SerializeField]
    bool startVisible;

    Controls controls;
    InputAction debugShowHideKey,
        debug2DKey,
        debug3DKey;

    void Awake()
    {
        controls = new Controls();
        hidden = startVisible;
        debugUIContainer.SetActive(hidden);
    }

    // setup event handlers
    void OnEnable()
    {
        controls.Enable();
        debugShowHideKey = controls.UI.DebugShowHide;
        debug2DKey = controls.UI.Debug2DScene;
        debug3DKey = controls.UI.Debug3DScene;

        debugShowHideKey.Enable();
        debug2DKey.Enable();
        debug3DKey.Enable();

        debugShowHideKey.performed += OnPressDebugShowHideKey;
        debug2DKey.performed += OnPressDebug2DKey;
        debug3DKey.performed += OnPressDebug3DKey;
    }

    // destroy event handlers
    void OnDisable()
    {
        controls.Disable();
        debugShowHideKey.Disable();
        debug2DKey.Disable();
        debug3DKey.Disable();
    }

    void OnPressDebugShowHideKey(InputAction.CallbackContext context)
    {
        debugUIContainer.SetActive(hidden);
        hidden = !hidden;
    }

    void OnPressDebug2DKey(InputAction.CallbackContext context)
    {
        if (SceneManager.GetActiveScene().name == "2D-demo")
            return;
        SceneManager.LoadScene("2D-demo");
    }

    void OnPressDebug3DKey(InputAction.CallbackContext context)
    {
        if (SceneManager.GetActiveScene().name == "3D-demo")
            return;
        SceneManager.LoadScene("3D-demo");
    }

    void Update()
    {
        if (hidden)
            return;

        var dt = Time.deltaTime;
        frameTimeCounter.text = "Frame Time: " + (dt * 1000).ToString() + " ms";
        fpsCounter.text = "FPS: " + (1.0 / dt).ToString();
    }
}
