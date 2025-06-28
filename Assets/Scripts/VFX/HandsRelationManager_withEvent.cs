using UnityEngine;
using System;

public class HandsRelationManager : MonoBehaviour
{
    [Header("Left Hand Joint Transforms")]
    public Transform leftHandPalm;
    public Transform leftHandWrist;
    public Transform leftThumbTip;
    public Transform leftIndexTip;
    public Transform leftMiddleTip;
    public Transform leftRingTip;
    public Transform leftLittleTip;

    [Header("Right Hand Joint Transforms")]
    public Transform rightHandPalm;
    public Transform rightHandWrist;
    public Transform rightThumbTip;
    public Transform rightIndexTip;
    public Transform rightMiddleTip;
    public Transform rightRingTip;
    public Transform rightLittleTip;

    public static event Action<float, Vector3> OnIndexesRelationUpdate;

    [SerializeField]
    private float _distance;
    public float Distance
    {
        get { return _distance; }
        private set { _distance = value; } // 私有 set
    }

    private void Update()
    {
        if (leftIndexTip != null && rightIndexTip != null)
        {
            Distance = Vector3.Distance(leftIndexTip.position, rightIndexTip.position);

            Vector3 centerPosition = Vector3.Lerp(leftIndexTip.position, rightIndexTip.position, 0.5f);

            OnIndexesRelationUpdate?.Invoke(Distance, centerPosition);

            // for test
            // if (OnIndexesRelationUpdate != null)
            // {
            //     Debug.Log($"distance: {Distance}");
            // }
        }
    }
}