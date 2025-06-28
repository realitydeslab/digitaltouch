using UnityEngine;
using UnityEngine.VFX;

public class FlameControl_withEvent : BaseVFXControl_withEvent
{
    public VisualEffect vfx;

    public float baseScale = 0.01f;
    [Tooltip("sensitivity")]
    public float scaleSensitivity = 0.5f;

    protected override void OnEnable()
    {
        base.OnEnable();
        vfx.enabled = true;
        Debug.Log("Flame is enabled");

    }

    protected override void OnDisable()
    {
        vfx.enabled = false;
        base.OnDisable();
        Debug.Log("Flame is disabled");

    }

    public override void OnTipsRelationUpdated(float distance, Vector3 centerPosition)
    {
        transform.position = centerPosition;

        float targetScale = baseScale + distance * scaleSensitivity;
        transform.localScale = Vector3.one * targetScale;
    }
}