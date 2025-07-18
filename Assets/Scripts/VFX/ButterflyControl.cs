using UnityEngine;
using UnityEngine.VFX;

public class ButterflyControl : BaseAffordanceControl
{
    public VisualEffect VFX;
    public Transform ButterfliesControl;

    protected override void OnEnable()
    {
        base.OnEnable();
        VFX.enabled = true;
        Debug.Log("Butterfly is disabled");
    }

    protected override void OnDisable()
    {
        VFX.enabled = false;
        base.OnDisable();
        Debug.Log("Butterfly is disabled");

    }

    public override void OnTipRelationDataUpdated(float distance, Vector3 centerPosition)
    {
        ButterfliesControl.transform.position = centerPosition;
    }
    public override void OnCameraRelationDataUpdated(float distance, Vector3 centerPosition)
    {
        return;
    }
}