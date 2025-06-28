using UnityEngine;

public class VFX_UIContorl : MonoBehaviour
{
    public BaseVFXControl_withEvent Flame;

    public BaseVFXControl_withEvent Particle;

    public BaseVFXControl_withEvent Trail;

    private bool isFlameEnable = false;
    private bool isParticleEnable = false;
    private bool isTrailEnable = false;

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
        if (Trail != null)
        {
            Trail.enabled = isTrailEnable;
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

    public void ToggleTrail()
    {
        isTrailEnable = !isTrailEnable;
        Trail.enabled = isTrailEnable;
    }
}
