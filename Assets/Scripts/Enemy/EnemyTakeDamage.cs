using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class EnemyTakeDamage : MonoBehaviour
{
    public bool CanTakeDamage = true;

    private List<int> ringIds = new();

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Ring ring))
        {
            if (CanTakeDamage == false) { return; }

            // Try to add new ring damage instance 
            if (CheckIfCanDamage(ring.gameObject))
            {
                ApplyDamage(ring);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Ring is no longer trying to damage, remove ring damage instance

        if (ringIds.Contains(collision.gameObject.GetInstanceID()))
        {
            ringIds.Remove(collision.gameObject.GetInstanceID());
        }
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
