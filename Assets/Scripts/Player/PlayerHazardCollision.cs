using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHazardCollision : MonoBehaviour
{
    private Vector2 currentCheckpoint;
    private PlayerMovement pm;
    private PlayerGetHit hit;

    private void Awake()
    {
        pm = GetComponent<PlayerMovement>();
        hit = GetComponent<PlayerGetHit>();
    }

    private void Start()
    {
        currentCheckpoint = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("RespawnsPlayer"))
        {
            RespawnPlayer(collision.gameObject.transform);
        }

        if (collision.gameObject.CompareTag("Checkpoint"))
        {
            currentCheckpoint = collision.gameObject.transform.position;
        }
    }

    private void RespawnPlayer(Transform other)
    {
        pm.ReleaseInputs();
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        if (GetComponent<PlayerHealth>().currentHealth > 1)
        {
            transform.position = currentCheckpoint;

            pm.ToggleMovement(false);
            
            StartCoroutine(EnableMovement());
        }
        
        hit.ApplyHit(1, other.position, false);

    }

    private IEnumerator EnableMovement()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        pm.ToggleMovement(true);
    }
}
