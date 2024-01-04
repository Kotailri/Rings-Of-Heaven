using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScoreCollision : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("ScoreAdder"))
        {
            Managers.scoreManager.AddScore(1);
            AudioManager.instance.PlaySound("blip");

            collision.gameObject.transform.position = Config.poolPosition;
        }
    }
}
