using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGetHit : MonoBehaviour
{
    private bool canGetHit = true;

    private float stunDuration = 0.25f;
    private float knockbackForce = 100.0f;
    private float invincibilityDuration = 1.0f;

    private PlayerHealth health;
    private PlayerKnockback knockback;
    public SpriteRenderer playerSprite;

    private void Awake()
    {
        health = GetComponent<PlayerHealth>();
        knockback = GetComponent<PlayerKnockback>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (canGetHit == false)
            return;

        if (collision.gameObject.TryGetComponent(out DamagePlayerOnHit damagePlayerOnHit))
        {
            ApplyHit(damagePlayerOnHit.GetContactDamage(), collision.gameObject.transform.position);
        }
    }

    private void ApplyHit(int damage, Vector2 hitPosition)
    {
        health.TakeDamage(damage);
        knockback.DoKnockback(knockbackForce, stunDuration, hitPosition);
        StartCoroutine(ApplyIFrames(invincibilityDuration));
    }

    private IEnumerator ApplyIFrames(float duration)
    {
        canGetHit = false;
        playerSprite.color = new Color(playerSprite.color.r, playerSprite.color.g, playerSprite.color.b, 0.5f);

        yield return new WaitForSeconds(duration);

        canGetHit = true;
        playerSprite.color = new Color(playerSprite.color.r, playerSprite.color.g, playerSprite.color.b, 1);
    }
}
