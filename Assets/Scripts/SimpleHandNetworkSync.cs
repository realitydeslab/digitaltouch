using UnityEngine;
using Unity.Netcode;

/// <summary>
/// 简单直接的手部网络同步 - 一个脚本解决所有问题
/// 将此脚本附加到每个手部关节上，它会自动创建网络可视化
/// 兼容现有的visionOS项目结构
/// </summary>
public class SimpleHandNetworkSync : NetworkBehaviour
{
    [Header("Visualization")]
    [SerializeField] private GameObject visualPrefab; // 简单的球体或立方体
    [SerializeField] private Material hostMaterial;   // 红色材质
    [SerializeField] private Material clientMaterial; // 蓝色材质
    [SerializeField] private float visualScale = 0.02f; // 可视化对象大小
    
    // 网络同步的位置和旋转
    private NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>(
        Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<Quaternion> networkRotation = new NetworkVariable<Quaternion>(
        Quaternion.identity, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    
    private GameObject visualObject;
    private Transform worldRoot;
    
    public override void OnNetworkSpawn()
    {
        // 找到或创建 World Root
        SetupWorldRoot();
        
        // 创建可视化对象
        CreateVisualization();
        
        // 设置网络变量回调
        networkPosition.OnValueChanged += OnPositionChanged;
        networkRotation.OnValueChanged += OnRotationChanged;
        
        Debug.Log($"[SimpleHandNetworkSync] {gameObject.name} spawned - IsOwner: {IsOwner}, ClientId: {OwnerClientId}");
    }
    
    private void SetupWorldRoot()
    {
        GameObject worldRootObject = GameObject.Find("World Root");
        if (worldRootObject == null)
        {
            worldRootObject = new GameObject("World Root");
        }
        worldRoot = worldRootObject.transform;
    }
    
    private void CreateVisualization()
    {
        if (visualPrefab != null)
        {
            visualObject = Instantiate(visualPrefab, transform.position, transform.rotation);
        }
        else
        {
            // 如果没有预制体，创建一个简单的球体
            visualObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            visualObject.transform.localScale = Vector3.one * visualScale;
        }
        
        visualObject.name = $"HandJoint_{gameObject.name}_Client{OwnerClientId}";
        
        // 设置材质颜色
        Renderer renderer = visualObject.GetComponent<Renderer>();
        if (renderer != null)
        {
            if (OwnerClientId == 0) // Host
            {
                if (hostMaterial != null)
                    renderer.material = hostMaterial;
                else
                    renderer.material.color = Color.red;
            }
            else // Client
            {
                if (clientMaterial != null)
                    renderer.material = clientMaterial;
                else
                    renderer.material.color = Color.blue;
            }
        }
        
        // 移除碰撞器（如果有的话）
        Collider col = visualObject.GetComponent<Collider>();
        if (col != null) Destroy(col);
    }
    
    private void FixedUpdate()
    {
        if (IsSpawned && IsOwner)
        {
            // 只有所有者更新网络数据
            Vector3 relativePos = worldRoot.InverseTransformPoint(transform.position);
            Quaternion relativeRot = Quaternion.Inverse(worldRoot.rotation) * transform.rotation;
            
            networkPosition.Value = relativePos;
            networkRotation.Value = relativeRot;
            
            // 更新本地可视化
            if (visualObject != null)
            {
                visualObject.transform.SetPositionAndRotation(transform.position, transform.rotation);
            }
        }
    }
    
    private void OnPositionChanged(Vector3 previousValue, Vector3 newValue)
    {
        if (!IsOwner && visualObject != null)
        {
            Vector3 worldPos = worldRoot.TransformPoint(newValue);
            visualObject.transform.position = worldPos;
        }
    }
    
    private void OnRotationChanged(Quaternion previousValue, Quaternion newValue)
    {
        if (!IsOwner && visualObject != null)
        {
            Quaternion worldRot = worldRoot.rotation * newValue;
            visualObject.transform.rotation = worldRot;
        }
    }
    
    public override void OnNetworkDespawn()
    {
        if (visualObject != null)
        {
            Destroy(visualObject);
        }
    }
}