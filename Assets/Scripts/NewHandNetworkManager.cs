using UnityEngine;
using Unity.Netcode;

/// <summary>
/// 新的简化手部网络管理器
/// 使用 SimpleHandNetworkSync 替代复杂的旧系统
/// </summary>
public class NewHandNetworkManager : NetworkBehaviour
{
    [Header("Hand References")]
    [SerializeField] private Transform leftHandRoot;
    [SerializeField] private Transform rightHandRoot;
    
    [Header("Visualization Settings")]
    [SerializeField] private Material hostMaterial;
    [SerializeField] private Material clientMaterial;
    [SerializeField] private float jointVisualSize = 0.02f;
    
    [Header("Setup Options")]
    [SerializeField] private bool setupOnSpawn = true;
    [SerializeField] private bool onlySetupForOwner = true;
    
    public override void OnNetworkSpawn()
    {
        Debug.Log($"[NewHandNetworkManager] Spawned for Client {OwnerClientId}, IsOwner: {IsOwner}");
        
        if (setupOnSpawn && IsOwner)
        {
            // 延迟几帧执行，确保场景对象都已经加载完成
            StartCoroutine(DelayedSetup());
        }
    }
    
    private System.Collections.IEnumerator DelayedSetup()
    {
        // 等待几帧，确保所有游戏对象都已初始化
        yield return new WaitForSeconds(0.5f);
        
        SetupHandNetworkSync();
    }
    
    /// <summary>
    /// 为手部关节设置SimpleHandNetworkSync
    /// </summary>
    public void SetupHandNetworkSync()
    {
        Debug.Log($"[NewHandNetworkManager] Setting up hand network sync for Client {OwnerClientId}");
        
        int totalJoints = 0;
        
        // 动态查找手部根对象
        FindHandRoots();
        
        // 设置左手
        if (leftHandRoot != null)
        {
            totalJoints += SetupHandJoints(leftHandRoot, "LeftHand");
        }
        else
        {
            Debug.LogWarning($"[NewHandNetworkManager] Left hand root not found for Client {OwnerClientId}");
        }
        
        // 设置右手
        if (rightHandRoot != null)
        {
            totalJoints += SetupHandJoints(rightHandRoot, "RightHand");
        }
        else
        {
            Debug.LogWarning($"[NewHandNetworkManager] Right hand root not found for Client {OwnerClientId}");
        }
        
        Debug.Log($"[NewHandNetworkManager] Client {OwnerClientId} setup complete: {totalJoints} joints configured");
    }
    
    /// <summary>
    /// 智能查找手部根对象，尝试多种命名方式
    /// </summary>
    private void FindHandRoots()
    {
        // 如果已经在Inspector中设置了，就不重新查找
        if (leftHandRoot != null && rightHandRoot != null)
        {
            Debug.Log("[NewHandNetworkManager] Hand roots already assigned in Inspector");
            return;
        }
        
        Debug.Log("[NewHandNetworkManager] Searching for hand root objects...");
        
        // 常见的手部命名方式
        string[] leftHandNames = {
            "Left Hand",
            "LeftHand", 
            "Hand_Left",
            "XR_Hand_Left",
            "HandLeft",
            "left_hand"
        };
        
        string[] rightHandNames = {
            "Right Hand",
            "RightHand",
            "Hand_Right", 
            "XR_Hand_Right",
            "HandRight",
            "right_hand"
        };
        
        // 查找左手
        if (leftHandRoot == null)
        {
            foreach (string name in leftHandNames)
            {
                GameObject leftHand = GameObject.Find(name);
                if (leftHand != null)
                {
                    leftHandRoot = leftHand.transform;
                    Debug.Log($"[NewHandNetworkManager] Found left hand: {name}");
                    break;
                }
            }
        }
        
        // 查找右手
        if (rightHandRoot == null)
        {
            foreach (string name in rightHandNames)
            {
                GameObject rightHand = GameObject.Find(name);
                if (rightHand != null)
                {
                    rightHandRoot = rightHand.transform;
                    Debug.Log($"[NewHandNetworkManager] Found right hand: {name}");
                    break;
                }
            }
        }
        
        // 如果还是没找到，尝试通过组件查找
        if (leftHandRoot == null || rightHandRoot == null)
        {
            SearchHandsByComponent();
        }
        
        // 最终检查
        if (leftHandRoot == null)
        {
            Debug.LogError("[NewHandNetworkManager] Could not find left hand object! Please assign manually or check naming.");
        }
        
        if (rightHandRoot == null)
        {
            Debug.LogError("[NewHandNetworkManager] Could not find right hand object! Please assign manually or check naming.");
        }
    }
    
    /// <summary>
    /// 通过组件查找手部（作为备用方案）
    /// </summary>
    private void SearchHandsByComponent()
    {
        Debug.Log("[NewHandNetworkManager] Searching hands by component...");
        
        // 查找所有可能的手部组件
        // 根据你的项目，这里可能是 HandTrackingManager, Hand, 或其他组件
        MonoBehaviour[] allComponents = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None);
        
        foreach (var comp in allComponents)
        {
            string objName = comp.gameObject.name.ToLower();
            
            if (leftHandRoot == null && objName.Contains("left") && objName.Contains("hand"))
            {
                leftHandRoot = comp.transform;
                Debug.Log($"[NewHandNetworkManager] Found left hand by component: {comp.gameObject.name}");
            }
            
            if (rightHandRoot == null && objName.Contains("right") && objName.Contains("hand"))
            {
                rightHandRoot = comp.transform;
                Debug.Log($"[NewHandNetworkManager] Found right hand by component: {comp.gameObject.name}");
            }
            
            if (leftHandRoot != null && rightHandRoot != null) break;
        }
    }
    
    private int SetupHandJoints(Transform handRoot, string handName)
    {
        Transform[] allJoints = handRoot.GetComponentsInChildren<Transform>();
        int setupCount = 0;
        
        foreach (Transform joint in allJoints)
        {
            // 跳过根对象
            if (joint == handRoot) continue;
            
            // 检查是否已经有网络同步组件
            if (joint.GetComponent<SimpleHandNetworkSync>() != null)
            {
                Debug.Log($"[NewHandNetworkManager] {joint.name} already has SimpleHandNetworkSync, skipping");
                continue;
            }
            
            // 自动添加 NetworkObject（如果没有的话）
            NetworkObject netObj = joint.GetComponent<NetworkObject>();
            if (netObj == null)
            {
                netObj = joint.gameObject.AddComponent<NetworkObject>();
                
                // 设置NetworkObject为在运行时生成，不是预制体
                // 这样避免了需要手动添加到NetworkPrefabs列表的问题
                Debug.Log($"[NewHandNetworkManager] Auto-added NetworkObject to {joint.name}");
            }
            
            // 添加 SimpleHandNetworkSync
            SimpleHandNetworkSync syncComponent = joint.gameObject.AddComponent<SimpleHandNetworkSync>();
            
            // 手动生成网络对象（避免预制体依赖）
            if (IsOwner && IsSpawned)
            {
                // 请求服务器为这个关节生成网络同步
                RequestJointSyncServerRpc(joint.name, OwnerClientId);
            }
            
            // 设置可视化参数
            ConfigureSyncComponent(syncComponent, joint.name);
            
            Debug.Log($"[NewHandNetworkManager] Added SimpleHandNetworkSync to {handName}/{joint.name}");
            setupCount++;
        }
        
        return setupCount;
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void RequestJointSyncServerRpc(string jointName, ulong clientId, ServerRpcParams serverRpcParams = default)
    {
        // 在服务器端为每个客户端的关节创建可视化
        Debug.Log($"[NewHandNetworkManager] Server received request for {jointName} from client {clientId}");
        
        // 这里可以生成网络对象，但为了简化，我们让客户端自己处理可视化
        NotifyJointReadyClientRpc(jointName, clientId);
    }
    
    [ClientRpc]
    private void NotifyJointReadyClientRpc(string jointName, ulong clientId)
    {
        Debug.Log($"[NewHandNetworkManager] Joint {jointName} ready for client {clientId}");
    }
    
    private void ConfigureSyncComponent(SimpleHandNetworkSync syncComponent, string jointName)
    {
        // 这里可以根据关节名称设置特殊配置
        // 比如不同关节使用不同大小的可视化对象
        
        if (jointName.Contains("Tip"))
        {
            // 指尖使用稍大的可视化
        }
        else if (jointName.Contains("Wrist"))
        {
            // 手腕使用更大的可视化
        }
    }
    
    /// <summary>
    /// 清理所有手部网络同步组件
    /// </summary>
    [ContextMenu("Clean Up Hand Sync")]
    public void CleanUpHandSync()
    {
        if (leftHandRoot != null)
        {
            CleanUpHandJoints(leftHandRoot);
        }
        
        if (rightHandRoot != null)
        {
            CleanUpHandJoints(rightHandRoot);
        }
    }
    
    private void CleanUpHandJoints(Transform handRoot)
    {
        SimpleHandNetworkSync[] syncComponents = handRoot.GetComponentsInChildren<SimpleHandNetworkSync>();
        
        foreach (var sync in syncComponents)
        {
            DestroyImmediate(sync);
        }
        
        Debug.Log($"[NewHandNetworkManager] Cleaned up {syncComponents.Length} sync components from {handRoot.name}");
    }
    
    /// <summary>
    /// 手动触发设置（用于调试）
    /// </summary>
    [ContextMenu("Manual Setup")]
    public void ManualSetup()
    {
        SetupHandNetworkSync();
    }
}