using UnityEngine;
using UnityEngine.VFX;

public class FlameControl : BaseAffordanceControl
{
    public VisualEffect Flame;
    public float baseScale = 0.01f;
    [Tooltip("Sensitivity")]
    public float scaleSensitivity = 0.5f;

    protected override void OnEnable()
    {
        base.OnEnable();
        Flame.enabled = true;
        Debug.Log("Flame is disabled");
    }

    protected override void OnDisable()
    {
        Flame.enabled = false;
        base.OnDisable();
        Debug.Log("Flame is disabled");

    }

    public override void OnTipRelationDataUpdated(float distance, Vector3 centerPosition)
    {
        transform.position = centerPosition;

        float targetScale = baseScale + distance * scaleSensitivity;
        transform.localScale = Vector3.one * targetScale;
    }
    public override void OnCameraRelationDataUpdated(float distance, Vector3 centerPosition)
    {
        return;
    }
}