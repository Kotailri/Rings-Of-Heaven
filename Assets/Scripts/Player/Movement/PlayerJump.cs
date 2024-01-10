using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJump : PlayerMovementBehaviour
{
    [Space(10)]
    [Header("Gravity")]
    public float gravityScale; //Strength of the player's gravity as a multiplier of gravity (set in ProjectSettings/Physics2D).

    [Space(5)]
    public float fallGravityMult; //Multiplier to the player's gravityScale when falling.
    public float maxFallSpeed; //Maximum fall speed (terminal velocity) of the player when falling.

    [Space(10)]
    [Header("Jump")]
    private bool isJumping;
    private bool isDoubleJumping; // are we in the middle of or falling from a double jump
    private bool _isJumpCut = false;

    public float jumpForce; //The actual force applied (upwards) to the player when they jump.
    public float jumpCutGravityMult; //Multiplier to increase gravity if the player releases thje jump button while still jumping

    [Space(10)]
    [Header("Jump Assists")]
    [Range(0.01f, 0.5f)] public float coyoteTime; //Grace period after falling off a platform, where you are still grounded
    [Range(0.01f, 0.5f)] public float jumpInputBufferTime; //Grace period after pressing jump, where you have still pressed jump


    [Space(15)]
    public ParticleSystem dustParticles;

    private Rigidbody2D RB;
    private PlayerGrounded grounded;

    private float _fallSpeedYDampingChangeThreshold;

    // Timers
    private float lastOnGroundTime;
    private float lastPressedJumpTime;

    private void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        grounded = GetComponent<PlayerGrounded>();
        _fallSpeedYDampingChangeThreshold = CameraManager.instance._fallSpeedYDampingChangeThreshold;
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
        if (isJumping && RB.velocity.y < 0)
        {
            isJumping = false;
        }

        // Falling, without jumping
        if (lastOnGroundTime > 0 && !isJumping)
        {
            _isJumpCut = false; // not a jump cut
        }

        // If can jump and jump was pressed recently
        if (CanJump() && lastPressedJumpTime > 0)
        {
            isJumping = true; // Is now jumping
            _isJumpCut = false; // Reset jump cut when new jump
            Jump(); // Do jump
        }

        CheckFallCameraChange();
        ApplyGravity();
    }

    private void CheckFallCameraChange()
    {
        // if falling past certain speed threshold
        if (RB.velocity.y < _fallSpeedYDampingChangeThreshold && !CameraManager.instance.IsLerpingYDamping && !CameraManager.instance.LerpedFromPlayerFalling)
        {
            CameraManager.instance.LerpYDamping(true);
        }

        // if standing still or moving up
        if (RB.velocity.y >= 0f && !CameraManager.instance.IsLerpingYDamping && CameraManager.instance.LerpedFromPlayerFalling)
        {
            // reset so it can be called again
            CameraManager.instance.LerpedFromPlayerFalling = false;
            CameraManager.instance.LerpYDamping(false);
        }
    }

    private void JumpCutDoubleJump()
    {
        if (isDoubleJumping && RB.velocity.y > 0)
        {
            _isJumpCut = true;
        }
    }

    private void UpdateTimers()
    {
        lastOnGroundTime -= Time.deltaTime;
        lastPressedJumpTime -= Time.deltaTime;
    }

    private void CheckGrounded()
    {
        if (!isJumping)
        {
            // If touching ground
            if (grounded.isGrounded)
            {
                // Reset grounded time, with coyote time tolerance
                lastOnGroundTime = coyoteTime;
                isDoubleJumping = false;
            }
        }
    }

    private void CheckDoubleJump()
    {
        // jump unlocked, not grounded, not currently double jumping, jump input
        if (PlayerUnlocks.isDoubleJumpUnlocked && lastOnGroundTime < 0 && !isDoubleJumping && CanMove())
        {
            isDoubleJumping = true; // Is now double jumping

            _isJumpCut = false; // Reset jump cut when new jump
            RB.velocity = new Vector2(RB.velocity.x, 0); // Zero the fall speed

            RB.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse); // Apply Jump
            GetComponent<PlayerDash>().AirDashReady = true;
            dustParticles.Play();
            AudioManager.instance.PlaySound("fluff");
        }
    }

    private void ApplyGravity()
    {
        if (_isJumpCut)
        {
            // When jump cut, gravity increases by jump cut grav mutliplier
            RB.gravityScale = gravityScale * jumpCutGravityMult;
            RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -maxFallSpeed));
        }
        else
        {
            // Apply normal gravity
            RB.gravityScale = gravityScale;
        }
    }

    private void OnJumpInput()
    {
        // when jump is pressed
        // set last pressed jump time to the tolerance
        // during the tolerance time, a jump is always being "pressed"
        lastPressedJumpTime = jumpInputBufferTime;
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
        lastPressedJumpTime = 0;
        lastOnGroundTime = 0;

        // zero the fall speed
        RB.velocity = new Vector2(RB.velocity.x, 0);

        // Apply jump force
        RB.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        dustParticles.Play();

        AudioManager.instance.PlaySound("fluff");
    }

    private bool CanJump()
    {
        // Can jump if on ground recently (tolerance), and not currently jumping
        return lastOnGroundTime > 0 && !isJumping;
    }

    private bool CanJumpCut()
    {
        // Can jump cut only when ascending 
        return isJumping && RB.velocity.y > 0;
    }
}
