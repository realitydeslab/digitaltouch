using UnityEngine;
using Unity.Netcode;

public class Dual_VFX_UIControl : NetworkBehaviour
{
    [Header("VFX References")]
    public BaseVFXControl Flame;
    public BaseVFXControl Particle;

    private NetworkVariable<bool> networkIsFlameEnable = new NetworkVariable<bool>(false);
    private NetworkVariable<bool> networkIsParticleEnable = new NetworkVariable<bool>(false);

    public override void OnNetworkSpawn()
    {
        networkIsFlameEnable.OnValueChanged += OnFlameStateChanged;
        networkIsParticleEnable.OnValueChanged += OnParticleStateChanged;

        OnFlameStateChanged(false, networkIsFlameEnable.Value);
        OnParticleStateChanged(false, networkIsParticleEnable.Value);
    }

    public override void OnNetworkDespawn()
    {
        networkIsFlameEnable.OnValueChanged -= OnFlameStateChanged;
        networkIsParticleEnable.OnValueChanged -= OnParticleStateChanged;
    }

    private void OnFlameStateChanged(bool previousValue, bool currentValue)
    {
        if (Flame != null)
        {
            Flame.enabled = currentValue;
        }
    }

    private void OnParticleStateChanged(bool previousValue, bool currentValue)
    {
        if (Particle != null)
        {
            Particle.enabled = currentValue;
        }
    }

    public void ToggleFlame()
    {
        ToggleFlameServerRpc();
    }

    public void ToggleParticle()
    {
        ToggleParticleServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void ToggleFlameServerRpc()
    {
        networkIsFlameEnable.Value = !networkIsFlameEnable.Value;
    }

    [ServerRpc(RequireOwnership = false)]
    private void ToggleParticleServerRpc()
    {
        networkIsParticleEnable.Value = !networkIsParticleEnable.Value;
    }
}