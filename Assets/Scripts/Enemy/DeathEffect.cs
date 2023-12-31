using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathEffect : MonoBehaviour
{
    public GameObject deathEffectObject;

    public void DoDeathEffect()
    {
        Instantiate(deathEffectObject, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
