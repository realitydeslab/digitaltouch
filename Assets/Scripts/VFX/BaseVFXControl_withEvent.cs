using UnityEngine;

public abstract class BaseVFXControl_withEvent: MonoBehaviour
{
    protected virtual void OnEnable()
    {
        HandsRelationManager.OnIndexesRelationUpdate += OnTipsRelationUpdated;
    }

    protected virtual void OnDisable()
    {
        HandsRelationManager.OnIndexesRelationUpdate -= OnTipsRelationUpdated;
    }

    public abstract void OnTipsRelationUpdated(float distance, Vector3 centerPosition);
}
