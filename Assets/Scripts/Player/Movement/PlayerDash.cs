using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerGrounded))]
[RequireComponent(typeof(PlayerFacing))]
public class PlayerDash : PlayerMovementBehaviour
{
    [SerializeField] private float DashForce;
    [SerializeField] private float DashDistance;
    [SerializeField] private float DashCooldown;

    public float CurrentDashCooldown { get; set; } = 0f;
    public bool  AirDashReady        { get; set; } = true;

    private Rigidbody2D    _RB;
    private PlayerMovement _playerMovement;
    private PlayerGrounded _playerGrounded;
    private PlayerFacing   _playerFacing;

    private bool       _isDashing     = false;
    private float      _dashStartXPos = 0f;
    private Vector2    _dashDirection = Vector2.zero;

    private Coroutine _dashCoroutine;

    private void Awake()
    {
        _RB             = GetComponent<Rigidbody2D>();
        _playerMovement = GetComponent<PlayerMovement>();
        _playerGrounded = GetComponent<PlayerGrounded>();
        _playerFacing   = GetComponent<PlayerFacing>();

        EventManager.StartListening(EventStrings.PLAYER_DASH_INTERRUPTED, OnDashInterrupt);
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (PlayerUnlocks.isDashUnlocked
            && CurrentDashCooldown <= 0
            && CanMove()
            && _isDashing == false
            && context.phase == InputActionPhase.Performed)
        {
            // Check Dash Requirements
            if (!_playerGrounded.IsGrounded && !AirDashReady)
            {
                return;
            }

            if (!_playerGrounded.IsGrounded && AirDashReady)
            {
                AirDashReady = false;
            }
            
            _isDashing = true;

            // Set the dash direction
            
            if (_playerFacing.PointingDirection == OrthogonalDirection.Left)
            {
                _dashDirection = Vector2.left;
            }

            if (_playerFacing.PointingDirection == OrthogonalDirection.Right)
            {
                _dashDirection = Vector2.right;
            }

            // Reset velocity
            _RB.velocity = Vector2.zero;
            PlayerMovementEventManager.TriggerEvent(PlayerMovementEvent.OnDashStart, null);

            // Dash Coroutine
            _dashCoroutine        = StartCoroutine(DashCoroutine());

            // Effects
            AudioManager.instance.PlaySound("dash");
            PlayerParticleManager.instance.PlayParticles(PlayerParticleName.DashParticles);

            // Set Cooldown
            CurrentDashCooldown = DashCooldown;
        }
    }

    private IEnumerator DashCoroutine()
    {
        // Lock Movement
        PlayerMovementLock.instance.LockMovement();
        _playerMovement.ReleaseInputs();
        _dashStartXPos = transform.position.x;

        // Lock Y position
        _RB.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        _isDashing = true;

        _RB.velocity = new Vector2(Mathf.Sign(_dashDirection.x) * DashForce, 0);

        yield return new WaitUntil(() => Mathf.Abs(transform.position.x - _dashStartXPos) >= DashDistance);

        InterruptDash();
    }

    private void OnDashInterrupt(System.Collections.Generic.Dictionary<string, object> package)
    {
        InterruptDash();
    }

    private void InterruptDash()
    {
        if (_isDashing)
        {
            if (_dashCoroutine != null) { StopCoroutine(_dashCoroutine); }

            // Unlock movement
            PlayerMovementLock.instance.UnlockMovement();
            _RB.velocity = Vector2.zero;

            // Unlock y position
            _RB.constraints = RigidbodyConstraints2D.FreezeRotation;
            _isDashing = false;

            // Signal dash end
            PlayerMovementEventManager.TriggerEvent(PlayerMovementEvent.OnDashEnd, null);
        }
    }

    private void Update()
    {
        if (CurrentDashCooldown > 0) 
        {
            CurrentDashCooldown -= Time.deltaTime;
        }

        if (_isDashing)
        {
            if (Mathf.Abs(_RB.velocity.x) < DashForce || Mathf.Abs(transform.position.x - _dashStartXPos) >= DashDistance)
            {
                InterruptDash();
            }
            else
            {
                _playerMovement.ReleaseInputs();
            }
        }

        if (_playerGrounded.IsGrounded)
        {
            AirDashReady = true;
        }
    }
}
