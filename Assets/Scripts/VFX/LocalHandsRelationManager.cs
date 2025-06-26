using UnityEngine;
using System;

public class LocalHandsRelationManager : MonoBehaviour
{
    [Header("Hand Joint Transforms")]
    public Transform leftIndexTip;
    public Transform rightIndexTip;
    public static LocalHandsRelationManager Instance { get; private set; }

    public float CurrentDistance { get; private set; }
    public Vector3 CurrentCenterPosition { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Update()
    {
        if (leftIndexTip != null && rightIndexTip != null)
        {
            CurrentDistance = Vector3.Distance(leftIndexTip.position, rightIndexTip.position);
            CurrentCenterPosition = Vector3.Lerp(leftIndexTip.position, rightIndexTip.position, 0.5f);

        }
    }
}