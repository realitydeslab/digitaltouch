using UnityEngine;
using Unity.Netcode;
using System;
using System.Collections.Generic;

[System.Serializable]
public class RelationOfJoints
{
    public string RelationName = "LeftIndexTip-RightIndexTip";
    public Transform Tip_1;
    public Transform Tip_2;

    public float GetDistance()
    {
        return Vector3.Distance(Tip_1.position, Tip_2.position);
    }

    public Vector3 GetCenterPoint()
    {
        return Vector3.Lerp(Tip_1.position, Tip_2.position, 0.5f);
    }
}

public class NetworkHandsRelationManager : NetworkBehaviour
{
    public List<RelationOfJoints> Relations = new List<RelationOfJoints>();

    [Header("Hand Joint Transforms (per player)")]
    public Transform leftIndexTip;
    public Transform rightIndexTip;

    public Affordance_UIControl vfxUIController;

    public NetworkHandsRelationManager[] playerManagers;

    public NetworkVariable<float> networkDistance = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public NetworkVariable<Vector3> networkCenterPosition = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public static NetworkHandsRelationManager Instance { get; private set; }
    public NetworkVariable<bool> networkIsFlameEnable = new NetworkVariable<bool>(false);
    public NetworkVariable<bool> networkIsParticleEnable = new NetworkVariable<bool>(false);


    public override void OnNetworkSpawn()
    {
        networkIsFlameEnable.OnValueChanged += onNetworkFlameEnableChange;
        networkIsParticleEnable.OnValueChanged += onNetworkParticleEnableChange;
        GameObject controllerObject = GameObject.Find("Dual VFX UI Controller");
        if (controllerObject)
        {
            vfxUIController = controllerObject.GetComponent<Affordance_UIControl>();
            Affordance_UIControl.OnFlameEnableChange += onFlameEnableChangServerRpc;
            Affordance_UIControl.OnParticleEnableChange += onParticleEnableChangeServerRpc;
            Debug.Log("Find Dual VFX UI Controller");
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
        networkIsFlameEnable.OnValueChanged -= onNetworkFlameEnableChange;
        networkIsParticleEnable.OnValueChanged -= onNetworkParticleEnableChange;
        Affordance_UIControl.OnFlameEnableChange -= onFlameEnableChangServerRpc;
        Affordance_UIControl.OnParticleEnableChange -= onParticleEnableChangeServerRpc;

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
                networkDistance.Value = dist;
                networkCenterPosition.Value = center;
            }
        }
        else if (playerManagers.Length >= 2)
        {
            NetworkHandsRelationManager hostManager = playerManagers[0].IsHost ? playerManagers[0] : playerManagers[1];
            NetworkHandsRelationManager clientManager = playerManagers[0].IsHost ? playerManagers[1] : playerManagers[0];

            if (hostManager.leftIndexTip != null && hostManager.rightIndexTip != null &&
                clientManager.leftIndexTip != null && clientManager.rightIndexTip != null)
            {
                Vector3 hostHandsCenter = Vector3.Lerp(hostManager.leftIndexTip.position, hostManager.rightIndexTip.position, 0.5f);
                Vector3 clientHandsCenter = Vector3.Lerp(clientManager.leftIndexTip.position, clientManager.rightIndexTip.position, 0.5f);

                float dist = Vector3.Distance(hostHandsCenter, clientHandsCenter);

                Vector3 center = Vector3.Lerp(hostHandsCenter, clientHandsCenter, 0.5f);

                networkDistance.Value = dist;
                networkCenterPosition.Value = center;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void onFlameEnableChangServerRpc(bool state)
    {
        networkIsFlameEnable.Value = state;
    }

    [ServerRpc(RequireOwnership = false)]
    private void onParticleEnableChangeServerRpc(bool state)
    {
        networkIsParticleEnable.Value = state;
    }

    private void onNetworkFlameEnableChange(bool previousState, bool currentState)
    {
        if (IsOwner)
            vfxUIController.SetFlame(currentState);
    }

    private void onNetworkParticleEnableChange(bool previousState, bool currentState)
    {
        if (IsOwner)
            vfxUIController.SetParticle(currentState);
    }
}