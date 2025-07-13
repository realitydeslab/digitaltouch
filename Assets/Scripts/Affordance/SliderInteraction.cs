using UnityEngine;
using System.Collections.Generic;

public class SliderInteraction : MonoBehaviour
{
    private Renderer sliderRenderer;
    private Material originalMaterial;
    public Material grayMaterial;
    public Material greenMaterial;

    private List<GameObject> contactingDragCubes = new List<GameObject>();
    private Rigidbody sliderRb; // 获取 SliderCube 的 Rigidbody

    private Vector3 initialSliderToDragMidpointOffset; // SliderCube 中心到双 DragCube 中心点的初始向量偏移

    // 用于施加排斥力，将 Slider 推出 Track 边界
    public float repulsionForceMagnitude = 50f; // 排斥力大小，根据需要调整

    void Start()
    {
        sliderRenderer = GetComponent<Renderer>();
        sliderRb = GetComponent<Rigidbody>();

        if (sliderRenderer == null || sliderRb == null)
        {
            Debug.LogError("SliderCube 需要 Renderer 和 Rigidbody 组件！", this);
            enabled = false;
        }

        originalMaterial = sliderRenderer.material;
        if (grayMaterial == null || greenMaterial == null)
        {
            Debug.LogError("请为 SliderInteraction 脚本指定 Gray Material 和 Green Material！", this);
            enabled = false;
        }

        SetSliderMaterial(originalMaterial);
    }

    void FixedUpdate()
    {
        // 移除列表中已经被销毁或不再激活的物体 (安全检查)
        contactingDragCubes.RemoveAll(item => item == null || !item.activeInHierarchy);

        UpdateSliderMaterial(); // 根据接触的 DragCube 数量更新材质

        if (contactingDragCubes.Count == 2)
        {
            Vector3 dragCube1Pos = contactingDragCubes[0].transform.position;
            Vector3 dragCube2Pos = contactingDragCubes[1].transform.position;
            Vector3 currentDragMidpoint = (dragCube1Pos + dragCube2Pos) / 2f;

            // 如果是刚刚进入拖拽状态 (从非拖拽到拖拽)
            if (initialSliderToDragMidpointOffset == Vector3.zero) // 初始偏移量为零表示刚刚开始拖拽
            {
                initialSliderToDragMidpointOffset = transform.position - currentDragMidpoint;
            }

            // 计算目标位置：基于当前拖拽中心点加上初始偏移
            Vector3 targetPosition = currentDragMidpoint + initialSliderToDragMidpointOffset;

            // 使用 Rigidbody.MovePosition 进行平滑物理移动
            // 为了防止穿透和保持物理交互，我们不再直接设置位置，而是施加力或速度
            // 但对于拖拽，直接设置目标位置更直观，所以这里用MovePosition
            sliderRb.MovePosition(targetPosition);
        }
        else
        {
            // 如果不足两个 DragCube 接触，重置拖拽偏移
            initialSliderToDragMidpointOffset = Vector3.zero;
        }
    }

    // 当 SliderCube 与 SliderTrack 碰撞时，施加一个推力将其推回
    void OnCollisionStay(Collision collision)
    {
        // 检查碰撞对象是否是 SliderTrack
        if (collision.gameObject.CompareTag("SliderTrack")) // 确保 SliderTrack 也有一个 Tag
        {
            // 获取碰撞点法线，这表示 SliderCube 试图离开 Track 的方向
            // 通常碰撞法线指向被碰撞物体的外部
            Vector3 collisionNormal = Vector3.zero;
            if (collision.contacts.Length > 0)
            {
                // 可以取多个接触点的平均法线，或者只是第一个
                collisionNormal = collision.contacts[0].normal;
            }

            // 对 SliderCube 施加一个与法线反方向的力，将其推回 Track 内部
            // 如果法线指向 SliderCube 外部，那么 -collisionNormal 指向内部
            // 注意：这需要在 SliderTrack 的 Mesh Collider 的法线指向内部才能正确工作
            // 或者：简单地把力方向反过来 `collisionNormal`
            sliderRb.AddForce(collisionNormal * repulsionForceMagnitude, ForceMode.Force);
        }
    }


    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("DragCube") && !contactingDragCubes.Contains(collision.gameObject))
        {
            contactingDragCubes.Add(collision.gameObject);
        }
    }

    void OnCollisionExit(Collision collision)
    {
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
                targetMaterial = greenMaterial;
                break;
            case 1:
                targetMaterial = grayMaterial;
                break;
            default:
                targetMaterial = originalMaterial;
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