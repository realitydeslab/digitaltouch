using UnityEngine;

/// <summary>
/// VFX控制器的接口，定义了监听HandsRelationManager广播的函数。
/// </summary>
public interface IVFXControl
{
    /// <summary>
    /// 当手掌关系更新时调用的函数。
    /// </summary>
    /// <param name="distance">两个手掌之间的距离。</param>
    /// <param name="centerPosition">两个手掌的中心位置。</param>
    void OnPalmsRelationUpdated(float distance, Vector3 centerPosition);
}
