using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InflateSpeechBubble : MonoBehaviour
{
    public float speedIn;
    public float speedOut;

    public GameObject speechBubble;

    private Vector2 defaultSize;

    private void Start()
    {
        defaultSize = speechBubble.transform.localScale;
        speechBubble.transform.localScale = Vector2.zero;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Inflate();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Deflate();
        }
    }

    public void Inflate()
    {
        LeanTween.scale(speechBubble, defaultSize, speedIn);
    }

    public void Deflate()
    {
        LeanTween.scale(speechBubble, Vector2.zero, speedOut);
    }
}
