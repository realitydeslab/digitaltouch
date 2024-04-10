using UnityEngine;
using UnityEngine.XR.Hands;

public class FingerSyncTrigger : MonoBehaviour
{
    private bool m_IsSyncing = false;

    private bool m_IsFisting = false;

    private float m_FistingTime;

    private const float FIST_THRESHOLD = 1f;

    public void OnHandGestureChanged(Handedness handedness, HandGesture oldGesture, HandGesture newGesture)
    {
        if (newGesture == HandGesture.Fisting) // Start finger sync
        {
            m_IsFisting = true;
            m_FistingTime = Time.time;
            return;
        }

        if (oldGesture == HandGesture.Fisting) // Stop finger sync
        {
            if (m_IsSyncing)
                StopFingerSync();
            return;
        }
    }

    private void Update()
    {
        if (!m_IsSyncing && m_IsFisting)
        {
            if (Time.time - m_FistingTime > FIST_THRESHOLD)
                StartFingerSync();
        }
    }

    private void StartFingerSync()
    {
        m_IsSyncing = true;
        Debug.Log("Start finger sync");
    }

    private void StopFingerSync()
    {
        m_IsFisting = false;
        m_IsSyncing = false;
        Debug.Log($"Stop finger sync");
    }
}
