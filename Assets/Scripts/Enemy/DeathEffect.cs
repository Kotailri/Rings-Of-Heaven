using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathEffect : MonoBehaviour
{
    public GameObject deathEffectObject;
    public List<GameObject> onDeathDrop = new();

    public void DoDeathEffect()
    {
        Instantiate(deathEffectObject, transform.position, Quaternion.identity);

        if (GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>().currentHealth < 10)
        {
            Instantiate(onDeathDrop[Random.Range(0, onDeathDrop.Count)], transform.position, Quaternion.identity);
        }
        
    }
}
