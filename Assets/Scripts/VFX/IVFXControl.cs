using UnityEngine;

public abstract class BaseVFXControl: MonoBehaviour
{
    protected virtual void OnEnable()
    {
        HandsRelationManager.OnIndexesRelationUpdate += OnPalmsRelationUpdated;
    }

    protected virtual void OnDisable()
    {
        HandsRelationManager.OnIndexesRelationUpdate -= OnPalmsRelationUpdated;
    }

    public abstract void OnPalmsRelationUpdated(float distance, Vector3 centerPosition);
}
