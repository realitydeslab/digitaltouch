using UnityEngine;
using UnityEngine.UI; // 需要引入UI命名空间
using UnityEngine.EventSystems; // 需要引入事件系统命名空间


// 平台特定编译，确保代码只在VisionOS或编辑器中运行
#if UNITY_EDITOR || UNITY_VISIONOS
using UnityEngine.XR.VisionOS.InputDevices;
#endif

namespace UnityEngine.XR.VisionOS.Samples.URP
{
    /// <summary>
    /// 这个脚本负责处理Apple Vision Pro上的空间指针与UI元素的交互。
    /// 它会读取主指针（通常是用户的眼睛或手）发出的射线，
    /// 并当射线悬停（hover）在可交互的UI元素（如Button）上时，触发其高亮状态。
    /// </summary>
    public class VisionProUIHover : MonoBehaviour
    {
        [Header("设置")]
        [Tooltip("射线检测的最大距离。")]
        public float maxRaycastDistance = 100f;

        [Tooltip("摄像机的偏移量或父对象，用于将设备坐标转换到世界坐标。通常是主摄像机本身或其父节点。")]
        [SerializeField]
        Transform m_CameraOffset;

        // 用于存储上一帧悬停的UI元素，以便在指针移开时恢复其状态
        private Selectable lastHoveredSelectable = null;
        
        // 用于调用OnPointerEnter/Exit的事件数据对象
        private PointerEventData pointerEventData;

#if UNITY_EDITOR || UNITY_VISIONOS
        // Unity Input System 的输入动作实例
        private PointerInput m_PointerInput;

        void OnEnable()
        {
            // 初始化并启用输入系统
            m_PointerInput ??= new PointerInput();
            m_PointerInput.Enable();

            // 确保摄像机偏移量已设置
            if (m_CameraOffset == null)
            {
                Debug.LogError("请在Inspector中设置 Camera Offset Transform，通常是场景中的主摄像机。");
                enabled = false;
                return;
            }
        }

        void OnDisable()
        {
            // 组件禁用时，禁用输入系统
            m_PointerInput?.Disable();
        }

        void Start()
        {
            // 初始化 PointerEventData
            // EventSystem.current 是场景中当前活跃的事件系统
            if (EventSystem.current == null)
            {
                Debug.LogError("场景中没有找到 EventSystem。UI交互将无法工作。");
                enabled = false;
                return;
            }
            pointerEventData = new PointerEventData(EventSystem.current);
        }

        void Update()
        {
            // 从输入系统中读取主指针的当前状态
            var primaryPointer = m_PointerInput.Default.PrimaryPointer.ReadValue<VisionOSSpatialPointerState>();

            // 检查指针当前是否处于活动状态（刚开始或正在移动）
            var phase = primaryPointer.phase;
            bool isPointerActive = (phase == VisionOSSpatialPointerPhase.Began || phase == VisionOSSpatialPointerPhase.Moved);

            if (isPointerActive)
            {
                // 如果指针是活动的，执行射线检测
                PerformRaycast(primaryPointer);
            }
            else
            {
                // 如果指针不活动（例如Ended, Cancelled, or None），则确保取消任何已有的高亮
                HandleExit();
            }
        }

        /// <summary>
        /// 根据指针状态执行射线检测并处理UI悬停逻辑。
        /// </summary>
        private void PerformRaycast(VisionOSSpatialPointerState pointerState)
        {
            // 1. 获取射线的起点和方向，并从设备空间转换到世界空间
            // 注意：我们使用 startRay... 属性，假设它们在指针活动时会持续更新
            var rayOrigin = m_CameraOffset.TransformPoint(pointerState.startRayOrigin);
            var rayDirection = m_CameraOffset.TransformDirection(pointerState.startRayDirection);
            var ray = new Ray(rayOrigin, rayDirection);

            // 2. 执行射线检测
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, maxRaycastDistance))
            {
                // 射线击中了某个物体
                // 尝试从击中的物体上获取 Selectable 组件 (Button, Toggle, Slider等都继承自Selectable)
                Selectable currentSelectable = hit.collider.GetComponent<Selectable>();

                if (currentSelectable != null && currentSelectable.interactable)
                {
                    // 如果击中的是可交互的UI元素
                    if (currentSelectable != lastHoveredSelectable)
                    {
                        // 是一个新的UI元素，先让上一个退出高亮
                        HandleExit();

                        // 触发新元素的 OnPointerEnter 事件，使其进入高亮状态
                        currentSelectable.OnPointerEnter(pointerEventData);
                        lastHoveredSelectable = currentSelectable;
                    }
                }
                else
                {
                    // 击中了非UI物体（如墙壁），取消高亮
                    HandleExit();
                }
            }
            else
            {
                // 射线没有击中任何物体，取消高亮
                HandleExit();
            }
        }

        /// <summary>
        /// 处理指针移出UI元素的情况，使其恢复正常状态。
        /// </summary>
        private void HandleExit()
        {
            if (lastHoveredSelectable != null)
            {
                // 触发 OnPointerExit 事件
                lastHoveredSelectable.OnPointerExit(pointerEventData);
                // 清空记录
                lastHoveredSelectable = null;
            }
        }
#endif
    }
}