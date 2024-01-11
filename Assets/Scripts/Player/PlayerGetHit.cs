using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGetHit : MonoBehaviour
{
    private bool  _canGetHit = true;
    [SerializeField] private float _stunDuration = 0.25f;
    [SerializeField] private float _knockbackForce = 10.0f;
    [SerializeField] private float _invincibilityDuration = 1.5f;

    private PlayerHealth    _playerHealth;
    private PlayerKnockback _playerKnockback;

    public SpriteRenderer PlayerSprite;

    private void Awake()
    {
        _playerHealth    = GetComponent<PlayerHealth>();
        _playerKnockback = GetComponent<PlayerKnockback>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (_canGetHit == false)
            return;

        if (collision.gameObject.TryGetComponent(out DamagePlayerOnHit damagePlayerOnHit))
        {
            ApplyHit(damagePlayerOnHit.GetContactDamage(), collision.gameObject.transform.position);
        }
    }

    public void ApplyHit(int damage, Vector2 hitPosition, bool withKnockback=true)
    {
        if (_canGetHit == false)
            return;

        if (_playerHealth.currentHealth > 1)
        {
            if (withKnockback)
            {
                _playerKnockback.DoKnockback(_knockbackForce, _stunDuration, hitPosition);
            }

            StartCoroutine(ApplyIFrames(_invincibilityDuration));
        }

        _playerHealth.TakeDamage(damage);
    }

    private IEnumerator ApplyIFrames(float duration)
    {
        _canGetHit = false;
        PlayerSprite.color = new Color(1, 0, 0, 0.5f);

        yield return new WaitForSeconds(duration);

        _canGetHit = true;
        PlayerSprite.color = new Color(1, 1, 1, 1);
    }
}
