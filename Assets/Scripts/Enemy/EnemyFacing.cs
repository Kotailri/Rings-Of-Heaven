using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public enum EnemyFacingMode
{
    LookAtPlayer,
    LookAtMovingDirection
}

public enum LeftRightFacing
{
    Left,
    Right
}

public class EnemyFacing : MonoBehaviour
{
    public LeftRightFacing DefaultFacingDirection;
    public EnemyFacingMode _EnemyFacingMode;

    private SpriteRenderer _spriteRenderer;
    private GameObject _playerObject;
    private Rigidbody2D _RB;

    private bool CanChangeFacingDirection = true;

    private void Awake()
    {
        _RB = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _playerObject = GameObject.FindGameObjectWithTag("Player");
    }

    public void PauseEnemyFacing(float time)
    {
        StartCoroutine(PauseEnemyFacingCoroutine(time));
    }

    private IEnumerator PauseEnemyFacingCoroutine(float time)
    {
        CanChangeFacingDirection = false;
        yield return new WaitForSeconds(time);
        CanChangeFacingDirection = true;
    }

    private void Update()
    {
        if (!CanChangeFacingDirection)
            return;

        if (_EnemyFacingMode == EnemyFacingMode.LookAtPlayer)
        {
            // should face right
            if (_playerObject.transform.position.x > transform.position.x) 
            {
                if (DefaultFacingDirection == LeftRightFacing.Left)
                {
                    _spriteRenderer.flipX = true;
                }
                else
                {
                    _spriteRenderer.flipX = false;
                }
            }
            // should face left
            else
            {
                if (DefaultFacingDirection == LeftRightFacing.Right)
                {
                    _spriteRenderer.flipX = true;
                }
                else
                {
                    _spriteRenderer.flipX = false;
                }
            }
        }

        if(_EnemyFacingMode == EnemyFacingMode.LookAtMovingDirection)
        {
            // moving right
            if (_RB.velocity.x > 0) 
            {
                if (DefaultFacingDirection == LeftRightFacing.Left)
                {
                    _spriteRenderer.flipX = true;
                }
                else
                {
                    _spriteRenderer.flipX = false;
                }
            }
            // moving left
            else
            {
                if (DefaultFacingDirection == LeftRightFacing.Right)
                {
                    _spriteRenderer.flipX = true;
                }
                else
                {
                    _spriteRenderer.flipX = false;
                }
            }
        }
    }
}
