using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGetHit : MonoBehaviour
{
    [SerializeField] private float _knockbackTime;
    [SerializeField] private float _knockbackForce;

    private bool _canGetHit = true;
    private float _invincibilityDuration = 1.5f;
    private PlayerKnockback _playerKnockback;

    public SpriteRenderer PlayerSprite;

    private void Awake()
    {
        _playerKnockback = GetComponent<PlayerKnockback>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (_canGetHit == false)
            return;

        if (collision.gameObject.TryGetComponent(out DamagePlayerOnHit damagePlayerOnHit))
        {
            // TODO: get damage and knockback info from the enemy 
            ApplyHit(damagePlayerOnHit.GetContactDamage(), collision.gameObject.transform.position);
        }
    }

    public void ApplyHit(int damage, Vector2 hitPosition, float force = 0, float time=0)
    {
        if (_canGetHit == false)
            return;

        if (force==0 && time==0)
        {
            EventManager.TriggerEvent(EventStrings.PLAYER_KNOCKED_BACK, new Dictionary<string, object> { 
                { "force", _knockbackForce }, { "time", _knockbackTime }, { "direction", (Vector2)transform.position - hitPosition } });
        }
        else
        {
            EventManager.TriggerEvent(EventStrings.PLAYER_KNOCKED_BACK, new Dictionary<string, object> {
                { "force", force }, { "time", time }, { "direction", (Vector2)transform.position - hitPosition } });
        }

        StartCoroutine(ApplyIFrames(_invincibilityDuration));
        EventManager.TriggerEvent(EventStrings.PLAYER_DAMAGED, new Dictionary<string, object> { { "amount", damage } });
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
