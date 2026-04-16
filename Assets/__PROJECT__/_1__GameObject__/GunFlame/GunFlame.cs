using UnityEngine;

public class GunFlame : PoolObject
{
    private ParticleSystem particle;

    void Awake()
    {
        particle = GetComponent<ParticleSystem>();
    }

    void OnParticleSystemStopped()
    {
        ReturnInstance(gameObject);
    }

    public void Play()
    {
        particle.Simulate(0f, true, true);
        particle.Play();
    }
}