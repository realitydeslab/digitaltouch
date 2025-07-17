using UnityEngine;

public class SynchronizedObject : MonoBehaviour
{
    public Vector3 relativePos;
    public Quaternion relativeRot;
    private void Start()
    {
        if (WorldRoot.Instance != null)
        {
            Vector3 rootPositionOrigin = WorldRoot.Instance.WorldRootTransform.position;
            Quaternion rootRotationOrigin = WorldRoot.Instance.WorldRootTransform.rotation;
            relativePos = this.transform.position - rootPositionOrigin;
            relativeRot = Quaternion.Inverse(rootRotationOrigin) * this.transform.rotation;

            WorldRoot.Instance.OnTransformChanged += AlignWithRoot;
            WorldRoot.Instance.ForceUpdateAlignment();
            Debug.Log("Has instance");
        }
    }

    private void AlignWithRoot()
    {
        if (WorldRoot.Instance != null)
        {
            Debug.Log("Align with world root");
            
            //this.transform.SetPositionAndRotation(relativePos, relativeRot);
            transform.position = WorldRoot.Instance.WorldRootTransform.position + relativePos;
            transform.rotation = WorldRoot.Instance.WorldRootTransform.rotation * relativeRot;
        }
    }

    private void OnDestroy()
    {
        if (WorldRoot.Instance != null)
        {
            WorldRoot.Instance.OnTransformChanged -= AlignWithRoot;
        }
    }
}