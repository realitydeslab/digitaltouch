using UnityEngine;

public abstract class BaseVFXControl : MonoBehaviour
{
    public float distance = 0f;
    public Vector3 centerPosition = Vector3.zero;
    public bool dataAvailable = false;
    protected virtual void Update()
    {


        if (NetworkHandsRelationManager.Instance != null)
        {
            // Debug.Log("Networking Instance");
            distance = NetworkHandsRelationManager.Instance.networkDistance.Value;
            centerPosition = NetworkHandsRelationManager.Instance.networkCenterPosition.Value;
            dataAvailable = true;
        }
        else if (LocalHandsRelationManager.Instance != null)
        {
            // Debug.Log("Local Instance");
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
