using UnityEngine;
using System;

/// <summary>
/// 管理左右手关键节点的Transform，并计算和广播双手手掌的关系数据。
/// </summary>
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

    /// <summary>
    /// 广播事件，发送两个手掌之间的距离和中心位置。
    /// 参数1: 距离 (float)
    /// 参数2: 中心位置 (Vector3)
    /// </summary>
    public static event Action<float, Vector3> OnPalmsRelationUpdate;

    private void Update()
    {
        // 确保左右手的Palm Transform都已经被赋值
        if (leftHandPalm != null && rightHandPalm != null)
        {
            // 1. 计算两个手掌之间的距离
            float distance = Vector3.Distance(leftHandPalm.position, rightHandPalm.position);

            // 2. 计算两个手掌之间的中心位置
            Vector3 centerPosition = Vector3.Lerp(leftHandPalm.position, rightHandPalm.position, 0.5f);

            // 3. 广播数据
            // 使用 ?.Invoke() 来安全地调用事件，只有当有订阅者时才会触发
            OnPalmsRelationUpdate?.Invoke(distance, centerPosition);
        }
    }
}