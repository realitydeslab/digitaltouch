using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 在网络环境中同步XR手部。
/// 对于本地玩家，此脚本会找到场景中的本地手部关节，并驱动网络预制件中的对应关节进行跟踪。
/// 对于远程玩家，此脚本不执行任何操作，同步任务由 NetworkHandJoint 脚本自身完成。
/// </summary>
public class HandSynchronizer : NetworkBehaviour
{
    [Header("本地追踪目标设置")]
    [Tooltip("场景中本地手部追踪管理器的根对象名称")]
    [SerializeField] private string _localHandManagerName = "Hand Tracking Manager"; // 根据您的实际情况修改，例如 "XR Origin"

    [Header("网络预制件关节设置")]
    [Tooltip("挂载了左手所有NetworkHandJoint关节的父对象")]
    [SerializeField] private Transform _networkLeftHandParent;
    [Tooltip("挂载了右手所有NetworkHandJoint关节的父对象")]
    [SerializeField] private Transform _networkRightHandParent;

    private List<Transform> _localLeftHandJoints;
    private List<Transform> _localRightHandJoints;

    private List<Transform> _networkLeftHandJoints;
    private List<Transform> _networkRightHandJoints;

    private bool _isInitialized = false;

    public override void OnNetworkSpawn()
    {
        // 此脚本的核心逻辑只在本地玩家（Owner）上运行
        if (!IsOwner)
        {
            return;
        }
        
        // 初始化网络关节列表
        InitializeNetworkJoints();

        // 尝试初始化本地追踪目标
        InitializeLocalTrackingTargets();
    }

    private void Update()
    {
        // 如果不是Owner，或者尚未成功初始化，则不执行任何操作
        if (!IsOwner || !_isInitialized)
        {
            return;
        }

        // 持续将本地关节的 transform 应用于网络关节
        ApplyLocalTransformsToNetworkJoints(_localLeftHandJoints, _networkLeftHandJoints);
        ApplyLocalTransformsToNetworkJoints(_localRightHandJoints, _networkRightHandJoints);
    }

    /// <summary>
    /// 从父对象获取所有网络关节的引用
    /// </summary>
    private void InitializeNetworkJoints()
    {
        _networkLeftHandJoints = new List<Transform>();
        _networkRightHandJoints = new List<Transform>();

        // GameObject xrOrigin = GameObject.Find("XR Origin");
        // if (xrOrigin)
        // {
        //     _networkLeftHandParent = xrOrigin.transform.Find("Left Hand");
        //     _networkRightHandParent = xrOrigin.transform.Find("Right Hand");
        // }
        
        if (_networkLeftHandParent != null)
        {
            foreach (Transform joint in _networkLeftHandParent)
            {
                _networkLeftHandJoints.Add(joint);
            }
        }

        if (_networkRightHandParent != null)
        {
            foreach (Transform joint in _networkRightHandParent)
            {
                _networkRightHandJoints.Add(joint);
            }
        }
    }

    /// <summary>
    /// 在场景中查找本地手部追踪目标并获取所有关节的引用
    /// </summary>
    private void InitializeLocalTrackingTargets()
    {
        GameObject manager = GameObject.Find(_localHandManagerName);
        if (manager == null)
        {
            Debug.LogError($"[HandSynchronizer] cannot find '{_localHandManagerName}' local hand manager！", this);
            return;
        }

        // 查找左右手根对象
        Transform localLeftHand = manager.transform.Find("LeftHand"); // 根据您的实际层级修改
        Transform localRightHand = manager.transform.Find("RightHand"); // 根据您的实际层级修改

        if (localLeftHand == null || localRightHand == null)
        {
            Debug.LogError($"[HandSynchronizer] can not find 'LeftHand' or 'RightHand' in '{_localHandManagerName}' ", this);
            return;
        }
        
        _localLeftHandJoints = new List<Transform>();
        _localRightHandJoints = new List<Transform>();

        // 递归或迭代获取所有子关节
        GetAllChildren(_localLeftHandJoints, localLeftHand);
        GetAllChildren(_localRightHandJoints, localRightHand);

        // 验证关节数量是否匹配
        if (_localLeftHandJoints.Count != _networkLeftHandJoints.Count || 
            _localRightHandJoints.Count != _networkRightHandJoints.Count)
        {
            Debug.LogError("[HandSynchronizer]", this);
            return;
        }

        _isInitialized = true;
        Debug.Log("[HandSynchronizer] local hand tracking succeeded");
    }

    /// <summary>
    /// 辅助方法，获取一个父对象下的所有子对象的Transform
    /// </summary>
    private void GetAllChildren(List<Transform> list, Transform parent)
    {
        foreach (Transform child in parent)
        {
            list.Add(child);
        }
    }

    /// <summary>
    /// 将源关节列表的Transform信息应用到目标关节列表
    /// </summary>
    private void ApplyLocalTransformsToNetworkJoints(List<Transform> sourceJoints, List<Transform> targetJoints)
    {
        if (sourceJoints == null || targetJoints == null || sourceJoints.Count != targetJoints.Count) return;

        for (int i = 0; i < sourceJoints.Count; i++)
        {
            if (sourceJoints[i] != null && targetJoints[i] != null)
            {
                targetJoints[i].position = sourceJoints[i].position;
                targetJoints[i].rotation = sourceJoints[i].rotation;
            }
        }
    }
}
