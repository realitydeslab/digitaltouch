using UnityEngine;

public abstract class NetworkBaseVFXControl : MonoBehaviour
{
    protected virtual void Update()
    {
        if (NetworkHandsRelationManager.Instance != null)
        {
            float currentDistance = NetworkHandsRelationManager.Instance.networkDistance.Value;
            Vector3 currentCenter = NetworkHandsRelationManager.Instance.networkCenterPosition.Value;

            OnRelationDataUpdated(currentDistance, currentCenter);
        }
    }

    protected virtual void OnEnable()
    {
        
    }

    protected virtual void OnDisable()
    {
        
    }

    public abstract void OnRelationDataUpdated(float distance, Vector3 centerPosition);
}
