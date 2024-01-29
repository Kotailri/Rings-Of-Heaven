using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMoveTowardsPlayer : EnemyMovement
{
    [SerializeField] private float _movementSpeed;
    private Transform _playerTransformRef;

    private void Awake()
    {
        _playerTransformRef = GameObject.FindGameObjectWithTag("Player").transform;
    }

    public override void ResumeMovement()
    {
        IsMoving = true;
    }

    public override void StopMovement()
    {
        IsMoving= false;
    }

    private void Update()
    {
        if (IsMoving)
        {
            if (transform.position.x > _playerTransformRef.position.x)
            {
                _RB.velocity = new Vector2(-_movementSpeed, _RB.velocity.y);
            }
            else
            {
                _RB.velocity = new Vector2(_movementSpeed, _RB.velocity.y);
            }
        }
    }
}
