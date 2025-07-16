using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A singleton class to manage the single source of truth for the world's origin in a Colocation XR application.
/// It provides an event to notify other scripts of changes to its transform.
/// </summary>
[DisallowMultipleComponent]
public class WorldRoot : MonoBehaviour
{
    // Singleton instance
    public static WorldRoot Instance { get; private set; }

    // Event fired when the world root's transform changes
    [Tooltip("Event to notify other scripts of changes to the world root's transform.")]
    public event Action OnTransformChanged;

    // The transform of the XR Origin, which will be the child of this WorldRoot
    public Transform WorldRootTransform { get; private set; }
    private Vector3 _lastWorldRootPosition;
    private quaternion _lastWorldRootRotation;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("WorldRoot already exists in the scene. Destroying this one.", this);
            Destroy(this.gameObject);
            return;
        }

        WorldRootTransform = this.transform;
        _lastWorldRootPosition = WorldRootTransform.position;
        _lastWorldRootRotation = WorldRootTransform.rotation;
    }

    private void LateUpdate()
    {
        if (WorldRootTransform != null)
        {
            if (_lastWorldRootPosition == WorldRootTransform.position && _lastWorldRootRotation == WorldRootTransform.rotation)
                return;

            OnTransformChanged?.Invoke();
            Debug.Log("World Root changed.");
            _lastWorldRootPosition = WorldRootTransform.position;
            _lastWorldRootRotation = WorldRootTransform.rotation;
        }
    }

    public void AlignObjectWithRoot(Transform target)
    {
        if (target != null)
        {
            target.position = transform.position;
            target.rotation = transform.rotation;
        }
    }

    public void ForceUpdateAlignment()
    {
        OnTransformChanged?.Invoke();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}