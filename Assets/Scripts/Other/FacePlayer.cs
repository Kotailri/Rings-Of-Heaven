using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    public PlayerRBFacingDirection initialFacingDirection;

    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void FixedUpdate()
    {
        if (initialFacingDirection == PlayerRBFacingDirection.Right)
        {
            if (transform.position.x > player.position.x) // me | player
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }

        else if (initialFacingDirection == PlayerRBFacingDirection.Left)
        {
            if (transform.position.x < player.position.x) // player | me
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }
}
