using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitbox : IFrameHitbox
{
    [SerializeField] private int _hitboxDamage = 0;

    public void SetHitboxDamage(int damage)
    {
        _hitboxDamage = damage;
    }

    private void Awake()
    {
        if (!Config.HitboxDebugMode)
        {
            GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Utility.IsOfTag(collision.gameObject, Tags.DamagesEnemy))
        {
            if (transform.parent.gameObject.TryGetComponent(out EnemyTakeDamage etd))
            {
                etd.GetHit(collision.gameObject.GetComponent<Ring>());
            }
        }

        if (_hitboxActive)
        {
            if (collision.gameObject.CompareTag("PlayerHitbox"))
            {
                EventManager.TriggerEvent(EventStrings.PLAYER_HIT, new Dictionary<string, object> {
                    { "hitPositionX", transform.position.x },
                    { "hitPositionY", transform.position.y },
                    { "hitDamage"  , _hitboxDamage }
                });
            }
        }
    }
}
