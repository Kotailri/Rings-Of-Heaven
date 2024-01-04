using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToPlayerOnDelay : MonoBehaviour
{
    public float delay;
    public float movespeed;

    private float currentTime = 0f;
    private Transform player;

    // Start is called before the first frame update
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (currentTime >= delay)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, movespeed * Time.deltaTime);
        }
        else
        {
            currentTime += Time.deltaTime;
        }
    }
}
