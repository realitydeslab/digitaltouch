using UnityEngine;
using Unity.Netcode;

public class HandJointSynchronizer : NetworkBehaviour
{
    [SerializeField] private GameObject m_NetworkHandJointPrefab;

    public Transform m_HandJoint;

    //public Transform HandJoint => m_HandJoint;

    //public NetworkVariable<Transform> HandJoint;

    public int HandJointIndex;

    private void Awake()
    {
        m_HandJoint = this.transform.parent;
    }


    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            if (IsClient)
            {
                Debug.Log($"[HandJointSynchronizer] Spawning NetworkHandJoint for index {HandJointIndex}, Owner: {OwnerClientId}");
                SpawnNetworkHandJointServerRpc();
            }

        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnNetworkHandJointServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var networkHandJoint = Instantiate(m_NetworkHandJointPrefab);
        NetworkObject netObj = networkHandJoint.GetComponent<NetworkObject>();

        // 在网络生成前设置 HandJointIndex
        if (netObj.TryGetComponent<NetworkHandJoint>(out var handJoint))
        {
            handJoint.HandJointIndex = HandJointIndex;
            Debug.Log($"[HandJointSynchronizer] Server spawning NetworkHandJoint {HandJointIndex} for client {serverRpcParams.Receive.SenderClientId}");
        }

        // 生成网络对象并分配给请求的客户端
        netObj.SpawnWithOwnership(serverRpcParams.Receive.SenderClientId);
        // SpawnNetworkHandJointClientRpc(serverRpcParams.Receive.SenderClientId);
    }

    // [ClientRpc]
    // private void SpawnNetworkHandJointClientRpc(ulong senderClientID)
    // {
    //     var networkHandJoint = Instantiate(m_NetworkHandJointPrefab);
    //     NetworkObject netObj = networkHandJoint.GetComponent<NetworkObject>();

    //     // 在网络生成前设置 HandJointIndex
    //     if (netObj.TryGetComponent<NetworkHandJoint>(out var handJoint) && senderClientID == NetworkManager.Singleton.LocalClientId)
    //     {
    //         handJoint.HandJointIndex = HandJointIndex;
    //         Debug.Log($"the local client ID is {NetworkManager.Singleton.LocalClientId}, the sender client id is {senderClientID}.");
    //     }
    // }
}
