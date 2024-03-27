using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

public enum EnemyState { None, Idle, Chase, Attack }

public class BasicEnemyStateController : MonoBehaviour, IEnemyController
{
    [SerializeField] private Vector2 _detectionArea;
    [SerializeField] private EnemyHitbox _hitbox;
    [SerializeField] private int _contactDamage;
    private EnemyState CurrentState = EnemyState.Idle;

    private EnemyHealth health;
    public IEnemyIdleState IdleState;
    public IEnemyChaseState ChaseState;
    public IEnemyAttackState AttackState;

    private LayerMask _playerLayerMask;
    private bool HasChaseState;
    private bool HasAttackState;

    private bool IsControllerActive = true;

    private void Awake()
    {
        _hitbox.SetHitboxDamage(_contactDamage);

        if (TryGetComponent(out IEnemyIdleState idleState))
        {
            IdleState = idleState;
        }
        else
        {
            Logger.PrintWarn("Enemy " + this.name + " is missing an idle state!");
            #if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
            #endif
        }

        if (TryGetComponent(out IEnemyChaseState chaseState))
        {
            ChaseState = chaseState;
            HasChaseState = true;
        }
        else
        {
            ChaseState = null;
            HasChaseState = false;
        }

        if (TryGetComponent(out IEnemyAttackState attackState))
        {
            AttackState = attackState;
            HasAttackState = true;
        }
        else
        {
            AttackState = null;
            HasAttackState = false;
        }

        _playerLayerMask = LayerMask.GetMask("Player");
    }

    public void PauseController(float time)
    {
        StartCoroutine(PauseControllerCoroutine(time));
    }

    private IEnumerator PauseControllerCoroutine(float time)
    {
        IsControllerActive = false;
        GetEnemyStateFromEnum(CurrentState).OnStatePaused();

        yield return new WaitForSeconds(time);

        IsControllerActive = true;
        GetEnemyStateFromEnum(CurrentState).OnStateResumed();
    }

    private void Update()
    {
        if (!IsControllerActive) { return; }

        EnemyState nextState = CheckStateChangeRequirements(CurrentState);
        ChangeStates(nextState);

        GetEnemyStateFromEnum(CurrentState).OnStateUpdate();
    }

    /// <summary>
    /// Converts EnemyState enum to IEnemyStateBehaviour
    /// </summary>
    /// <param name="stateEnum"></param>
    /// <returns></returns>
    private IEnemyBehaviourState GetEnemyStateFromEnum(EnemyState stateEnum)
    {
        switch (stateEnum)
        {
            case EnemyState.Idle:
                return IdleState;

            case EnemyState.Chase:
                return ChaseState;

            case EnemyState.Attack:
                return AttackState;
        }
        return null;
    }

    /// <summary>
    /// Returns a new state to change to, None if no change
    /// </summary>
    /// <param name="_currentState"></param>
    /// <returns></returns>
    private EnemyState CheckStateChangeRequirements(EnemyState _currentState)
    {
        // Check Attack State
        if (HasAttackState && 
            _currentState != EnemyState.Attack &&
            Physics2D.OverlapBox(transform.position, AttackState.GetAttackDetectionArea(), _playerLayerMask))
        {
            return EnemyState.Attack;
        }

        // Check Chase State
        if (HasChaseState && 
            _currentState != EnemyState.Chase &&
            Physics2D.OverlapBox(transform.position, _detectionArea, _playerLayerMask))
        {
            return EnemyState.Chase;
        }
        
        // Check Idle State
        if (_currentState != EnemyState.Idle)
        {
            return EnemyState.Idle;
        }

        // No State Change
        return EnemyState.None;
    }

    /// <summary>
    /// Changes state, calls state exit and enter functions
    /// </summary>
    /// <param name="newState"></param>
    private void ChangeStates(EnemyState newState)
    {
        if (CurrentState != newState && newState != EnemyState.None)
        {
            GetEnemyStateFromEnum(CurrentState).OnStateExit();

            CurrentState = newState;

            GetEnemyStateFromEnum(CurrentState).OnStateEnter();
        }
        
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, _detectionArea);
    }
}
