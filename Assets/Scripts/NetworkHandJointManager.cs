using UnityEngine;
using Unity.Netcode;

public class NetworkHandJointManager : NetworkBehaviour
{
    [Header("Hand References")]
    [SerializeField] private GameObject leftHand;
    [SerializeField] private GameObject rightHand;
    
    [Header("Prefab Settings")]
    [SerializeField] private GameObject handJointSynchronizerPrefab;
    
    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = true;
    
    private static int globalJointIndex = 0;

    public override void OnNetworkSpawn()
    {
        // 暂时禁用旧系统，避免与新的 SimpleHandNetworkSync 冲突
        Debug.Log($"[NetworkHandJointManager] DISABLED - Using SimpleHandNetworkSync instead. Client {OwnerClientId}");
        
        // 注释掉旧的逻辑
        /*
        if (IsOwner)
        {
            leftHand = FindHandForOwner("Left Hand");
            rightHand = FindHandForOwner("Right Hand");

            if (leftHand == null || rightHand == null)
            {
                Debug.LogWarning($"[NetworkHandJointManager] Client {OwnerClientId} could not find hand objects. Left: {leftHand != null}, Right: {rightHand != null}");
            }

            SetupHandJointsForOwner();
        }
        */
    }
    
    private GameObject FindHandForOwner(string handName)
    {
        // 尝试多种方式查找手部对象
        GameObject hand = GameObject.Find(handName);
        
        if (hand == null)
        {
            // 如果直接查找失败，尝试查找包含客户端ID的手部对象
            hand = GameObject.Find($"{handName} Client {OwnerClientId}");
        }
        
        if (hand == null)
        {
            // 查找所有可能的手部对象
            GameObject[] allHands = GameObject.FindGameObjectsWithTag("Hand");
            foreach (var potentialHand in allHands)
            {
                if (potentialHand.name.Contains(handName) && 
                    potentialHand.name.Contains(OwnerClientId.ToString()))
                {
                    hand = potentialHand;
                    break;
                }
            }
        }
        
        return hand;
    }
    
    private void SetupHandJointsForOwner()
    {
        if (leftHand != null)
        {
            SetupHandJoints(leftHand, "Left");
        }
        
        if (rightHand != null)
        {
            SetupHandJoints(rightHand, "Right");
        }
    }
    
    private void SetupHandJoints(GameObject hand, string handType)
    {
        // 获取手部游戏对象下的所有子物体
        Transform[] childTransforms = hand.GetComponentsInChildren<Transform>();
        
        int localJointCount = 0;
        
        foreach (Transform childTransform in childTransforms)
        {
            // 跳过父物体本身
            if (childTransform == hand.transform)
                continue;
            
            // 为每个子物体创建 HandJointSynchronizer
            GameObject synchronizerObject = Instantiate(handJointSynchronizerPrefab, childTransform);
            
            // 使用全局唯一索引，包含客户端ID避免冲突
            int uniqueIndex = globalJointIndex + (int)(OwnerClientId * 1000);
            synchronizerObject.name = $"Hand Joint Synchronizer {uniqueIndex}";
            
            // 获取 HandJointSynchronizer 组件并设置属性
            if (synchronizerObject.TryGetComponent<HandJointSynchronizer>(out HandJointSynchronizer synchronizer))
            {
                // 设置 HandJointIndex，这个值会传递给 NetworkHandJoint
                synchronizer.HandJointIndex = uniqueIndex;
                synchronizer.m_HandJoint = childTransform;
                if (enableDebugLogs)
                {
                    Debug.Log($"[Client {OwnerClientId}] Created HandJointSynchronizer for {handType} hand joint: {childTransform.name} with index {uniqueIndex}");
                }
            }
            else
            {
                Debug.LogWarning($"HandJointSynchronizerPrefab does not contain HandJointSynchronizer component!");
                Destroy(synchronizerObject);
                continue;
            }
            
            globalJointIndex++;
            localJointCount++;
        }
        
        if (enableDebugLogs)
        {
            Debug.Log($"[Client {OwnerClientId}] Setup completed for {handType} hand: {localJointCount} joints synchronized");
        }
    }
}