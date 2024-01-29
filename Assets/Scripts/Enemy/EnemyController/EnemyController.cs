using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Transform _playerTransformRef;
    private LayerMask _playerLayerMask;

    private Rigidbody2D _RB;

    [SerializeField] private EnemyIdle     _idleBehaviour;
    [SerializeField] private EnemyMovement _moveBehaviour;

    [Header("Movement")]
    [SerializeField] private float _detectionRange;
    [SerializeField] private float _detectionTime;

    private void Awake()
    {
        _playerTransformRef = GameObject.FindGameObjectWithTag("Player").transform;
        _playerLayerMask = LayerMask.GetMask("Player");

        _RB = GetComponent<Rigidbody2D>();

        _idleBehaviour = GetComponent<EnemyIdle>();
        _moveBehaviour = GetComponent<EnemyMovement>();

        StartCoroutine(IntializeComponents());
    }

    private IEnumerator IntializeComponents()
    {
        yield return new WaitUntil(() => _idleBehaviour != null);
        yield return new WaitUntil(() => _moveBehaviour != null);

        _idleBehaviour.SetRigidBody(_RB);
        _moveBehaviour.SetRigidBody(_RB);

        _idleBehaviour.StartIdleBehaviour();
        _moveBehaviour.IsMoving = false;

        InvokeRepeating(nameof(SwapMovementIdle), 0.5f, _detectionTime);
    }

    /// <summary>
    /// Swap between movement and idle behaviour based on player in range.
    /// </summary>
    private void SwapMovementIdle()
    {
        if (Physics2D.OverlapCircle(transform.position, _detectionRange, _playerLayerMask))
        {
            _idleBehaviour.StopIdleBehaviour();
            _moveBehaviour.ResumeMovement();
        }
        else
        {
            _idleBehaviour.StartIdleBehaviour();
            _moveBehaviour.StopMovement();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);
    }
}
