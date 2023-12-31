using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScoreCollision : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("ScoreAdder"))
        {
            Destroy(collision.gameObject);
            Managers.scoreManager.AddScore(1);
        }
    }
}
