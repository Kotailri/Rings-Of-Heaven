using System.Collections;
using System.Collections.Generic;
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

    [Space(10f)]
    public bool FlipsObject;
    public bool _isFlipped = false;

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

    private void FlipX(bool flip)
    {
        if (flip == _isFlipped) { return; }

        _isFlipped = flip;

        if (FlipsObject)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

        }
        else
        {
            _spriteRenderer.flipX = flip;
        }
        
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
                    FlipX(true);
                }
                else
                {
                    FlipX(false);
                }
            }
            // should face left
            else
            {
                if (DefaultFacingDirection == LeftRightFacing.Right)
                {
                    FlipX(true);
                }
                else
                {
                    FlipX(false);
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
                    FlipX(true);
                }
                else
                {
                    FlipX(false);
                }
            }
            // moving left
            else
            {
                if (DefaultFacingDirection == LeftRightFacing.Right)
                {
                    FlipX(true);
                }
                else
                {
                    FlipX(false);
                }
            }
        }
    }
}
