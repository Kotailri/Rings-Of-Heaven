using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySecretSound : MonoBehaviour
{
    private bool activated = true;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (activated && collision.gameObject.CompareTag("Player"))
        {
            AudioManager.instance.PlaySound("mystery");
            activated = false;
        }
    }
}
