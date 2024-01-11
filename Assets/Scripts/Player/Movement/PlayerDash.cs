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
    [SerializeField] private float DashTime;
    [SerializeField] private float DashCooldown;

    public float CurrentDashCooldown { get; set; } = 0f;
    public bool  AirDashReady        { get; set; } = true;

    [Header("Particles")]
    public GameObject AfterImage;

    private Rigidbody2D    _RB;
    private PlayerMovement _playerMovement;
    private PlayerGrounded _playerGrounded;
    private PlayerFacing   _playerFacing;

    private bool    _isDashing     = false;
    private Vector2 _dashDirection = Vector2.zero;

    private void Awake()
    {
        _RB             = GetComponent<Rigidbody2D>();
        _playerMovement = GetComponent<PlayerMovement>();
        _playerGrounded = GetComponent<PlayerGrounded>();
        _playerFacing   = GetComponent<PlayerFacing>();
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (PlayerUnlocks.isDashUnlocked
            && CurrentDashCooldown <= 0
            && CanMove()
            && _isDashing == false
            && context.phase == InputActionPhase.Started)
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

            // Dash Coroutines
            StartCoroutine(DashCoroutine());
            StartCoroutine(DashEffects());

            // Set Cooldown
            CurrentDashCooldown = DashCooldown;
        }
    }

    private IEnumerator DashCoroutine()
    {
        // Lock Movement
        PlayerMovementLock.instance.LockMovement();
        _playerMovement.ReleaseInputs();

        // Lock Y position
        _RB.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        _isDashing = true;
        
        yield return new WaitForSecondsRealtime(DashTime);

        // Unlock movement
        PlayerMovementLock.instance.UnlockMovement();
        _RB.velocity = Vector2.zero;

        // Unlock y position
        _RB.constraints = RigidbodyConstraints2D.FreezeRotation;
        _isDashing = false;

        PlayerMovementEventManager.TriggerEvent(PlayerMovementEvent.OnDashEnd, null);
    }

    private IEnumerator DashEffects()
    {
        AudioManager.instance.PlaySound("dash");

        // Set the after image rotation
        Quaternion rotation = Quaternion.identity;
        if (_playerFacing.PointingDirection == OrthogonalDirection.Left)
        {
            rotation = Quaternion.Euler(0, 180f, 0);
        }

        // Create after images and play particle effects
        Instantiate(AfterImage, transform.position, rotation);
        PlayerParticleManager.instance.PlayParticles(PlayerParticleName.DashParticles);

        yield return new WaitForSecondsRealtime(DashTime / 3f);

        PlayerParticleManager.instance.PlayParticles(PlayerParticleName.DashParticles);
        Instantiate(AfterImage, transform.position, rotation);

        yield return new WaitForSecondsRealtime(DashTime / 3f);

        Instantiate(AfterImage, transform.position, rotation);
    }

    private void Update()
    {
        if (CurrentDashCooldown > 0) 
        {
            CurrentDashCooldown -= Time.deltaTime;
        }

        if (_isDashing)
        {
            _playerMovement.ReleaseInputs();
            _RB.velocity = Vector2.zero;
            _RB.velocity = new Vector2(Mathf.Sign(_dashDirection.x) * DashForce, 0);
        }

        if (_playerGrounded.IsGrounded)
        {
            AirDashReady = true;
        }
    }
}
