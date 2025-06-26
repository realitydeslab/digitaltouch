using UnityEngine;
using Unity.Netcode;

public class NetworkHandsRelationManager : NetworkBehaviour
{
    // --- 成员变量 ---
    [Header("Hand Joint Transforms (per player)")]
    public Transform leftIndexTip;
    public Transform rightIndexTip;

    public NetworkHandsRelationManager[] playerManagers;

    // --- 网络同步变量 ---
    // 这两个变量的值由服务器设置，并自动同步到所有客户端。
    // 权限设置为只允许服务器写入，客户端只能读取。
    public NetworkVariable<float> networkDistance = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<Vector3> networkCenterPosition = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    // --- 单例模式，方便VFX控制器访问 ---
    // 我们假设场景中最终的效果是由Host/Server玩家的管理脚本驱动的
    public static NetworkHandsRelationManager Instance { get; private set; }

    // --- Netcode 生命周期函数 ---

    public override void OnNetworkSpawn()
    {
        // 当这个网络对象生成时
        if (IsServer)
        {
            // 如果是在服务器上生成的，并且还没有主实例，就将自己设置为主实例
            // 在我们的双人游戏中，第一个生成（Host）的自然成为主实例
            if (Instance == null)
            {
                Instance = this;
            }
        }
    }

    public override void OnNetworkDespawn()
    {
        // 当网络对象销毁时，如果是主实例，则清空静态引用
        if (IsServer && Instance == this)
        {
            Instance = null;
        }
    }


    // --- 核心逻辑 ---

    private void Update()
    {
        // 核心计算逻辑只在服务器上运行，客户端不执行任何计算
        if (!IsServer)
        {
            return;
        }

        // 找到场景中所有的玩家控制器
        playerManagers = FindObjectsOfType<NetworkHandsRelationManager>();
        
        // --- 根据玩家数量切换计算逻辑 ---

        if (playerManagers.Length == 1)
        {
            // 情况1: 只有一名玩家 (Host自己)
            // 确保这位玩家的关节都已赋值
            if (playerManagers[0].leftIndexTip != null && playerManagers[0].rightIndexTip != null)
            {
                // 计算该玩家自己双手的食指指尖距离和中心点
                float dist = Vector3.Distance(playerManagers[0].leftIndexTip.position, playerManagers[0].rightIndexTip.position);
                Vector3 center = Vector3.Lerp(playerManagers[0].leftIndexTip.position, playerManagers[0].rightIndexTip.position, 0.5f);

                // 更新网络变量，Netcode会自动将这些值同步给客户端
                networkDistance.Value = dist;
                networkCenterPosition.Value = center;
            }
        }
        else if (playerManagers.Length >= 2)
        {
            // 情况2: 有两名或以上玩家 (我们只考虑前两个)
            NetworkHandsRelationManager hostManager = playerManagers[0].IsHost ? playerManagers[0] : playerManagers[1];
            NetworkHandsRelationManager clientManager = playerManagers[0].IsHost ? playerManagers[1] : playerManagers[0];
            
            // 确保两位玩家的关节都已赋值
            if (hostManager.leftIndexTip != null && hostManager.rightIndexTip != null &&
                clientManager.leftIndexTip != null && clientManager.rightIndexTip != null)
            {
                // 1. 分别计算每个玩家双手的中心点
                Vector3 hostHandsCenter = Vector3.Lerp(hostManager.leftIndexTip.position, hostManager.rightIndexTip.position, 0.5f);
                Vector3 clientHandsCenter = Vector3.Lerp(clientManager.leftIndexTip.position, clientManager.rightIndexTip.position, 0.5f);

                // 2. 计算两个玩家“手部中心点”之间的距离
                float dist = Vector3.Distance(hostHandsCenter, clientHandsCenter);

                // 3. 计算两个玩家“手部中心点”的中心点
                Vector3 center = Vector3.Lerp(hostHandsCenter, clientHandsCenter, 0.5f);

                // 更新网络变量，Netcode会自动将这些值同步给客户端
                networkDistance.Value = dist;
                networkCenterPosition.Value = center;
            }
        }
    }
}