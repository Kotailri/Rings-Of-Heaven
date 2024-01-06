using System.Collections;
using UnityEngine;

public class PlayerKnockback : MonoBehaviour
{
    private Rigidbody2D RB;
    private PlayerMovement pm;

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        pm = GetComponent<PlayerMovement>();
    }

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

        RB.velocity = Vector2.zero;
        RB.AddForce(forceDirection * knockbackForce, ForceMode2D.Impulse);
    }

    private IEnumerator LockMovementTimer(float time)
    {
        PlayerMovementLock.instance.LockMovement();
        yield return new WaitForSeconds(time);
        PlayerMovementLock.instance.UnlockMovement();
    }
}
