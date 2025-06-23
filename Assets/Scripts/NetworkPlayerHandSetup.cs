using UnityEngine;
using Unity.Netcode;

public class NetworkPlayerHandSetup : NetworkBehaviour
{
    [Header("网络预制件关节设置")]
    [SerializeField] private Transform _networkLeftHandParent;
    [SerializeField] private Transform _networkRightHandParent;

    public override void OnNetworkSpawn()
    {
        int jointIndex = 0;
        if (_networkLeftHandParent != null)
        {
            foreach(var joint in _networkLeftHandParent.GetComponentsInChildren<NetworkHandJoint>())
            {
                joint.HandJointIndex = jointIndex++;
            }
        }
        if (_networkRightHandParent != null)
        {
            foreach(var joint in _networkRightHandParent.GetComponentsInChildren<NetworkHandJoint>())
            {
                joint.HandJointIndex = jointIndex++;
            }
        }
    }
}