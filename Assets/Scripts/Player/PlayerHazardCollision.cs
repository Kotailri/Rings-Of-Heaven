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

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("RespawnsPlayer"))
        {
            RespawnPlayer(collision.gameObject.transform);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Checkpoint"))
        {
            currentCheckpoint = collision.gameObject.transform.position;
        }
    }

    private void RespawnPlayer(Transform other)
    {
        if (GetComponent<PlayerHealth>().currentHealth == 1)
        {
            GetComponent<PlayerHealth>().Die();
            return;
        }

        transform.position = currentCheckpoint;

        pm.ToggleMovement(false);
        hit.ApplyHit(1, other.position, false);
        StartCoroutine(EnableMovement());   
    }

    private IEnumerator EnableMovement()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        pm.ToggleMovement(true);
    }
}
