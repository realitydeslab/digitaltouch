using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class RainControl : BaseAffordanceControl
{
    public List<ParticleSystem> m_ParticleSystems;
    public List<VisualEffect> m_VFX;

    [Tooltip("Sensitivity")]
    public float scaleSensitivity = 10f;

    public RTPCController rTPC;
    public PlaySound playSound;

    protected override void OnEnable()
    {
        foreach (var particle in m_ParticleSystems)
        {
            particle.gameObject.SetActive(true);
            particle.Play();
        }
        foreach (var vfx in m_VFX)
        {
            vfx.enabled = true;
            vfx.Play();
        }
        base.OnEnable();

        playSound.TriggerWwiseSound();
    }

    protected override void OnDisable()
    {
        playSound.StopWwiseSound();
        foreach (var particle in m_ParticleSystems)
        {
            particle.Stop();
            particle.gameObject.SetActive(false);
        }
        foreach (var vfx in m_VFX)
        {
            vfx.Stop();
            vfx.enabled = false;
        }
        base.OnDisable();
    }


    public override void OnTipRelationDataUpdated(float distance, Vector3 centerPosition)
    {
        float speed = distance * scaleSensitivity;
        SetParticleStartSpeed(speed);

        if (rTPC != null)
        {
            rTPC.SetRTPCValue(distance);
        }
    }

    public override void OnCameraRelationDataUpdated(float distance, Vector3 centerPosition)
    {
        return;
    }
    
    public void SetParticleStartSpeed(float speed)
    {
        foreach (var particle in m_ParticleSystems)
        {
            var mainModule = particle.main;
            mainModule.simulationSpeed = speed;
        }
    }
}