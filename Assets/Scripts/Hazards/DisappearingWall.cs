using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DisappearingWall : MonoBehaviour
{
    private float alpha;

    private void Start()
    {
        alpha = GetComponent<Renderer>().material.color.a;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            LeanTween.alpha(gameObject, 0.1f, 0.5f);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            LeanTween.alpha(gameObject, alpha, 0.5f);
        }
    }
}
