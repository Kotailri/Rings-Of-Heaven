using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;

public class PlayerJump : MonoBehaviour
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
    [Header("Checks")]
    [SerializeField] private Transform _groundCheckPoint;
    [SerializeField] private Vector2 _groundCheckSize = new(0.49f, 0.03f);
    [SerializeField] private LayerMask _groundLayer;
    [HideInInspector] public bool isGrounded;

    [Space(15)]
    public ParticleSystem dustParticles;

    private Rigidbody2D RB;
    private bool canMove = true;

    // Timers
    private float lastOnGroundTime;
    private float lastPressedJumpTime;

    private Controls controls;

    private void Awake()
    {
        controls = new Controls();
        controls.Gameplay.Jump.started += ctx => OnJumpInput();
        controls.Gameplay.Jump.started += ctx => CheckDoubleJump();

        controls.Gameplay.Jump.canceled += ctx => OnJumpUpInput();
        controls.Gameplay.Jump.canceled += ctx => JumpCutDoubleJump();

        controls.Gameplay.Enable();

        RB = GetComponent<Rigidbody2D>();
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

        // jump cut if jump released while ascending from double jump
        //if (PlayerControls.GetJumpReleased() && isDoubleJumping && RB.velocity.y > 0)
        //{
        //    _isJumpCut = true;
        //}

        ApplyGravity();
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
            if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer))
            {
                // Reset grounded time, with coyote time tolerance
                lastOnGroundTime = coyoteTime;
                isDoubleJumping = false;
            }
        }

        if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    public void ToggleMovement(bool _canMove)
    {
        canMove = _canMove;
    }

    private void CheckDoubleJump()
    {
        // jump unlocked, not grounded, not currently double jumping, jump input
        if (PlayerUnlocks.isDoubleJumpUnlocked && lastOnGroundTime < 0 && !isDoubleJumping)
        {
            isDoubleJumping = true; // Is now double jumping

            _isJumpCut = false; // Reset jump cut when new jump
            RB.velocity = new Vector2(RB.velocity.x, 0); // Zero the fall speed

            RB.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse); // Apply Jump
            dustParticles.Play();
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
        if (!canMove)
            return;

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
        // Ensures we can't call Jump multiple times from one press, remove all tolerances
        lastPressedJumpTime = 0;
        lastOnGroundTime = 0;

        // zero the fall speed
        RB.velocity = new Vector2(RB.velocity.x, 0);

        // Apply jump force
        RB.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        dustParticles.Play();
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
    }
}
