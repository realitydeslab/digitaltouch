using UnityEngine;

public class ParticleControl_withEvent : BaseVFXControl_withEvent
{
    public ParticleSystem m_ParticleSystem;

    [Tooltip("Sensitivity")]
    public float scaleSensitivity = 10f;

    protected override void OnEnable()
    {
        base.OnEnable();
        m_ParticleSystem.Play();
        Debug.Log("Particle is enabled");

    }

    protected override void OnDisable()
    {
        m_ParticleSystem.Stop();
        base.OnDisable();
        Debug.Log("Particle is disabled");

    }

    public override void OnTipsRelationUpdated(float distance, Vector3 centerPosition)
    {
        if (distance <= 0.05)
        {
            m_ParticleSystem.Pause(true);
        }
        else
        {
            if (m_ParticleSystem.isPaused)
                m_ParticleSystem.Pause(false);
        }
        float speed = distance * scaleSensitivity;
        SetParticleStartSpeed(speed);

    }

    public void SetParticleStartSpeed(float speed)
        {
            var mainModule = m_ParticleSystem.main;
            mainModule.simulationSpeed = speed;
        }
}
