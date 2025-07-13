using UnityEngine;

public class SliderMovementLimiter : MonoBehaviour
{
    [Header("滑动范围设置")]
    public Transform sliderTrack; // 拖入 SliderTrack 物体
    public float sliderHalfLength = 0.1f; // 滑块自身长度的一半 (0.2 / 2 = 0.1)

    private float minZ;
    private float maxZ;

    void Start()
    {
        if (sliderTrack == null)
        {
            Debug.LogError("请将 SliderTrack 物体拖入到 SliderMovementLimiter 脚本的 sliderTrack 字段中！", this);
            enabled = false;
            return;
        }

        // 计算滑块运动范围的 Z 轴边界
        float trackHalfLength = sliderTrack.localScale.z / 2f;
        float trackCenterZ = sliderTrack.position.z;

        minZ = trackCenterZ - trackHalfLength + sliderHalfLength;
        maxZ = trackCenterZ + trackHalfLength - sliderHalfLength;

        // 确保滑块初始位置在范围内
        Vector3 currentPos = transform.position;
        currentPos.z = Mathf.Clamp(currentPos.z, minZ, maxZ);
        transform.position = currentPos;
    }

    // 提供给外部调用的方法来获取限制后的Z位置
    public float ClampZPosition(float targetZ)
    {
        return Mathf.Clamp(targetZ, minZ, maxZ);
    }
}