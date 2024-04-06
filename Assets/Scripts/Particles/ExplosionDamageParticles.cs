using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionDamageParticles : MonoBehaviour
{
    public int HitDamage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            EventManager.TriggerEvent(EventStrings.PLAYER_HIT, new Dictionary<string, object>() { 
                { "hitPositionX", transform.position.x }, { "hitPositionY", transform.position.y }, { "hitDamage", HitDamage } 
            });
        }
    }
}
