using UnityEngine;

public abstract class BaseVFXControl : MonoBehaviour
{
    protected virtual void Update()
    {
        float distance = 0f;
        Vector3 centerPosition = Vector3.zero;
        bool dataAvailable = false;

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

    public abstract void OnRelationDataUpdated(float distance, Vector3 centerPosition);
}
