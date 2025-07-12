using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUpdater : MonoBehaviour
{
    public ImageTrackingStablizer stablizer;
    public Slider progressBar;
    public Text ProgressValue;

    public Canvas m_Canvas;

    public FaceCamera faceCamera;

    private void Awake()
    {
        if (m_Canvas == null)
        {
            m_Canvas = GetComponent<Canvas>();
            if (!m_Canvas)
            {
                Debug.LogWarning("CanvasEventCameraBinder: No Canvas component found on this GameObject. This script requires a Canvas component.", this);
                enabled = false; // Disable the script if no Canvas is found
                return;
            }
        }

        if (faceCamera == null)
        {
            faceCamera = GetComponent<FaceCamera>();
            
        }

        if (m_Canvas.renderMode != RenderMode.WorldSpace)
        {
            Debug.LogWarning("CanvasEventCameraBinder: Canvas Render Mode is not set to World Space. This script is intended for World Space Canvases.", this);
            enabled = false; // Disable the script if it's not a World Space Canvas
        }
    }

    void OnEnable()
    {
        Camera mainCamera = Camera.main;

        if (mainCamera != null)
        {
            m_Canvas.worldCamera = mainCamera;
            if (faceCamera != null)
            {
                faceCamera.SetCamera(mainCamera.transform);
            }
        }
        else
        {
            Debug.LogWarning("CanvasEventCameraBinder: No Main Camera found in the scene. Make sure your main camera is tagged 'MainCamera'.", this);
        }

        stablizer = FindFirstObjectByType<ImageTrackingStablizer>();
        if (stablizer != null)
        {
            Debug.Log("Find Image Tracking Stablizer");
            stablizer.OnProgressUpdated.AddListener(UpdateProgressBar);
        }
    }

    void OnDisable()
    {
        if (stablizer != null)
        {
            stablizer.OnProgressUpdated.RemoveListener(UpdateProgressBar);
        }
    }

    void UpdateProgressBar(float progress)
    {
        if (progressBar != null)
        {
            progressBar.value = progress;
        }
        ProgressValue.text = $"{Mathf.RoundToInt(progress * 100)}%";
    }
}
