using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StartDirection
{
    Left,
    Right,
    Random
}

public class EnemyPatrol : MonoBehaviour, StunnedByRing
{
    public StartDirection startDirection;
    public float patrolSpeed;

    [Space(10)]
    [Header("Ledge Detection")]
    public Transform ledgeDetectLeft;
    public Transform ledgeDetectRight;

    [Space(10)]
    public Vector2 ledgeDetectSize;
    public LayerMask ledgeDetectLayer;

    [Space(10)]
    [Header("Wall Detection")]
    public Transform wallDetectLeft;
    public Transform wallDetectRight;

    [Space(10)]
    public Vector2 wallDetectSize;
    public LayerMask wallDetectLayer;

    [Space(10)]
    [Header("Ground Detection")]
    public Transform groundDetect;
    public Vector2 groundDetectSize;
    public LayerMask groundDetectLayer;

    private Rigidbody2D RB;
    private StartDirection currentDirection;

    private bool canMove = true;

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (startDirection == StartDirection.Random)
        {
            if (Random.Range(0,2) == 0)
            {
                startDirection = StartDirection.Left;
            }
            else
            {
                startDirection = StartDirection.Right;
            }
        }

        SetDirection(startDirection);
    }

    private void SetDirection(StartDirection direction)
    {
        switch (direction)
        {
            case StartDirection.Left:
                currentDirection = StartDirection.Left;
                RB.velocity = Vector2.left * patrolSpeed;
                break;

            case StartDirection.Right:
                currentDirection = StartDirection.Right;
                RB.velocity = Vector2.right * patrolSpeed;
                break;
        }
    }

    private void Update()
    {
        if (canMove == false)
        {
            return;
        }

        if (!CheckGrounded())
        {
            RB.velocity = new Vector2(0, RB.velocity.y);
            return;
        }
        else
        {
            if (RB.velocity.x == 0)
            {
                SetDirection(currentDirection);
            }
        }

        // Ledge detected on left side
        if (!Physics2D.OverlapBox(ledgeDetectLeft.position, ledgeDetectSize, 0, ledgeDetectLayer))
        {
            SetDirection(StartDirection.Right);
        }

        // Ledge detected on right side
        if (!Physics2D.OverlapBox(ledgeDetectRight.position, ledgeDetectSize, 0, ledgeDetectLayer))
        {
            SetDirection(StartDirection.Left);
        }

        // Wall detected on left side
        if (Physics2D.OverlapBox(wallDetectLeft.position, wallDetectSize, 0, wallDetectLayer))
        {
            SetDirection(StartDirection.Right);
        }

        // Wall detected on right side
        if (Physics2D.OverlapBox(wallDetectRight.position, wallDetectSize, 0, wallDetectLayer))
        {
            SetDirection(StartDirection.Left);
        }
    }

    private bool CheckGrounded()
    {
        return Physics2D.OverlapBox(groundDetect.position, groundDetectSize, 0, groundDetectLayer);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(ledgeDetectLeft.position, ledgeDetectSize);
        Gizmos.DrawWireCube(ledgeDetectRight.position, ledgeDetectSize);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(wallDetectLeft.position, wallDetectSize);
        Gizmos.DrawWireCube(wallDetectRight.position, wallDetectSize);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(groundDetect.position, groundDetectSize);
    }

    public void GetStunned()
    {

        if (canMove)
        {
            canMove = false;
            RB.velocity = Vector2.zero;
            Utility.InvokeLambda(() =>
            {
                RecoverMovement();
            }, 0.1f);
        }
        
    }

    private void RecoverMovement()
    {
        if (RB == null)
            return;

        if (!canMove)
        {
            RB.velocity = Vector2.zero;
            canMove = true;
            SetDirection(currentDirection);
        }
        
    }
}
