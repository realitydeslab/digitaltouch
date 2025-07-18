using UnityEngine;
using UnityEngine.VFX;

public class VertexControl : BaseAffordanceControl
{
    public VisualEffect Vertex;
    private VFXEventAttribute eventAttribute;

    // private string positionAttribute = "Position";
    // private string enteredTriggerEvent = "EnteredTrigger";

    public float baseScale = 0.01f;
    [Tooltip("Sensitivity")]
    public float scaleSensitivity = 1f;

    protected override void OnEnable()
    {
        base.OnEnable();
        Vertex.enabled = true;
        Debug.Log("Vertex is disabled");
    }

    protected override void OnDisable()
    {
        Vertex.enabled = false;
        base.OnDisable();
        Debug.Log("Vertex is disabled");

    }

    public override void OnTipRelationDataUpdated(float distance, Vector3 centerPosition)
    {
        Vertex.SetFloat("TrailLifetime", distance);
        Vertex.SetFloat("Spawn Rate", distance);

    }
    public override void OnCameraRelationDataUpdated(float distance, Vector3 centerPosition)
    {
        return;
    }
}