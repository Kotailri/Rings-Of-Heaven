using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathEffectExplode : MonoBehaviour, IDeathEffect
{
    public GameObject ExplosionParticles;

    public void DoDeathEffect()
    {
        Instantiate(ExplosionParticles, transform.position, Quaternion.identity);
    }
}
