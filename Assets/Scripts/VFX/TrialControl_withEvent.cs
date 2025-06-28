using UnityEngine;
using UnityEngine.VFX;

public class TrialControl_withEvent : BaseVFXControl_withEvent
{
    public VisualEffect vfx;

    private Transform vfxTransform;

    protected override void OnEnable()
    {
        vfxTransform = vfx.gameObject.transform;
        base.OnEnable();
        vfx.enabled = true;
        Debug.Log("Trial is enabled");

    }

    protected override void OnDisable()
    {
        vfx.enabled = false;
        base.OnDisable();
        Debug.Log("Trial is disabled");

    }

    public override void OnTipsRelationUpdated(float distance, Vector3 centerPosition)
    {
        //Vector3 controlPosition = Vector3.Lerp(vfxTransform.position, centerPosition, 0.5f);
        this.transform.position = centerPosition;
    }
}
