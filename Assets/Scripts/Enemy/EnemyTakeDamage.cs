using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class EnemyTakeDamage : MonoBehaviour
{
    public ParticleSystem damageParticles;
    public bool CanTakeDamage = true;
    public float knockbackMultiplier;

    private Rigidbody2D RB;

    private List<int> ringIds;

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        ringIds = new();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Ring ring))
        {
            if (CanTakeDamage == false) { return; }

            // Try to add new ring damage instance 
            if (CheckIfCanDamage(ring.gameObject))
            {
                ApplyDamage(ring);
                GetKnockedBack();
                GetComponent<Animator>().SetTrigger("hit");
                damageParticles.Play();
            }
        }
    }

    private void GetKnockedBack()
    {
        Vector2 knockbackDirection = transform.position - GameObject.FindGameObjectWithTag("Player").transform.position;

        if (TryGetComponent(out StunnedByRing stun))
        {
            stun.GetStunned();
        }
        else
        {
            RB.velocity = Vector2.zero;
        }

        if (knockbackMultiplier > 0)
            RB.AddForce(knockbackDirection.normalized * (Config.RingKnockbackForce * knockbackMultiplier), ForceMode2D.Impulse);
    }

    private bool CheckIfCanDamage(GameObject ring)
    {
        // Check if ring has alredy been added to damage instance list
        if (ringIds.Contains(ring.GetInstanceID()))
        {
            return false;
        }

        ringIds.Add(ring.GetInstanceID());
        return true;
        
    }

    private void ApplyDamage(Ring ring)
    {
        // Apply damage to enemy health
        int damageTaken = ring.GetRingDamage();
        if (TryGetComponent(out EnemyHealth health) && damageTaken > 0)
        {
            health.RemoveHealth(damageTaken);
        }
        else
        {
            Utility.PrintWarn("No damage was applied to [" + gameObject.name + "]");
        }

        if (TryGetComponent(out FlashEffect fe))
        {
            fe.DoFlashEffect();
        }
    }
}
