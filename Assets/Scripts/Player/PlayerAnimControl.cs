using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimControl : MonoBehaviour
{
    private Animator anim;

    private Rigidbody2D RB;
    private PlayerMovement pm;
    private PlayerJump jump;

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        pm = GetComponent<PlayerMovement>();
        jump = GetComponent<PlayerJump>();
    }

    private void Update()
    {
        if (jump.isGrounded)
        {
            if (Mathf.Abs(pm.GetMoveInputX()) > 0)
            {
                anim.SetBool("isWalking", true);
            }
            else
            {
                anim.SetBool("isWalking", false);
            }
        }   
        else
        {
            print(RB.velocity.y);
            if (RB.velocity.y > 0.5)
            {
                anim.SetInteger("isFlying", 1);
            }

            else if (RB.velocity.y < -0.5)
            {
                anim.SetInteger("isFlying", -1);
            }

            else
            {
                anim.SetInteger("isFlying", 0);
            }
        }
    }
}
