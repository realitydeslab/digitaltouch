using UnityEngine;

public class RibbonControl : MonoBehaviour
{
    public ParticleSystem Ribbon1;
    public ParticleSystem Ribbon2;

    public void OnEnable()
    {
        Ribbon1.Play();
        Ribbon2.Play();
    }

    public void OnDisable()
    {
        Ribbon1.Stop();
        Ribbon2.Stop();
    }
}
