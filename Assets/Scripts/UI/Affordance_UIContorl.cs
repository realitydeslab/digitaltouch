using UnityEngine;
using Unity.Netcode;
using System;
using System.Collections.Generic;

public class Affordance_UIControl : MonoBehaviour
{
    public List<BaseAffordanceControl> affordanceControls;
    // public BaseVFXControl Flame;

    // public BaseVFXControl Particle;

    // private bool isFlameEnable = false;
    // private bool isParticleEnable = false;
    // public static event Action<bool> OnFlameEnableChange;
    // public static event Action<bool> OnParticleEnableChange;

    private void Start()
    {
        // if (Flame != null)
        // {
        //     Flame.enabled = isFlameEnable;
        // }
        // if (Particle != null)
        // {
        //     Particle.enabled = isParticleEnable;
        // }
    }

    // public void ToggleFlame()
    // {
    //     isFlameEnable = !isFlameEnable;
    //     Flame.enabled = isFlameEnable;
    //     OnFlameEnableChange?.Invoke(isFlameEnable);
    // }

    // public void ToggleParticle()
    // {
    //     isParticleEnable = !isParticleEnable;
    //     Particle.enabled = isParticleEnable;
    //     OnParticleEnableChange?.Invoke(isParticleEnable);
    // }

    // public void SetFlame(bool state)
    // {
    //     if (isFlameEnable == state)
    //     {
    //         return;
    //     }
    //     else
    //     {
    //         isFlameEnable = state;
    //         Flame.enabled = isFlameEnable;
    //     }
    //     // OnFlameEnableChange?.Invoke(isFlameEnable);
    // }

    // public void SetParticle(bool state)
    // {
    //     if (isParticleEnable == state)
    //     {
    //         return;
    //     }
    //     else
    //     {
    //         isParticleEnable = state;
    //         Particle.enabled = isParticleEnable;
    //     }
    //     // OnParticleEnableChange?.Invoke(isParticleEnable);
    // }
}