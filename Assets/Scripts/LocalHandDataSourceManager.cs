using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 这个管理器在游戏开始时运行在本地场景中。
/// 它的职责是：
/// 1. 找到本地XR设备追踪的所有手部关节。
/// 2. 为每一个关节创建一个对应的数据源GameObject (例如 "Hand Joint Synchronizer 0", "Hand Joint Synchronizer 1", ...)。
/// 3. 将每个数据源的 m_HandJoint 字段指向真实的本地追踪关节。
/// </summary>
public class LocalHandDataSourceManager : MonoBehaviour
{
    [Header("Local Tracking Targets")]
    [Tooltip("场景中本地手部追踪管理器的根对象名称，例如OVRCameraRig或XR Origin")]
    [SerializeField] private Transform _localLeftHand;
    [SerializeField] private Transform _localRightHand;

    [SerializeField] private Transform _localXRCamera;
    // [SerializeField] private string _localHandManagerName = "Hand Tracking Manager";

    // [Tooltip("包含'LeftHand'和'RightHand'子对象的追踪空间，通常是OVRCameraRig下的TrackingSpace")]
    // [SerializeField] private string _trackingSpaceName = "TrackingSpace";


    [Header("Data Source Prefab")]
    [Tooltip("一个仅挂载了 HandJointSynchronizer 脚本的空预制件")]
    [SerializeField] private GameObject _synchronizerPrefab;
    

    void Start()
    {
        if (_synchronizerPrefab == null)
        {
            Debug.LogError("[LocalHandDataSourceManager] _synchronizerPrefab is not set!", this);
            return;
        }

        SetupHandDataSources();
    }

    private void SetupHandDataSources()
    {
        // 1. 查找本地XR Rig
        // GameObject manager = GameObject.Find(_localHandManagerName);
        // if (manager == null)
        // {
        //     Debug.LogError($"[LocalHandDataSourceManager] 无法找到名为 '{_localHandManagerName}' 的对象！", this);
        //     return;
        // }
        // Transform trackingSpace = manager.transform.Find(_trackingSpaceName);
        // if (trackingSpace == null)
        // {
        //     Debug.LogError($"[LocalHandDataSourceManager] 无法在 '{_localHandManagerName}' 下找到名为 '{_trackingSpaceName}' 的子对象！", this);
        //     return;
        // }

        // 2. 查找左右手根对象
        // Transform localLeftHand = manager.transform.Find("Left Hand");
        // Transform localRightHand = manager.transform.Find("Right Hand");

        if (_localLeftHand == null || _localRightHand == null)
        {
            Debug.LogError($"[LocalHandDataSourceManager] can not find 'Left Hand' or 'Right Hand'", this);
            return;
        }

        GameObject cameraSyncObj = Instantiate(_synchronizerPrefab, this.transform);
        cameraSyncObj.name = "XR Camera Synchronizer";
        if (cameraSyncObj.TryGetComponent<SimpleHandJointSynchronizer>(out SimpleHandJointSynchronizer cameraSynchronizer))
        {
            cameraSynchronizer.m_HandJoint = _localXRCamera;
        }

        // 3. 将所有本地关节收集到一个列表中
        List<Transform> allLocalJoints = new List<Transform>();
        GetAllChildren(allLocalJoints, _localLeftHand);
        GetAllChildren(allLocalJoints, _localRightHand);

        // 4. 为每个本地关节创建数据源
        for (int i = 0; i < allLocalJoints.Count; i++)
        {
            // 实例化预制件
            GameObject syncObject = Instantiate(_synchronizerPrefab, this.transform);
            
            // 命名，以便 NetworkHandJoint 可以找到它
            syncObject.name = $"Hand Joint Synchronizer {i}";
            
            // 获取组件并设置其 m_HandJoint 引用
            if (syncObject.TryGetComponent<SimpleHandJointSynchronizer>(out SimpleHandJointSynchronizer synchronizer))
            {
                synchronizer.m_HandJoint = allLocalJoints[i];
            }
        }
        
        Debug.Log($"[LocalHandDataSourceManager] successfully create {allLocalJoints.Count} hand joint sync.");
    }
    
    /// <summary>
    /// 辅助方法，获取一个父对象下的所有子对象的Transform
    /// </summary>
    private void GetAllChildren(List<Transform> list, Transform parent)
    {
        foreach (Transform child in parent.GetComponentInChildren<Transform>())
        {
            list.Add(child);
            // 如果关节还有子关节，使用递归
            // if(child.childCount > 0) GetAllChildren(list, child);
        }
    }
}