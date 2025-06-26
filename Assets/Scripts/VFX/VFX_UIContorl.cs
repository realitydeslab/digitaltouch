using UnityEngine;

public class VFX_UIContorl : MonoBehaviour
{
    public BaseVFXControl_withEvent Flame;

    public BaseVFXControl_withEvent Particle;

    private bool isFlameEnable = false;
    private bool isParticleEnable = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Start()
    {
        if (Flame != null)
        {
            Flame.enabled = isFlameEnable;
        }
        if (Particle != null)
        {
            Particle.enabled = isParticleEnable;
        }
    }

    public void ToggleFlame()
    {
        isFlameEnable = !isFlameEnable;
        Flame.enabled = isFlameEnable;
    }

    public void ToggleParticle()
    {
        isParticleEnable = !isParticleEnable;
        Particle.enabled = isParticleEnable;
    }
}
