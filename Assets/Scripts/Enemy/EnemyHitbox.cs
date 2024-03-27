using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    private bool _hitboxActive = true;
    [SerializeField] private int _hitboxDamage = 0;

    public void SetHitboxDamage(int damage)
    {
        _hitboxDamage = damage;
    }

    private void OnEnable()
    {
        if (!Config.HitboxDebugMode)
        {
            GetComponent<SpriteRenderer>().enabled = false;
        }
        EventManager.StartListening(EventStrings.PLAYER_HIT, OnPlayerHit);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventStrings.PLAYER_HIT, OnPlayerHit);
    }

    private void OnPlayerHit(Dictionary<string, object> payload)
    {
        StartCoroutine(DisableHitboxTime());
    }

    private IEnumerator DisableHitboxTime()
    {
        _hitboxActive = false;
        yield return new WaitForSeconds(Config.PlayerIFrameTime);
        _hitboxActive = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ring"))
        {

        }

        if (_hitboxActive)
        {
            if (collision.gameObject.CompareTag("Player"))
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
