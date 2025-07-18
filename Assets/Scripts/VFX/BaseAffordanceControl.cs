using System;
using UnityEngine;

public abstract class BaseAffordanceControl : MonoBehaviour
{
    public int Index;
    public bool IsEnable = false;
    public float xrCameraDistance = 0f;
    public Vector3 xrCameraCenterPosition = Vector3.zero;
    public float tipDistance = 0f;
    public Vector3 tipCenterPosition = Vector3.zero;
    public bool dataAvailable = false;
    public event Action<int, bool> OnEnableChange;

    protected virtual void Start()
    {
        this.enabled = IsEnable;
    }
    protected virtual void Update()
    {
        if (NetworkHandsRelationManager.Instance != null)
        {
            xrCameraDistance = NetworkHandsRelationManager.Instance.networkCameraDistance.Value;
            xrCameraCenterPosition = NetworkHandsRelationManager.Instance.networkCameraCenterPosition.Value;
            tipDistance = NetworkHandsRelationManager.Instance.networkIndexDistance.Value;
            tipCenterPosition = NetworkHandsRelationManager.Instance.networkIndexCenterPosition.Value;
            dataAvailable = true;
        }
        else if (LocalHandsRelationManager.Instance != null)
        {
            tipDistance = LocalHandsRelationManager.Instance.CurrentDistance;
            tipCenterPosition = LocalHandsRelationManager.Instance.CurrentCenterPosition;
            dataAvailable = true;
        }

        if (dataAvailable)
        {
            OnCameraRelationDataUpdated(xrCameraDistance, xrCameraCenterPosition);
            OnTipRelationDataUpdated(tipDistance, tipCenterPosition);
        }
    }

    protected virtual void OnEnable()
    {

    }

    protected virtual void OnDisable()
    {

    }

    public void ToggleEnable()
    {
        IsEnable = !IsEnable;
        this.enabled = IsEnable;
        OnEnableChange?.Invoke(Index, this.enabled);
        Debug.Log("Toggle to " + this.enabled);
    }

    public void SetEnable(bool state)
    {
        if (IsEnable == state)
        {
            return;
        }
        else
        {
            IsEnable = state;
            this.enabled = IsEnable;
        }
    }

    public abstract void OnCameraRelationDataUpdated(float distance, Vector3 centerPosition);
    public abstract void OnTipRelationDataUpdated(float distance, Vector3 centerPosition);

    
}
