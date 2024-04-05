using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowChaseState : MonoBehaviour, IEnemyChaseState
{
    public float ChaseSpeed;
    private Transform _playerTransform;
    private Rigidbody2D RB;
    private bool _isActive = true;

    private void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        _playerTransform = Utility.GetPlayer().transform;
    }

    public void OnStateEnter()
    {
        
    }

    public void OnStateExit()
    {
        RB.velocity = new Vector2(0, RB.velocity.y);
    }

    public void OnStatePaused()
    {
        RB.velocity = new Vector2(0, RB.velocity.y);
        _isActive = false;
    }

    public void OnStateResumed()
    {
        _isActive = true;
    }

    public void OnStateUpdate()
    {
        if (!_isActive) { return; }

        float closeEnough = 1.0f;

        // should move right
        if (transform.position.x < _playerTransform.position.x - closeEnough)
        {
            RB.velocity = new Vector2(ChaseSpeed, RB.velocity.y);
        }
        // should move left
        else if (transform.position.x > _playerTransform.position.x + closeEnough)
        {
            RB.velocity = new Vector2(-ChaseSpeed, RB.velocity.y);
        }
        // should stay still
        else
        {
            RB.velocity = new Vector2(0, RB.velocity.y);
        }
    }
}
