using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

/// <summary>
/// 用于调试手部网络同步状态的工具
/// </summary>
public class NetworkHandSyncDebugger : NetworkBehaviour
{
    [Header("Debug UI")]
    [SerializeField] private bool showDebugInfo = true;
    [SerializeField] private float debugUpdateInterval = 1f;
    
    private float lastDebugUpdate = 0f;
    private Dictionary<ulong, int> clientHandJointCounts = new Dictionary<ulong, int>();

    private void Update()
    {
        if (showDebugInfo && Time.time - lastDebugUpdate > debugUpdateInterval)
        {
            UpdateDebugInfo();
            lastDebugUpdate = Time.time;
        }
    }

    private void UpdateDebugInfo()
    {
        if (!IsSpawned) return;

        // 清空计数
        clientHandJointCounts.Clear();

        // 统计每个客户端的手部关节数量
        NetworkHandJoint[] allNetworkHandJoints = FindObjectsOfType<NetworkHandJoint>();
        
        foreach (var handJoint in allNetworkHandJoints)
        {
            ulong ownerId = handJoint.OwnerClientId;
            if (!clientHandJointCounts.ContainsKey(ownerId))
            {
                clientHandJointCounts[ownerId] = 0;
            }
            clientHandJointCounts[ownerId]++;
        }

        // 输出调试信息
        string debugInfo = $"[NetworkHandSyncDebugger] Network Hand Joints Status:\n";
        debugInfo += $"Total NetworkHandJoint objects: {allNetworkHandJoints.Length}\n";
        
        foreach (var kvp in clientHandJointCounts)
        {
            debugInfo += $"Client {kvp.Key}: {kvp.Value} hand joints\n";
        }
        
        // 检查 HandJointSynchronizer
        HandJointSynchronizer[] allSynchronizers = FindObjectsOfType<HandJointSynchronizer>();
        debugInfo += $"Total HandJointSynchronizer objects: {allSynchronizers.Length}\n";

        Debug.Log(debugInfo);
    }

    /// <summary>
    /// 手动检查特定客户端的手部关节
    /// </summary>
    public void CheckClientHandJoints(ulong clientId)
    {
        NetworkHandJoint[] clientHandJoints = System.Array.FindAll(
            FindObjectsOfType<NetworkHandJoint>(),
            hj => hj.OwnerClientId == clientId
        );

        Debug.Log($"[NetworkHandSyncDebugger] Client {clientId} has {clientHandJoints.Length} network hand joints");
        
        foreach (var handJoint in clientHandJoints)
        {
            Debug.Log($"  - HandJoint Index: {handJoint.HandJointIndex}, IsOwner: {handJoint.IsOwner}");
        }
    }

    /// <summary>
    /// 检查是否有孤立的 HandJointSynchronizer（没有对应的 NetworkHandJoint）
    /// </summary>
    public void CheckOrphanedSynchronizers()
    {
        HandJointSynchronizer[] allSynchronizers = FindObjectsOfType<HandJointSynchronizer>();
        NetworkHandJoint[] allNetworkHandJoints = FindObjectsOfType<NetworkHandJoint>();
        
        List<int> networkHandJointIndices = new List<int>();
        foreach (var networkHandJoint in allNetworkHandJoints)
        {
            networkHandJointIndices.Add(networkHandJoint.HandJointIndex);
        }

        Debug.Log("[NetworkHandSyncDebugger] Checking for orphaned HandJointSynchronizers...");
        
        foreach (var synchronizer in allSynchronizers)
        {
            if (!networkHandJointIndices.Contains(synchronizer.HandJointIndex))
            {
                Debug.LogWarning($"Orphaned HandJointSynchronizer found: Index {synchronizer.HandJointIndex}, Object: {synchronizer.name}");
            }
        }
    }

    [ContextMenu("Force Debug Update")]
    public void ForceDebugUpdate()
    {
        UpdateDebugInfo();
    }

    [ContextMenu("Check All Clients")]
    public void CheckAllClients()
    {
        for (int i = 0; i < NetworkManager.Singleton.ConnectedClients.Count; i++)
        {
            CheckClientHandJoints((ulong)i);
        }
    }

    [ContextMenu("Check Orphaned Synchronizers")]
    public void CheckOrphanedSynchronizersMenu()
    {
        CheckOrphanedSynchronizers();
    }
}