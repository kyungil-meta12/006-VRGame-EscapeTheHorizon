using UnityEngine;

public class Ammo : PoolObject
{
    private ParticleSystem particle;

    void Awake()
    {
        particle = GetComponent<ParticleSystem>();
    }

    public void Play()
    {
        particle.Simulate(0f, true, true);
        particle.Play();
    }

    void OnParticleSystemStopped()
    {
        ReturnInstance(gameObject);
    }
}
