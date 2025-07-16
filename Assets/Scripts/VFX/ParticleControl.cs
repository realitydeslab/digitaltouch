using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ParticleControl : BaseVFXControl
{
    public List<ParticleSystem> m_ParticleSystems;

    [Tooltip("Sensitivity")]
    public float scaleSensitivity = 10f;

    public RTPCController rTPC;

    protected override void OnEnable()
    {
        base.OnEnable();
        foreach (var particle in m_ParticleSystems)
        {
            particle.Play();
        }
    }

    protected override void OnDisable()
    {
        foreach (var particle in m_ParticleSystems)
        {
            particle.Stop();
        }
        base.OnDisable();
    }

    public override void OnRelationDataUpdated(float distance, Vector3 centerPosition)
    {
        // if (distance <= 0.05)
        // {
        //     m_ParticleSystem.Pause(true);
        // }
        // else
        // {
        //     if (m_ParticleSystem.isPaused)
        //         m_ParticleSystem.Pause(false);
        // }
        float speed = distance * scaleSensitivity;
        SetParticleStartSpeed(speed);

        if (rTPC != null)
        {
            rTPC.SetRTPCValue(distance);
        }
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