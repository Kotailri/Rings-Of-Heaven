using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimControl : MonoBehaviour
{
    private Animator       _animator;
    private Rigidbody2D    _RB;
    private PlayerGrounded _playerGrounded;

    private void Awake()
    {
        _RB             = GetComponent<Rigidbody2D>();
        _animator       = GetComponent<Animator>();
        _playerGrounded = GetComponent<PlayerGrounded>();
    }

    private void Update()
    {
        if (_playerGrounded.IsGrounded)
        {
            _animator.SetInteger("isFlying", 0);

            if (Mathf.Abs(_RB.velocity.x) > 0.5)
            {
                _animator.SetBool("isWalking", true);
            }
            else
            {
                _animator.SetBool("isWalking", false);
            }
        }   
        else
        {
            if (_RB.velocity.y > 0.5)
            {
                _animator.SetInteger("isFlying", 1);
            }

            else if (_RB.velocity.y < -0.5)
            {
                _animator.SetInteger("isFlying", -1);
            }

            else
            {
                _animator.SetInteger("isFlying", 0);
            }
        }
    }
}
