using System.Collections;
using UnityEngine;

public class PlayerKnockback : MonoBehaviour
{
    private Rigidbody2D _RB;

    private void Awake()
    {
        _RB = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Add a knockback force to the player
    /// </summary>
    /// <param name="knockbackForce"></param>
    /// <param name="stunTime"></param>
    /// <param name="hitPosition"></param>
    public void DoKnockback(float knockbackForce, float stunTime, Vector2 hitPosition)
    {
        StartCoroutine(LockMovementTimer(stunTime));
        Vector2 forceDirection = new(0, 1);

        if (transform.position.x < hitPosition.x)
        {
            forceDirection = new Vector2(-2, 0.1f);
        }
        
        if (transform.position.x > hitPosition.x)
        {
            forceDirection = new Vector2(2, 0.1f);
        }

        _RB.velocity = Vector2.zero;
        _RB.AddForce(forceDirection * knockbackForce, ForceMode2D.Impulse);
    }

    private IEnumerator LockMovementTimer(float time)
    {
        PlayerMovementLock.instance.LockMovement();
        yield return new WaitForSeconds(time);
        PlayerMovementLock.instance.UnlockMovement();
    }
}
