using UnityEngine;

public class SynchronizedObject : MonoBehaviour
{
    private Transform worldRootTransform;
    private void Start()
    {
        if (WorldRoot.Instance != null)
        {
            WorldRoot.Instance.OnTransformChanged += AlignWithRoot;
            WorldRoot.Instance.ForceUpdateAlignment();
        }
    }

    private void AlignWithRoot()
    {
        if (WorldRoot.Instance != null)
        {
            
            Vector3 relativePos = worldRootTransform.InverseTransformPoint(transform.position);
            Quaternion relativeRot = Quaternion.Inverse(worldRootTransform.rotation) * transform.rotation;
            this.transform.SetPositionAndRotation(relativePos, relativeRot);
        }
    }

    public void InitRelationWithRoot()
    {
        if (WorldRoot.Instance != null)
            worldRootTransform = WorldRoot.Instance.WorldRootTransform;
        
    }

    private void OnDestroy()
    {
        if (WorldRoot.Instance != null)
        {
            WorldRoot.Instance.OnTransformChanged -= AlignWithRoot;
        }
    }
}