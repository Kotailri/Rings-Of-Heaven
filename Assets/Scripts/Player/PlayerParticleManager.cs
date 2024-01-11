using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerParticle
{
    public PlayerParticleName ParticleName;
    public ParticleSystem Particle;
}

public enum PlayerParticleName
{
    DashParticles,
    TurnParticles,
    JumpParticles
}

public class PlayerParticleManager : MonoBehaviour
{
    public static PlayerParticleManager instance;
    public List<PlayerParticle> Particles = new();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public ParticleSystem PlayParticles(PlayerParticleName _particleName)
    {
        foreach (PlayerParticle p in Particles)
        {
            if (_particleName == p.ParticleName)
            {
                p.Particle.Play();
                return p.Particle;
            }
        }
        return null;
    }
}
