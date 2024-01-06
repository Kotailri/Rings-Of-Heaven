using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHazardCollision : MonoBehaviour
{
    private Vector2 currentCheckpoint;
    private PlayerMovement pm;
    private PlayerGetHit hit;

    public Transform[] checkpointChecks;
    public LayerMask checkpointLayer;
    public Vector2 checkpointLayerCheck;

    private void Awake()
    {
        pm = GetComponent<PlayerMovement>();
        hit = GetComponent<PlayerGetHit>();
    }

    private void Start()
    {
        currentCheckpoint = transform.position;
    }

    private void FixedUpdate()
    {
        foreach (Transform t in checkpointChecks)
        {
            if (!Physics2D.OverlapBox(t.position, checkpointLayerCheck, 0, checkpointLayer))
            {
                return;
            }
        }

        currentCheckpoint = transform.position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        foreach (Transform t in checkpointChecks)
            Gizmos.DrawWireCube(t.position, checkpointLayerCheck);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("RespawnsPlayer"))
        {
            RespawnPlayer(collision.gameObject.transform);
        }

        if (collision.gameObject.CompareTag("Checkpoint"))
        {
            //currentCheckpoint = collision.gameObject.transform.position;
        }
    }

    private void RespawnPlayer(Transform other)
    {
        pm.ReleaseInputs();
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        if (GetComponent<PlayerHealth>().currentHealth > 1)
        {
            transform.position = currentCheckpoint;
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;

            PlayerMovementLock.instance.LockMovement(1);
            
            StartCoroutine(EnableMovement());
        }
        
        hit.ApplyHit(1, other.position, false);

    }

    private IEnumerator EnableMovement()
    {
        yield return new WaitForSecondsRealtime(1f);
        PlayerMovementLock.instance.UnlockMovement(1);
    }
}
