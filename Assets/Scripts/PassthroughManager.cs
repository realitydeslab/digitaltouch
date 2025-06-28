using UnityEngine;
using System.Collections; 

public class PassthroughManager : MonoBehaviour
{
    [Tooltip("Main Camera")]
    [SerializeField] private Camera m_Camera;

    [SerializeField] private float fadeDuration = 1.0f;
    [SerializeField] private Color backgroundColor = Color.black;

    private bool isTransparent = true; 
    private Coroutine activeFadeCoroutine;

    private void Start()
    {
        m_Camera.clearFlags = CameraClearFlags.Color;
        m_Camera.backgroundColor = Color.clear;
        isTransparent = true;
    }

    public void TogglePassthrough() 
    {
        isTransparent = !isTransparent;

        Color targetColor = isTransparent ? Color.clear : backgroundColor;

        if (activeFadeCoroutine != null)
        {
            StopCoroutine(activeFadeCoroutine);
        }

        Debug.Log($"Toggle the passthrough to be {isTransparent}");
        activeFadeCoroutine = StartCoroutine(FadeColorCoroutine(targetColor, fadeDuration));
    }

    private IEnumerator FadeColorCoroutine(Color targetColor, float duration)
    {
        Color startingColor = m_Camera.backgroundColor;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            m_Camera.backgroundColor = Color.Lerp(startingColor, targetColor, t);
            yield return null;
        }

        m_Camera.backgroundColor = targetColor;
        activeFadeCoroutine = null; 
    }
}

