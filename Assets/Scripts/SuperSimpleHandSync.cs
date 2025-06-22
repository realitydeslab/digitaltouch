using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

/// <summary>
/// 超简化的手部同步方案
/// 不需要每个关节都是NetworkObject，只用网络变量传输数据
/// </summary>
public class SuperSimpleHandSync : NetworkBehaviour
{
    [Header("Hand Detection")]
    [SerializeField] private bool autoFindHands = true;
    [SerializeField] private Transform leftHandRoot;
    [SerializeField] private Transform rightHandRoot;
    
    [Header("Visualization")]
    [SerializeField] private GameObject jointVisualizationPrefab;
    [SerializeField] private float jointSize = 0.02f;
    
    // 存储手部关节位置的网络变量
    private NetworkVariable<HandData> leftHandData = new NetworkVariable<HandData>(
        new HandData(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<HandData> rightHandData = new NetworkVariable<HandData>(
        new HandData(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    
    // 本地手部关节
    private Transform[] leftHandJoints;
    private Transform[] rightHandJoints;
    
    // 可视化对象
    private Dictionary<int, GameObject> leftHandVisuals = new Dictionary<int, GameObject>();
    private Dictionary<int, GameObject> rightHandVisuals = new Dictionary<int, GameObject>();
    
    // 结构体用于网络传输
    [System.Serializable]
    public struct HandData : INetworkSerializable
    {
        public Vector3[] positions;
        public bool isValid;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref isValid);
            
            if (isValid)
            {
                int count = positions?.Length ?? 0;
                serializer.SerializeValue(ref count);
                
                if (serializer.IsReader)
                {
                    positions = new Vector3[count];
                }
                
                for (int i = 0; i < count; i++)
                {
                    serializer.SerializeValue(ref positions[i]);
                }
            }
        }
    }
    
    public override void OnNetworkSpawn()
    {
        Debug.Log($"[SuperSimpleHandSync] Spawned for Client {OwnerClientId}, IsOwner: {IsOwner}");
        
        if (IsOwner)
        {
            StartCoroutine(InitializeHands());
        }
        
        // 监听网络数据变化
        leftHandData.OnValueChanged += OnLeftHandDataChanged;
        rightHandData.OnValueChanged += OnRightHandDataChanged;
    }
    
    private System.Collections.IEnumerator InitializeHands()
    {
        yield return new WaitForSeconds(0.5f);
        
        if (autoFindHands)
        {
            FindHands();
        }
        
        SetupHandJoints();
    }
    
    private void FindHands()
    {
        if (leftHandRoot == null)
        {
            string[] leftNames = { "Left Hand", "LeftHand", "Hand_Left", "XR_Hand_Left" };
            foreach (string name in leftNames)
            {
                GameObject obj = GameObject.Find(name);
                if (obj != null)
                {
                    leftHandRoot = obj.transform;
                    Debug.Log($"[SuperSimpleHandSync] Found left hand: {name}");
                    break;
                }
            }
        }
        
        if (rightHandRoot == null)
        {
            string[] rightNames = { "Right Hand", "RightHand", "Hand_Right", "XR_Hand_Right" };
            foreach (string name in rightNames)
            {
                GameObject obj = GameObject.Find(name);
                if (obj != null)
                {
                    rightHandRoot = obj.transform;
                    Debug.Log($"[SuperSimpleHandSync] Found right hand: {name}");
                    break;
                }
            }
        }
    }
    
    private void SetupHandJoints()
    {
        if (leftHandRoot != null)
        {
            leftHandJoints = leftHandRoot.GetComponentsInChildren<Transform>();
            Debug.Log($"[SuperSimpleHandSync] Left hand has {leftHandJoints.Length} joints");
        }
        
        if (rightHandRoot != null)
        {
            rightHandJoints = rightHandRoot.GetComponentsInChildren<Transform>();
            Debug.Log($"[SuperSimpleHandSync] Right hand has {rightHandJoints.Length} joints");
        }
    }
    
    private void FixedUpdate()
    {
        if (!IsSpawned || !IsOwner) return;
        
        // 更新左手数据
        if (leftHandJoints != null && leftHandJoints.Length > 0)
        {
            Vector3[] leftPositions = new Vector3[leftHandJoints.Length];
            for (int i = 0; i < leftHandJoints.Length; i++)
            {
                leftPositions[i] = leftHandJoints[i].position;
            }
            
            leftHandData.Value = new HandData { positions = leftPositions, isValid = true };
        }
        
        // 更新右手数据
        if (rightHandJoints != null && rightHandJoints.Length > 0)
        {
            Vector3[] rightPositions = new Vector3[rightHandJoints.Length];
            for (int i = 0; i < rightHandJoints.Length; i++)
            {
                rightPositions[i] = rightHandJoints[i].position;
            }
            
            rightHandData.Value = new HandData { positions = rightPositions, isValid = true };
        }
    }
    
    private void OnLeftHandDataChanged(HandData previousValue, HandData newValue)
    {
        if (IsOwner || !newValue.isValid) return;
        
        UpdateHandVisuals(newValue, leftHandVisuals, "LeftHand", OwnerClientId);
    }
    
    private void OnRightHandDataChanged(HandData previousValue, HandData newValue)
    {
        if (IsOwner || !newValue.isValid) return;
        
        UpdateHandVisuals(newValue, rightHandVisuals, "RightHand", OwnerClientId);
    }
    
    private void UpdateHandVisuals(HandData handData, Dictionary<int, GameObject> visuals, string handName, ulong clientId)
    {
        // 清理旧的可视化对象
        foreach (var visual in visuals.Values)
        {
            if (visual != null) Destroy(visual);
        }
        visuals.Clear();
        
        // 创建新的可视化对象
        if (handData.positions != null)
        {
            for (int i = 0; i < handData.positions.Length; i++)
            {
                GameObject visual = CreateJointVisual(handData.positions[i], clientId, $"{handName}_Joint_{i}");
                visuals[i] = visual;
            }
        }
        
        Debug.Log($"[SuperSimpleHandSync] Updated {handName} visuals for client {clientId}: {handData.positions?.Length ?? 0} joints");
    }
    
    private GameObject CreateJointVisual(Vector3 position, ulong clientId, string name)
    {
        GameObject visual;
        
        if (jointVisualizationPrefab != null)
        {
            visual = Instantiate(jointVisualizationPrefab, position, Quaternion.identity);
        }
        else
        {
            visual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            visual.transform.localScale = Vector3.one * jointSize;
            
            // 移除碰撞器
            Collider col = visual.GetComponent<Collider>();
            if (col != null) Destroy(col);
        }
        
        visual.name = name;
        
        // 设置颜色
        Renderer renderer = visual.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material mat = renderer.material;
            mat.color = clientId == 0 ? Color.red : Color.blue;
        }
        
        return visual;
    }
    
    public override void OnNetworkDespawn()
    {
        // 清理所有可视化对象
        foreach (var visual in leftHandVisuals.Values)
        {
            if (visual != null) Destroy(visual);
        }
        
        foreach (var visual in rightHandVisuals.Values)
        {
            if (visual != null) Destroy(visual);
        }
        
        leftHandVisuals.Clear();
        rightHandVisuals.Clear();
    }
    
    [ContextMenu("Manual Find Hands")]
    public void ManualFindHands()
    {
        FindHands();
        SetupHandJoints();
    }
}