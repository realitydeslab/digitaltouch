using UnityEngine;
using System.Collections.Generic; // 引入 List

public class SliderInteraction : MonoBehaviour
{
    private Renderer sliderRenderer;
    private Material originalMaterial;
    public Material grayMaterial;   // 拖入灰色材质
    public Material greenMaterial;  // 拖入绿色材质

    private List<GameObject> contactingDragCubes = new List<GameObject>(); // 存储当前接触的 DragCube
    private SliderMovementLimiter sliderLimiter; // 用于获取和应用位置限制

    private Vector3 initialDragMidpoint; // 拖拽开始时两个DragCube的中心点
    private float initialSliderZOffset; // 拖拽开始时SliderCube与DragCube中心点的Z轴偏移

    void Start()
    {
        sliderRenderer = GetComponent<Renderer>();
        if (sliderRenderer != null)
        {
            originalMaterial = sliderRenderer.material; // 保存原始材质
        }
        else
        {
            Debug.LogError("SliderCube 没有 Renderer 组件！", this);
            enabled = false;
        }

        sliderLimiter = GetComponent<SliderMovementLimiter>();
        if (sliderLimiter == null)
        {
            Debug.LogError("SliderCube 需要 SliderMovementLimiter 组件！", this);
            enabled = false;
        }

        if (grayMaterial == null || greenMaterial == null)
        {
            Debug.LogError("请为 SliderInteraction 脚本指定 Gray Material 和 Green Material！", this);
            enabled = false;
        }

        // 初始状态设置为原始材质
        SetSliderMaterial(originalMaterial);
    }

    void Update() // 使用 FixedUpdate 处理物理和移动
    {
        // 移除列表中已经被销毁或不再激活的物体 (安全检查)
        contactingDragCubes.RemoveAll(item => item == null || !item.activeInHierarchy);

        // 根据接触的 DragCube 数量更新材质
        UpdateSliderMaterial();

        // 仅当有两个 DragCube 接触时才移动滑块
        if (contactingDragCubes.Count == 2)
        {
            // 获取两个 DragCube 的当前位置
            Vector3 dragCube1Pos = contactingDragCubes[0].transform.position;
            Vector3 dragCube2Pos = contactingDragCubes[1].transform.position;

            // 计算它们的中心点
            Vector3 currentDragMidpoint = (dragCube1Pos + dragCube2Pos) / 2f;

            // 如果是刚刚进入拖拽状态，记录初始偏移
            if (sliderRenderer.material == greenMaterial && initialSliderZOffset == 0 && initialDragMidpoint == Vector3.zero)
            {
                initialDragMidpoint = currentDragMidpoint;
                initialSliderZOffset = transform.position.z - initialDragMidpoint.z;
            }

            // 计算目标 Z 轴位置：基于当前拖拽中心点加上初始偏移
            float targetZ = currentDragMidpoint.z + initialSliderZOffset;

            // 使用 SliderMovementLimiter 限制目标 Z 位置
            float clampedZ = sliderLimiter.ClampZPosition(targetZ);

            // 移动 SliderCube
            Vector3 newPosition = transform.position;
            newPosition.z = clampedZ;

            // 使用 Rigidbody.MovePosition 进行平滑物理移动
            GetComponent<Rigidbody>().MovePosition(newPosition);
        }
        else
        {
            // 如果不足两个 DragCube 接触，重置拖拽偏移
            initialSliderZOffset = 0;
            initialDragMidpoint = Vector3.zero;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // 检查碰撞对象是否是 DragCube 且尚未在列表中
        if (collision.gameObject.CompareTag("DragCube") && !contactingDragCubes.Contains(collision.gameObject))
        {
            contactingDragCubes.Add(collision.gameObject);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        // 检查离开的碰撞对象是否是 DragCube
        if (collision.gameObject.CompareTag("DragCube") && contactingDragCubes.Contains(collision.gameObject))
        {
            contactingDragCubes.Remove(collision.gameObject);
        }
    }

    private void UpdateSliderMaterial()
    {
        Material targetMaterial;
        switch (contactingDragCubes.Count)
        {
            case 2:
                targetMaterial = greenMaterial; // 两个接触，可拖拽
                break;
            case 1:
                targetMaterial = grayMaterial;  // 一个接触，灰色
                break;
            default:
                targetMaterial = originalMaterial; // 零个或更多 (理论上不会超过2)，恢复原始色
                break;
        }

        SetSliderMaterial(targetMaterial);
    }

    private void SetSliderMaterial(Material mat)
    {
        if (sliderRenderer.material != mat)
        {
            sliderRenderer.material = mat;
        }
    }
}