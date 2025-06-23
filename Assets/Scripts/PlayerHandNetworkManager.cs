using UnityEngine;
using Unity.Netcode;

/// <summary>
/// 管理单个玩家的手部网络同步
/// 这个脚本应该附加到每个玩家的预制体上
/// </summary>
public class PlayerHandNetworkManager : NetworkBehaviour
{
    [Header("Hand References")]
    [SerializeField] private GameObject leftHandRoot;
    [SerializeField] private GameObject rightHandRoot;
    
    [Header("Prefab Settings")]
    [SerializeField] private GameObject handJointSynchronizerPrefab;
    [SerializeField] private GameObject networkHandJointPrefab;
    
    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = true;
    
    private int jointCounter = 0;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            Debug.Log($"[PlayerHandNetworkManager] Player {OwnerClientId} spawned, setting up hands...");
            SetupPlayerHands();
        }
        else
        {
            Debug.Log($"[PlayerHandNetworkManager] Remote player {OwnerClientId} spawned");
        }
    }

    private void SetupPlayerHands()
    {
        // 查找当前玩家的手部对象
        FindPlayerHands();
        
        if (leftHandRoot != null)
        {
            SetupHandJoints(leftHandRoot, "Left", OwnerClientId);
        }
        else
        {
            Debug.LogWarning($"[PlayerHandNetworkManager] Left hand not found for player {OwnerClientId}");
        }
        
        if (rightHandRoot != null)
        {
            SetupHandJoints(rightHandRoot, "Right", OwnerClientId);
        }
        else
        {
            Debug.LogWarning($"[PlayerHandNetworkManager] Right hand not found for player {OwnerClientId}");
        }
    }

    private void FindPlayerHands()
    {
        // 尝试查找标准命名的手部对象
        if (leftHandRoot == null)
        {
            leftHandRoot = GameObject.Find("Left Hand");
        }
        
        if (rightHandRoot == null)
        {
            rightHandRoot = GameObject.Find("Right Hand");
        }
        
        // 如果还没找到，尝试带玩家ID的命名
        // if (leftHandRoot == null)
        // {
        //     leftHandRoot = GameObject.Find($"Left Hand Player {OwnerClientId}");
        // }
        
        // if (rightHandRoot == null)
        // {
        //     rightHandRoot = GameObject.Find($"Right Hand Player {OwnerClientId}");
        // }
        
        if (enableDebugLogs)
        {
            Debug.Log($"[PlayerHandNetworkManager] Player {OwnerClientId} found hands - Left: {leftHandRoot != null}, Right: {rightHandRoot != null}");
        }
    }

    private void SetupHandJoints(GameObject handRoot, string handType, ulong playerId)
    {
        Transform[] jointTransforms = handRoot.GetComponentsInChildren<Transform>();
        int handJointsCreated = 0;
        
        foreach (Transform jointTransform in jointTransforms)
        {
            // 跳过根对象
            if (jointTransform == handRoot.transform)
                continue;
                
            CreateHandJointSynchronizer(jointTransform, handType, playerId);
            handJointsCreated++;
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"[PlayerHandNetworkManager] Player {playerId} {handType} hand: {handJointsCreated} joints setup");
        }
    }

    private void CreateHandJointSynchronizer(Transform jointTransform, string handType, ulong playerId)
    {
        GameObject syncObject = Instantiate(handJointSynchronizerPrefab, jointTransform);
        
        int uniqueJointIndex = (int)(playerId * 1000) + jointCounter;
        syncObject.name = $"Hand Joint Synchronizer {uniqueJointIndex}";
        
        if (syncObject.TryGetComponent<HandJointSynchronizer>(out var synchronizer))
        {
            synchronizer.HandJointIndex = uniqueJointIndex;
            synchronizer.m_HandJoint = jointTransform;
            
            if (enableDebugLogs)
            {
                Debug.Log($"[PlayerHandNetworkManager] Created synchronizer for Player {playerId} {handType} {jointTransform.name} with index {uniqueJointIndex}");
            }
        }
        else
        {
            Debug.LogError($"[PlayerHandNetworkManager] HandJointSynchronizer component not found on prefab!");
            Destroy(syncObject);
            return;
        }
        
        jointCounter++;
    }

    /// <summary>
    /// 手动触发手部设置（用于调试）
    /// </summary>
    [ContextMenu("Setup Hands Manually")]
    public void SetupHandsManually()
    {
        if (IsOwner)
        {
            SetupPlayerHands();
        }
    }
}