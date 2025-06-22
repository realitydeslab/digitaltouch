using UnityEngine;
using Unity.Netcode;

/// <summary>
/// 自动为手部关节设置网络同步的工具
/// </summary>
public class HandSyncSetup : NetworkBehaviour
{
    [Header("Hand References")]
    [SerializeField] private Transform leftHandRoot;
    [SerializeField] private Transform rightHandRoot;
    
    [Header("Visualization Settings")]
    [SerializeField] private GameObject visualPrefab;
    [SerializeField] private Material hostMaterial;
    [SerializeField] private Material clientMaterial;
    
    [Header("Debug")]
    [SerializeField] private bool autoSetupOnStart = true;
    
    void Start()
    {
        if (autoSetupOnStart)
        {
            SetupHandSync();
        }
    }
    
    [ContextMenu("Setup Hand Sync")]
    public void SetupHandSync()
    {
        Debug.Log("[HandSyncSetup] Starting hand sync setup...");
        
        int totalJoints = 0;
        
        if (leftHandRoot != null)
        {
            totalJoints += SetupHandJoints(leftHandRoot, "LeftHand");
        }
        
        if (rightHandRoot != null)
        {
            totalJoints += SetupHandJoints(rightHandRoot, "RightHand");
        }
        
        Debug.Log($"[HandSyncSetup] Setup complete! Added SimpleHandNetworkSync to {totalJoints} joints");
    }
    
    private int SetupHandJoints(Transform handRoot, string handName)
    {
        Transform[] allTransforms = handRoot.GetComponentsInChildren<Transform>();
        int count = 0;
        
        foreach (Transform jointTransform in allTransforms)
        {
            // 跳过根对象
            if (jointTransform == handRoot) continue;
            
            // 检查是否已经有 SimpleHandNetworkSync 组件
            if (jointTransform.GetComponent<SimpleHandNetworkSync>() != null) continue;
            
            // 添加 NetworkObject（如果没有的话）
            NetworkObject netObj = jointTransform.GetComponent<NetworkObject>();
            if (netObj == null)
            {
                netObj = jointTransform.gameObject.AddComponent<NetworkObject>();
            }
            
            // 添加 SimpleHandNetworkSync 组件
            SimpleHandNetworkSync syncComponent = jointTransform.gameObject.AddComponent<SimpleHandNetworkSync>();
            
            // 设置可视化设置
            SetSyncComponentSettings(syncComponent);
            
            Debug.Log($"[HandSyncSetup] Added sync to {handName} joint: {jointTransform.name}");
            count++;
        }
        
        return count;
    }
    
    private void SetSyncComponentSettings(SimpleHandNetworkSync syncComponent)
    {
        // 使用反射或其他方式设置私有字段，或者修改 SimpleHandNetworkSync 为公开字段
        // 为了简单起见，我们可以让 SimpleHandNetworkSync 使用默认设置
    }
    
    [ContextMenu("Remove All Hand Sync")]
    public void RemoveAllHandSync()
    {
        SimpleHandNetworkSync[] allSyncComponents = FindObjectsByType<SimpleHandNetworkSync>(FindObjectsSortMode.None);
        
        foreach (var sync in allSyncComponents)
        {
            DestroyImmediate(sync);
        }
        
        Debug.Log($"[HandSyncSetup] Removed {allSyncComponents.Length} SimpleHandNetworkSync components");
    }
    
    [ContextMenu("List All Hand Joints")]
    public void ListAllHandJoints()
    {
        Debug.Log("[HandSyncSetup] Listing all hand joints:");
        
        if (leftHandRoot != null)
        {
            ListJointsInHand(leftHandRoot, "Left Hand");
        }
        
        if (rightHandRoot != null)
        {
            ListJointsInHand(rightHandRoot, "Right Hand");
        }
    }
    
    private void ListJointsInHand(Transform handRoot, string handName)
    {
        Transform[] joints = handRoot.GetComponentsInChildren<Transform>();
        Debug.Log($"{handName} has {joints.Length - 1} joints:"); // -1 because we exclude the root
        
        foreach (Transform joint in joints)
        {
            if (joint == handRoot) continue;
            
            bool hasNetworkObject = joint.GetComponent<NetworkObject>() != null;
            bool hasSync = joint.GetComponent<SimpleHandNetworkSync>() != null;
            
            Debug.Log($"  - {joint.name} | NetworkObject: {hasNetworkObject} | Sync: {hasSync}");
        }
    }
}