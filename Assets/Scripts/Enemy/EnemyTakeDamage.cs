using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTakeDamage : MonoBehaviour
{
    public bool CanTakeDamage = true;

    private List<string> ringIds = new();

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Ring ring))
        {
            if (CanTakeDamage == false) { return; }

            // Try to add new ring damage instance 
            CheckIfDamaged(ring.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Ring is no longer trying to damage, remove ring damage instance

        if (ringIds.Contains(collision.gameObject.GetInstanceID().ToString()))
        {
            ringIds.Remove(collision.gameObject.GetInstanceID().ToString());
        }

        if (ringIds.Contains(collision.gameObject.GetInstanceID().ToString() + "_RETURN"))
        {
            ringIds.Remove(collision.gameObject.GetInstanceID().ToString() + "_RETURN");
        }
    }

    private void CheckIfDamaged(GameObject ring)
    {
        // Check if ring has alredy been added to damage instance list
        if (ringIds.Contains(ring.GetInstanceID().ToString()))
        {
            return;
        }

        if (ring.TryGetComponent(out Ring returningRing)) 
        {
            if (returningRing is IRingReturn)
            {
                if (((IRingReturn)returningRing).IsReturning())
                {
                    // Check if returning ring has already been added to damage instance list
                    if (ringIds.Contains(ring.GetInstanceID().ToString() + "_RETURN"))
                    {
                        return;
                    }
                    else
                    {
                        ringIds.Add(ring.GetInstanceID().ToString() + "_RETURN");
                        ApplyDamage(ring);
                        return;
                    }
                }
            }
        }

        ringIds.Add(ring.GetInstanceID().ToString());
        ApplyDamage(ring);
    }

    private void ApplyDamage(GameObject _ring)
    {
        if (_ring.TryGetComponent(out Ring ring))
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
            
        }

        if (TryGetComponent(out FlashEffect fe))
        {
            fe.DoFlashEffect();
        }
    }
}
