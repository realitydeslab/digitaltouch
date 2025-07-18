using UnityEngine;
using Unity.Netcode;
using System;

public class NetworkHandJoint : NetworkBehaviour
{
    [SerializeField] private NetworkVariable<Vector3> m_HandJointPosition = new(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [SerializeField] private NetworkVariable<Quaternion> m_HandJointRotation = new(Quaternion.identity, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public Transform m_HandJoint;
    public bool IsCamera;

    [SerializeField] private Transform m_WorldRoot;

    public int HandJointIndex;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            // 尝试查找对应的 HandJointSynchronizer
            StartCoroutine(FindHandJointSynchronizer());
        }

        m_HandJointPosition.OnValueChanged += OnHandJointPositionChanged;
        m_HandJointRotation.OnValueChanged += OnHandJointRotationChanged;

        GetComponent<MeshRenderer>().material.color = OwnerClientId == 0 ? Color.red : Color.blue;
        
        // 查找或创建 World Root
        SetupWorldRoot();
    }
    
    private System.Collections.IEnumerator FindHandJointSynchronizer()
    {
        // 修复索引计算 - HandJointIndex 已经包含了客户端偏移
        string targetName = $"Hand Joint Synchronizer {HandJointIndex}";
        if (IsCamera)
            targetName = "XR Camera Synchronizer";
        
        int maxRetries = 30; // 最多等待3秒
        int retries = 0;
        
        //Debug.Log($"[NetworkHandJoint] Looking for {targetName} for OwnerClientId {OwnerClientId}");
        
        while (retries < maxRetries)
        {
            GameObject syncObject = GameObject.Find(targetName);
            if (syncObject != null)
            {
                if (syncObject.TryGetComponent<HandJointSynchronizer>(out HandJointSynchronizer synchronizer))
                {
                    m_HandJoint = synchronizer.m_HandJoint;
                    //Debug.Log($"[NetworkHandJoint] Successfully found HandJoint for index {HandJointIndex}, Owner: {OwnerClientId}");
                    yield break;
                }
                if (syncObject.TryGetComponent<SimpleHandJointSynchronizer>(out SimpleHandJointSynchronizer simpleSynchronizer))
                {
                    m_HandJoint = simpleSynchronizer.m_HandJoint;
                    //Debug.Log($"[NetworkHandJoint] Successfully found HandJoint for index {HandJointIndex}, Owner: {OwnerClientId}");
                    yield break;
                }
            }
            
            retries++;
            yield return new WaitForSeconds(0.1f);
        }
        
        Debug.LogError($"[NetworkHandJoint] Failed to find Hand Joint Synchronizer {HandJointIndex} for Owner {OwnerClientId} after {maxRetries * 0.1f} seconds");
    }
    
    private void SetupWorldRoot()
    {
        GameObject worldRootObject = GameObject.Find("World Root");
        if (worldRootObject == null)
        {
            worldRootObject = new GameObject("World Root");
            Debug.LogWarning("[NetworkHandJoint] World Root not found, created a new one at origin");
        }
        m_WorldRoot = worldRootObject.transform;
    }

    private void FixedUpdate()
    {
        if (IsSpawned && IsOwner && m_HandJoint != null && m_WorldRoot != null)
        {
            Vector3 relativePosition = m_WorldRoot.InverseTransformPoint(m_HandJoint.position);
            Quaternion relativeRotation = Quaternion.Inverse(m_WorldRoot.rotation) * m_HandJoint.rotation;

            m_HandJointPosition.Value = relativePosition;
            m_HandJointRotation.Value = relativeRotation;

            transform.SetPositionAndRotation(m_HandJoint.position, m_HandJoint.rotation);
        }
    }

    private void OnHandJointPositionChanged(Vector3 previousValue, Vector3 newValue)
    {
        if (!IsOwner)
        {
            transform.position = m_WorldRoot.TransformPoint(newValue);
            //Debug.Log("Not owner position change");
        }
    }

    private void OnHandJointRotationChanged(Quaternion previousValue, Quaternion newValue)
    {
        if (!IsOwner)
        {
            transform.rotation = m_WorldRoot.rotation * newValue;
            //Debug.Log("Not owner rotation change");
        }   
    }
}
