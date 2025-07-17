using System;
using UnityEngine;

public abstract class BaseAffordanceControl : MonoBehaviour
{
    public int Index;
    public bool IsEnable = false;
    public float distance = 0f;
    public Vector3 centerPosition = Vector3.zero;
    public bool dataAvailable = false;
    public event Action<int,bool> OnEnableChange;
    protected virtual void Update()
    {
        if (NetworkHandsRelationManager.Instance != null)
        {
            distance = NetworkHandsRelationManager.Instance.networkDistance.Value;
            centerPosition = NetworkHandsRelationManager.Instance.networkCenterPosition.Value;
            dataAvailable = true;
        }
        else if (LocalHandsRelationManager.Instance != null)
        {
            distance = LocalHandsRelationManager.Instance.CurrentDistance;
            centerPosition = LocalHandsRelationManager.Instance.CurrentCenterPosition;
            dataAvailable = true;
        }

        if (dataAvailable)
        {
            OnRelationDataUpdated(distance, centerPosition);
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
        // OnFlameEnableChange?.Invoke(isFlameEnable);
    }

    public abstract void OnRelationDataUpdated(float distance, Vector3 centerPosition);
}
