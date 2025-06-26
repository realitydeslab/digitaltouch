using UnityEngine;

/// <summary>
/// 实现VFXControl接口，控制VFX的位置和大小。
/// </summary>
public class FlameControl : MonoBehaviour, IVFXControl
{
    [Tooltip("基础缩放值")]
    public float baseScale = 0.1f;
    [Tooltip("缩放对距离的敏感度")]
    public float scaleSensitivity = 0.1f;

    private void OnEnable()
    {
        Debug.Log("Flame VFX is enabled");
        HandsRelationManager.OnPalmsRelationUpdate += OnPalmsRelationUpdated;
    }

    private void OnDisable()
    {
        HandsRelationManager.OnPalmsRelationUpdate -= OnPalmsRelationUpdated;
    }

    /// <summary>
    /// 这是实现IVFXControl接口的方法。
    /// 它会接收HandsRelationManager广播的数据并作出响应。
    /// </summary>
    /// <param name="distance">双手手掌间的距离。</param>
    /// <param name="centerPosition">双手手掌的中心点。</param>
    public void OnPalmsRelationUpdated(float distance, Vector3 centerPosition)
    {
        transform.position = centerPosition;

        // 2. 根据双手的距离来改变VFX物体的scale
        float targetScale = baseScale + distance * scaleSensitivity;
        transform.localScale = Vector3.one * targetScale;
    }
}