using UnityEngine;
using UnityEngine.VFX;

public class FlameControl : BaseVFXControl
{
    public VisualEffect vfx;

    public float baseScale = 0.1f;
    [Tooltip("sensitivity")]
    public float scaleSensitivity = 0.1f;

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

    public override void OnPalmsRelationUpdated(float distance, Vector3 centerPosition)
    {
        transform.position = centerPosition;

        float targetScale = baseScale + distance * scaleSensitivity;
        transform.localScale = Vector3.one * targetScale;
    }
}