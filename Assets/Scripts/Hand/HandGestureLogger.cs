using UnityEngine;
using UnityEngine.XR.Hands;

public class HandGestureLogger : MonoBehaviour
{
    public void OnHandFisted(Handedness handedness)
    {
        Debug.Log($"OnHandFisted: {handedness}");
    }

    public void OnHandUnfisted(Handedness handedness)
    {
        Debug.Log($"OnHandUnfisted: {handedness}");
    }
}
