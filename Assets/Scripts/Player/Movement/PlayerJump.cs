using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerGrounded))]
public class PlayerJump : PlayerMovementBehaviour
{
    [Header("Gravity")]
    [SerializeField] private float GravityScale; //Strength of the player's gravity as a multiplier of gravity (set in ProjectSettings/Physics2D).

    [Space(5)]
    [SerializeField] private float FallGravityMultiplier; //Multiplier to the player's gravityScale when falling.
    [SerializeField] private float MaxFallSpeed; //Maximum fall speed (terminal velocity) of the player when falling.

    [Header("Jump")]
    [SerializeField] private float JumpForce; //The actual force applied (upwards) to the player when they jump.
    [SerializeField] private float JumpCutMultiplier; //Multiplier to increase gravity if the player releases thje jump button while still jumping

    private bool _isJumping; // are we in the middle of or falling from a jump
    private bool _isDoubleJumping; // are we in the middle of or falling from a double jump
    private bool _isJumpCut = false;

    [Header("Jump Assists")]
    [Range(0.01f, 0.5f)]
    [SerializeField] private float CoyoteTime; // Grace period after falling off a platform, where you are still grounded
    [Range(0.01f, 0.5f)]
    [SerializeField] private float JumpInputBufferTime; // Grace period after pressing jump, where you have still pressed jump

    private Rigidbody2D _RB;
    private PlayerGrounded _playerGrounded;

    // Timers
    private float _lastOnGroundTime;
    private float _lastPressedJumpTime;

    private void Awake()
    {
        _RB             = GetComponent<Rigidbody2D>();
        _playerGrounded = GetComponent<PlayerGrounded>();
    }

    public void JumpInputAction(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            OnJumpInput();
            CheckDoubleJump();
        }

        if (context.phase == InputActionPhase.Canceled)
        {
            OnJumpUpInput();
            JumpCutDoubleJump();
        }
    }

    private void Update()
    {
        UpdateTimers();
        CheckGrounded();
        
        // After jump, now falling = no longer jumping
        if (_isJumping && _RB.velocity.y < 0)
        {
            _isJumping = false;
        }

        // Falling, without jumping
        if (_lastOnGroundTime > 0 && !_isJumping)
        {
            _isJumpCut = false; // not a jump cut
        }

        // If can jump and jump was pressed recently
        if (CanJump() && _lastPressedJumpTime > 0)
        {
            _isJumping = true; // Is now jumping
            _isJumpCut = false; // Reset jump cut when new jump
            Jump(); // Do jump
        }

        ApplyGravity();
    }

    private void JumpCutDoubleJump()
    {
        if (_isDoubleJumping && _RB.velocity.y > 0)
        {
            _isJumpCut = true;
        }
    }

    private void UpdateTimers()
    {
        _lastOnGroundTime -= Time.deltaTime;
        _lastPressedJumpTime -= Time.deltaTime;
    }

    private void CheckGrounded()
    {
        if (!_isJumping)
        {
            // If touching ground
            if (_playerGrounded.IsGrounded)
            {
                // Reset grounded time, with coyote time tolerance
                _lastOnGroundTime = CoyoteTime;
                _isDoubleJumping = false;
            }
        }
    }

    private void CheckDoubleJump()
    {
        // jump unlocked, not grounded, not currently double jumping, jump input
        if (PlayerUnlocks.isDoubleJumpUnlocked && _lastOnGroundTime < 0 && !_isDoubleJumping && CanMove())
        {
            _isDoubleJumping = true; // Is now double jumping

            _isJumpCut = false; // Reset jump cut when new jump
            _RB.velocity = new Vector2(_RB.velocity.x, 0); // Zero the fall speed

            _RB.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse); // Apply Jump
            GetComponent<PlayerDash>().AirDashReady = true;
            PlayerParticleManager.instance.PlayParticles(PlayerParticleName.JumpParticles);
            AudioManager.instance.PlaySound("fluff");

            PlayerMovementEventManager.TriggerEvent(PlayerMovementEvent.OnDoubleJump, null);
        }
    }

    private void ApplyGravity()
    {
        if (_isJumpCut)
        {
            // When jump cut, gravity increases by jump cut grav mutliplier
            _RB.gravityScale = GravityScale * JumpCutMultiplier;
            _RB.velocity = new Vector2(_RB.velocity.x, Mathf.Max(_RB.velocity.y, -MaxFallSpeed));
        }
        else
        {
            // Apply normal gravity
            _RB.gravityScale = GravityScale;
        }
    }

    private void OnJumpInput()
    {
        // when jump is pressed
        // set last pressed jump time to the tolerance
        // during the tolerance time, a jump is always being "pressed"
        _lastPressedJumpTime = JumpInputBufferTime;
    }

    private void OnJumpUpInput()
    {
        // when jump is released, apply a jump cut
        if (CanJumpCut())
            _isJumpCut = true;
    }

    private void Jump()
    {
        if (!CanMove())
        {
            return;
        }

        // Ensures we can't call Jump multiple times from one press, remove all tolerances
        _lastPressedJumpTime = 0;
        _lastOnGroundTime = 0;

        // zero the fall speed
        _RB.velocity = new Vector2(_RB.velocity.x, 0);

        // Apply jump force
        _RB.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
        PlayerParticleManager.instance.PlayParticles(PlayerParticleName.JumpParticles);

        AudioManager.instance.PlaySound("fluff");
        PlayerMovementEventManager.TriggerEvent(PlayerMovementEvent.OnJump, null);
    }

    private bool CanJump()
    {
        // Can jump if on ground recently (tolerance), and not currently jumping
        return _lastOnGroundTime > 0 && !_isJumping;
    }

    private bool CanJumpCut()
    {
        // Can jump cut only when ascending 
        return _isJumping && _RB.velocity.y > 0;
    }
}
