using UnityEngine;
using Unity.Netcode;
using System;
using System.Collections.Generic;
using System.Drawing;

[System.Serializable]
public class LocalRelation
{
    public string RelationName = "Local_IndexTip";
    public Transform Point_1;
    public Transform Point_2;
    public float GetDistance()
    {
        return Vector3.Distance(Point_1.position, Point_2.position);
    }

    public Vector3 GetCenterPoint()
    {
        return Vector3.Lerp(Point_1.position, Point_2.position, 0.5f);
    }
}

[System.Serializable]
public class NetworkRelation
{
    public string RelationName = "Network_LeftIndexTip";
    public Transform LocalPoint;
}

// [System.Serializable]
// public class NetworkAffordance
// {
//     public BaseAffordanceControl affordance;
//     public NetworkVariable<bool> networkIsEnable = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

//     public NetworkAffordance(BaseAffordanceControl _affordance)
//     {
//         affordance = _affordance;
//         //networkIsEnable = 
//         networkIsEnable.OnValueChanged += onAffordanceEnableChange;
//     }

//     ~NetworkAffordance()
//     {
//         networkIsEnable.OnValueChanged -= onAffordanceEnableChange;
//     }
//     public void onAffordanceEnableChange(bool previousState, bool currentState)
//     {
//         affordance.SetEnable(currentState);
//         Debug.Log("Affordance State is " + currentState);
//     }
// }

public class NetworkHandsRelationManager : NetworkBehaviour
{
    public List<LocalRelation> Relations = new List<LocalRelation>();
    // public List<BaseAffordanceControl> affordanceControls = new List<BaseAffordanceControl>();
    // public List<NetworkVariable<bool>> networkIsAffordanceEnableStates = new List<NetworkVariable<bool>>();
    public List<NetworkAffordance> networkAffordances = new List<NetworkAffordance>();

    public Transform XR_Camera;

    [Header("Hand Joint Transforms (per player)")]
    public Transform leftIndexTip;
    public Transform rightIndexTip;

    public Affordance_UIControl affordanceController;

    public NetworkHandsRelationManager[] playerManagers;

    public NetworkVariable<float> networkCameraDistance = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<Vector3> networkCameraCenterPosition = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<float> networkIndexDistance = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<Vector3> networkIndexCenterPosition = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public static NetworkHandsRelationManager Instance { get; private set; }

    public override void OnNetworkSpawn()
    {
        GameObject controllerObject = GameObject.Find("UI Canvas");
        if (controllerObject)
        {
            affordanceController = controllerObject.GetComponent<Affordance_UIControl>();
            foreach (var affordance in affordanceController.affordanceControls)
            {
                affordance.OnEnableChange += onAffordanceEnableChangServerRpc;
                networkAffordances.Add(new NetworkAffordance(affordance));
                Debug.Log("add a network affordance");
            }
        }

        if (IsOwner)
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }
    }

    public override void OnNetworkDespawn()
    {
        foreach (var affordance in affordanceController.affordanceControls)
        {
            affordance.OnEnableChange -= onAffordanceEnableChangServerRpc;
        }

        if (IsOwner && Instance == this)
        {
            Instance = null;
        }
    }

    private void Update()
    {
        if (!IsServer)
        {
            return;
        }

        playerManagers = FindObjectsOfType<NetworkHandsRelationManager>();

        if (playerManagers.Length == 1)
        {
            if (playerManagers[0].leftIndexTip != null && playerManagers[0].rightIndexTip != null)
            {
                float dist = Vector3.Distance(playerManagers[0].leftIndexTip.position, playerManagers[0].rightIndexTip.position);
                Vector3 center = Vector3.Lerp(playerManagers[0].leftIndexTip.position, playerManagers[0].rightIndexTip.position, 0.5f);
                networkIndexDistance.Value = dist;
                networkIndexCenterPosition.Value = center;
            }
        }
        else if (playerManagers.Length >= 2)
        {
            NetworkHandsRelationManager hostManager = playerManagers[0].IsHost ? playerManagers[0] : playerManagers[1];
            NetworkHandsRelationManager clientManager = playerManagers[0].IsHost ? playerManagers[1] : playerManagers[0];

            // Relations of XR Cameras.
            if (hostManager.XR_Camera != null && clientManager.XR_Camera != null)
            {
                float dist = Vector3.Distance(hostManager.XR_Camera.position, clientManager.XR_Camera.position);
                Vector3 center = Vector3.Lerp(hostManager.XR_Camera.position, clientManager.XR_Camera.position, 0.5f);
                networkCameraDistance.Value = dist;
                networkCameraCenterPosition.Value = center;
            }

            if (hostManager.leftIndexTip != null && hostManager.rightIndexTip != null &&
                clientManager.leftIndexTip != null && clientManager.rightIndexTip != null)
            {
                Vector3 hostHandsCenter = Vector3.Lerp(hostManager.leftIndexTip.position, hostManager.rightIndexTip.position, 0.5f);
                Vector3 clientHandsCenter = Vector3.Lerp(clientManager.leftIndexTip.position, clientManager.rightIndexTip.position, 0.5f);

                float dist = Vector3.Distance(hostHandsCenter, clientHandsCenter);

                Vector3 center = Vector3.Lerp(hostHandsCenter, clientHandsCenter, 0.5f);

                networkIndexDistance.Value = dist;
                networkIndexCenterPosition.Value = center;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void onAffordanceEnableChangServerRpc(int index, bool state)
    {
        Debug.Log($"Origin affordance {index} is {networkAffordances[index].networkIsEnable.Value}, in {networkAffordances.Count}");
        Debug.Log($"Enable affordance {index} to be {state}");
        networkAffordances[index].networkIsEnable.Value = state;
    }
}