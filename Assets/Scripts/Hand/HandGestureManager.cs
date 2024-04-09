using UnityEngine;
using UnityEngine.XR.Hands;
using UnityEngine.Events;

public class HandGestureManager : MonoBehaviour
{
    [SerializeField] private HandTrackingManager m_HandTrackingManager;

    public bool IsLeftHandFisting => m_IsLeftHandFisting;

    public bool IsRightHandFisting => m_IsRightHandFisting;

    private bool m_IsLeftHandFisting;

    private bool m_IsRightHandFisting;

    private const float FINGER_MAX_DIST = 0.15f;

    public UnityEvent<Handedness> OnHandFisted;

    public UnityEvent<Handedness> OnHandUnfisted;

    public void OnTrackingAcquired(Handedness handedness)
    {
        
    }

    public void OnTrackingLost(Handedness handedness)
    {
        if (handedness == Handedness.Left && m_IsLeftHandFisting)
        {
            m_IsLeftHandFisting = false;
            OnHandUnfisted?.Invoke(Handedness.Left);
        }
        else if (handedness == Handedness.Right && m_IsRightHandFisting)
        {
            m_IsRightHandFisting = false;
            OnHandUnfisted?.Invoke(Handedness.Right);
        }
    }

    public void OnUpdatedHand(Handedness handedness)
    {
        var hand = handedness == Handedness.Left ? m_HandTrackingManager.LeftHand : m_HandTrackingManager.RightHand;
        bool isHandFisting = ValidateHandFisting(hand);

        if (handedness == Handedness.Left && m_IsLeftHandFisting != isHandFisting)
        {
            m_IsLeftHandFisting = isHandFisting;
            if (m_IsLeftHandFisting)
                OnHandFisted?.Invoke(Handedness.Left);
            else
                OnHandUnfisted?.Invoke(Handedness.Left);
        }
        else if (handedness == Handedness.Right && m_IsRightHandFisting != isHandFisting)
        {
            m_IsRightHandFisting = isHandFisting;
            if (m_IsRightHandFisting)
                OnHandFisted?.Invoke(Handedness.Right);
            else
                OnHandUnfisted?.Invoke(Handedness.Right);
        }
    }

    private bool ValidateHandFisting(Hand hand)
    {
        var wrist = hand.GetHandJointPose(XRHandJointID.Wrist);
        var thumbTip = hand.GetHandJointPose(XRHandJointID.ThumbTip);
        var indexTip = hand.GetHandJointPose(XRHandJointID.IndexTip);
        var middleTip = hand.GetHandJointPose(XRHandJointID.MiddleTip);
        var ringTip = hand.GetHandJointPose(XRHandJointID.RingTip);
        var littleTip = hand.GetHandJointPose(XRHandJointID.LittleTip);

        var thumbDist = Vector3.Distance(wrist.position, thumbTip.position);
        var indexDist = Vector3.Distance(wrist.position, indexTip.position);
        var middleDist = Vector3.Distance(wrist.position, middleTip.position);
        var ringDist = Vector3.Distance(wrist.position, ringTip.position);
        var littleDist = Vector3.Distance(wrist.position, littleTip.position);

        return thumbDist < FINGER_MAX_DIST && indexDist < FINGER_MAX_DIST && middleDist < FINGER_MAX_DIST && ringDist < FINGER_MAX_DIST && littleDist < FINGER_MAX_DIST;
    }
}
