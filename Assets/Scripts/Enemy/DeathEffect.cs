using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathEffect : MonoBehaviour
{
    public GameObject deathEffectObject;
    public List<GameObject> onDeathDrop = new();

    [Space(10)]
    public GameObject chanceDrop;
    public int percentChangeDrop;

    public void DoDeathEffect()
    {
        Instantiate(deathEffectObject, transform.position, Quaternion.identity);
        Instantiate(onDeathDrop[Random.Range(0, onDeathDrop.Count)], transform.position, Quaternion.identity);

        if (percentChangeDrop > Random.Range(0,100))
        {
            Instantiate(chanceDrop, transform.position, Quaternion.identity);
        }
    }
}
