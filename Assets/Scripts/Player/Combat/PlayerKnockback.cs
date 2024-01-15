using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKnockback : MonoBehaviour
{
    private Rigidbody2D _RB;

    private float   _currentKnockbackTime;
    private float   _knoockbackTime;
    private float   _knockbackForce;
    private Vector2 _knockbackDirection;

    private void Awake()
    {
        _RB = GetComponent<Rigidbody2D>();
        EventManager.StartListening(EventStrings.PLAYER_KNOCKED_BACK, OnPlayerKnockback);
    }

    private void OnDisable()
    {
        EventManager.StopListening(EventStrings.PLAYER_KNOCKED_BACK, OnPlayerKnockback);
    }

    /// <summary>
    /// Knockback through event manager
    /// </summary>
    /// <param name="force"></param>
    /// <param name="time"></param>
    /// <param name="direction"></param>
    private void OnPlayerKnockback(Dictionary<string, object> payload)
    {
        DoKnockback((float)payload["force"], (float)payload["time"], (Vector2)payload["direction"]);
    }

    private void Update()
    {
        if (_currentKnockbackTime < _knoockbackTime)
        {
            _RB.velocity = _knockbackDirection.normalized * _knockbackForce;
            _currentKnockbackTime += Time.deltaTime;
        }
    }

    /// <summary>
    /// Starts a knockback force on the player
    /// </summary>
    /// <param name="knockbackForce"></param>
    /// <param name="knockbackTime"></param>
    /// <param name="knockbackDirection"></param>
    private void DoKnockback(float knockbackForce, float knockbackTime, Vector2 knockbackDirection)
    {
        EventManager.TriggerEvent(EventStrings.PLAYER_DASH_INTERRUPTED, null);

        _knoockbackTime = knockbackTime;
        _knockbackForce = knockbackForce;
        _knockbackDirection = knockbackDirection;

        _RB.velocity = Vector2.zero;
        _currentKnockbackTime = 0;

        StartCoroutine(LockMovementTimer());
    }

    private IEnumerator LockMovementTimer()
    {
        PlayerMovementLock.instance.LockMovement();
        yield return new WaitForSeconds(_knoockbackTime);
        PlayerMovementLock.instance.UnlockMovement();
    }
}
