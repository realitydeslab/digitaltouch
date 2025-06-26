using UnityEngine;
using UnityEngine.VFX;

public class NetworkFlameControl : NetworkBaseVFXControl 
{
    public VisualEffect Flame;
    public float baseScale = 0.01f;
    [Tooltip("Sensitivity")]
    public float scaleSensitivity = 0.5f;

    protected override void OnEnable()
    {
        base.OnEnable();
        Flame.enabled = true;
    }

    protected override void OnDisable()
    {
        Flame.enabled = false;
        base.OnDisable();
    }

    public override void OnRelationDataUpdated(float distance, Vector3 centerPosition)
    {
        transform.position = centerPosition;

        float targetScale = baseScale + distance * scaleSensitivity;
        transform.localScale = Vector3.one * targetScale;
    }
}