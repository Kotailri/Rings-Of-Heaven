using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreAdderOnCollision : MonoBehaviour
{
    public int ScoreValue;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            EventManager.TriggerEvent(EventStrings.SCORE_ADDED, new Dictionary<string, object> { { "score", ScoreValue } });
            AudioManager.instance.PlaySound("blip");

            transform.position = Config.poolPosition;
        }
    }
}
