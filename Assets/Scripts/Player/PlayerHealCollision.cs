using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealCollision : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Heal"))
        {
            Destroy(collision.gameObject);
            GetComponent<PlayerHealth>().Heal(1);
        }
    }
}
