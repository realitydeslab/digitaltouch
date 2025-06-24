using UnityEngine;

public class PassthroughManager : MonoBehaviour
{
    public Camera m_Camera;
    public void ToggleSkybox()
    {
        if (m_Camera.clearFlags == CameraClearFlags.Skybox)
        {
            m_Camera.clearFlags = CameraClearFlags.Color;
            m_Camera.backgroundColor = Color.clear;
        }
        else
        {
            m_Camera.clearFlags = CameraClearFlags.Skybox;
        }

    }
}
