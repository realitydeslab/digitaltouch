using UnityEngine.Networking;
using UnityEngine;
using Unity.Netcode;
using System;

public class NetworkAffordance : NetworkBehaviour
{
    public BaseAffordanceControl affordance;
    public NetworkVariable<bool> networkIsEnable = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkAffordance(BaseAffordanceControl _affordance)
    {
        affordance = _affordance;
        //networkIsEnable = 
        networkIsEnable.OnValueChanged += onAffordanceEnableChange;
    }

    ~NetworkAffordance()
    {
        networkIsEnable.OnValueChanged -= onAffordanceEnableChange;
    }
    public void onAffordanceEnableChange(bool previousState, bool currentState)
    {
        affordance.SetEnable(currentState);
        Debug.Log("Affordance State is " + currentState);
    }
}