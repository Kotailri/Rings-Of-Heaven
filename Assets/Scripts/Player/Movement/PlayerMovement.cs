using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerRBFacingDirection
{
    Right, Left
}

public class PlayerMovement : MonoBehaviour
{
    [Header("Run")]
    public float runMaxSpeed; //Target speed we want the player to reach.
    public float runAccelAmount; //The actual force (multiplied with speedDiff) applied to the player.
    public float runDeccelAmount; //Actual force (multiplied with speedDiff) applied to the player .

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

    private Rigidbody2D RB;
    [HideInInspector]
    public PlayerRBFacingDirection facing = PlayerRBFacingDirection.Right;
    private Vector2 moveInput;

    // Timers
    private float lastOnGroundTime;
    private float lastPressedJumpTime;

    [Header("TEMP DEBUG")]
    public bool OnIce;
    public float slipperyness;
    public float iceSpeedMultiplier;

    private bool canMove = true;
    private PlayerRBFacingDirection lastDirectionInput = PlayerRBFacingDirection.Right;

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
    }

    public void ToggleMovement(bool _canMove)
    {
        canMove = _canMove;
    }

    private void Update()
    {
        // Timers
        lastOnGroundTime -= Time.deltaTime;
        lastPressedJumpTime -= Time.deltaTime;

        // Get the x moveinput to:  0 for not moving, -1 for left, +1 for right 
        if (canMove)
            moveInput.x = Utility.BoolToInt(PlayerControls.GetRight()) - Utility.BoolToInt(PlayerControls.GetLeft());
        else
            moveInput.x = 0;

        if (PlayerControls.GetRightPressed())
        {
            lastDirectionInput = PlayerRBFacingDirection.Right;
        }

        if (PlayerControls.GetLeftPressed())
        {
            lastDirectionInput = PlayerRBFacingDirection.Left;
        }

        // Change the facing direction when new movement direction is different from current
        if (moveInput.x == 0)
        {
            
            if (facing != lastDirectionInput)
            {
                Turn();
            }
        }
        else
        {
            if (moveInput.x > 0)
            {
                CheckDirectionToFace(true);
            }

            if (moveInput.x < 0)
            {
                CheckDirectionToFace(false);
            }
        }

        #region Jump Inputs
        if (canMove)
        {
            if (PlayerControls.GetJumpPressed())
            {
                OnJumpInput();
            }

            if (PlayerControls.GetJumpReleased())
            {
                OnJumpUpInput();
            }
        }
        #endregion

        #region Jump Checks
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
        #endregion

        #region Double Jump
        // jump unlocked, not grounded, not currently double jumping, jump input
        if (PlayerUnlocks.isDoubleJumpUnlocked && lastOnGroundTime < 0 && !isDoubleJumping && PlayerControls.GetJumpPressed())
        {
            isDoubleJumping = true; // Is now double jumping

            _isJumpCut = false; // Reset jump cut when new jump
            RB.velocity = new Vector2(RB.velocity.x, 0); // Zero the fall speed

            RB.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse); // Apply Jump
        }

        // jump cut if jump released while ascending from double jump
        if (PlayerControls.GetJumpReleased() && isDoubleJumping && RB.velocity.y > 0)
        {
            _isJumpCut = true;
        }
        #endregion

        #region Gravity
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
        #endregion
    }

    private void FixedUpdate()
    {
        if (OnIce)
            RunOnIce();
        else
            Run();
    }

    public void OnJumpInput()
    {
        // when jump is pressed
        // set last pressed jump time to the tolerance
        // during the tolerance time, a jump is always being "pressed"
        lastPressedJumpTime = jumpInputBufferTime;
    }

    public void OnJumpUpInput()
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

    private void Run()
    {
        //Calculate the direction we want to move in and our desired velocity
        float targetSpeed = moveInput.x * runMaxSpeed;

        // Apply acceleration/decceleration
        float accelRate;
        if (Mathf.Abs(targetSpeed) > 0.01)
            accelRate = runAccelAmount;
        else
            accelRate = runDeccelAmount;

        //Calculate difference between current velocity and desired velocity
        float speedDif = targetSpeed - RB.velocity.x;

        //Calculate force along x-axis to apply to thr player
        float movement = speedDif * accelRate;

        //Convert this to a vector and apply to rigidbody
        RB.AddForce(movement * Vector2.right, ForceMode2D.Force);
    }

    private void RunOnIce()
    {
        float targetSpeed = moveInput.x * runMaxSpeed * iceSpeedMultiplier;
        float currVelocity = RB.velocity.x;
        
        if (moveInput.x == 0.0f) // trying to stop
        {
            if (Mathf.Abs(currVelocity) > 0.1f) // keep slowing down
            {
                float factor = 0f;
                if (currVelocity < 0)
                {
                    factor = currVelocity + (1.0f / slipperyness);
                }
                else if (currVelocity > 0)
                {
                    factor = currVelocity - (1.0f / slipperyness);
                }
                RB.velocity = new Vector2(factor, RB.velocity.y);
            }
        }
        else // trying to move
        {
            if (moveInput.x > 0.0f) // want to move right
            {
                if (currVelocity < targetSpeed) // keep speeding up right direction
                {
                    float factor = currVelocity + (1.0f / slipperyness);
                    RB.velocity = new Vector2(factor, RB.velocity.y);
                }
            }
            else if (moveInput.x < 0.0f) // want to move left
            {
                if (currVelocity > targetSpeed) // keep speeding up left direction
                {
                    float factor = currVelocity - (1.0f / slipperyness);
                    RB.velocity = new Vector2(factor, RB.velocity.y);
                }
            }
        }
    }

    private void Turn()
    {
        if (facing == PlayerRBFacingDirection.Right)
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);

            facing = PlayerRBFacingDirection.Left;
        }
        else
        {
            Vector3 rotator = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
            transform.rotation = Quaternion.Euler(rotator);

            facing = PlayerRBFacingDirection.Right;
        }
    }

    public void CheckDirectionToFace(bool isFacingRight)
    {
        if (isFacingRight)
        {
            if (facing != PlayerRBFacingDirection.Right)
                Turn();
        }
        else
        {
            if (facing != PlayerRBFacingDirection.Left)
                Turn();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
    }
}
