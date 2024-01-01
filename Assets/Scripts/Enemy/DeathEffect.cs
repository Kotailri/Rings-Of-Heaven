using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathEffect : MonoBehaviour
{
    public GameObject deathEffectObject;
    public GameObject onDeathDrop;

    public void DoDeathEffect()
    {
        Instantiate(deathEffectObject, transform.position, Quaternion.identity);
        Instantiate(onDeathDrop, transform.position, Quaternion.identity);
    }
}
