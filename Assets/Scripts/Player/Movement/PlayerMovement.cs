using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FacingDirection
{
    Right,
    Left, 
    Up, 
    Down
}

public class PlayerMovement : MonoBehaviour
{
    [Header("Run")]
    public float runMaxSpeed; //Target speed we want the player to reach.
    //public float runAcceleration; //The speed at which our player accelerates to max speed, can be set to runMaxSpeed for instant acceleration down to 0 for none at all
    public float runAccelAmount; //The actual force (multiplied with speedDiff) applied to the player.
    //public float runDecceleration; //The speed at which our player decelerates from their current speed, can be set to runMaxSpeed for instant deceleration down to 0 for none at all
    public float runDeccelAmount; //Actual force (multiplied with speedDiff) applied to the player .

    [Space(10)]
    [Range(0.01f, 1)] 
    public float accelInAir; //Multipliers applied to acceleration rate when airborne.
    [Range(0.01f, 1)] 
    public float deccelInAir;

    [Space(10)]
    [Header("Gravity")]
    //public float gravityStrength; //Downwards force (gravity) needed for the desired jumpHeight and jumpTimeToApex.
    public float gravityScale; //Strength of the player's gravity as a multiplier of gravity (set in ProjectSettings/Physics2D).
                                                 //Also the value the player's rigidbody2D.gravityScale is set to.
    [Space(5)]
    public float fallGravityMult; //Multiplier to the player's gravityScale when falling.
    public float maxFallSpeed; //Maximum fall speed (terminal velocity) of the player when falling.
    [Space(5)]
    public float fastFallGravityMult; //Larger multiplier to the player's gravityScale when they are falling and a downwards input is pressed.
                                      //Seen in games such as Celeste, lets the player fall extra fast if they wish.
    public float maxFastFallSpeed; //Maximum fall speed(terminal velocity) of the player when performing a faster fall.

    [Space(10)]
    [Header("Jump")]
    [HideInInspector]
    public bool isJumping;
    //public float jumpHeight; //Height of the player's jump
    //public float jumpTimeToApex; //Time between applying the jump force and reaching the desired jump height. These values also control the player's gravity and jump force.
    public float jumpForce; //The actual force applied (upwards) to the player when they jump.

    [Space(10)]
    [Header("Both Jumps")]
    public float jumpCutGravityMult; //Multiplier to increase gravity if the player releases thje jump button while still jumping
    [Range(0f, 1)] 
    public float jumpHangGravityMult; //Reduces gravity while close to the apex (desired max height) of the jump
    public float jumpHangTimeThreshold; //Speeds (close to 0) where the player will experience extra "jump hang". The player's velocity.y is closest to 0 at the jump's apex (think of the gradient of a parabola or quadratic function)
    //[Space(0.5f)]
    //public float jumpHangAccelerationMult;
    //public float jumpHangMaxSpeedMult;

    [Space(10)]
    [Header("Jump Assists")]
    [Range(0.01f, 0.5f)] public float coyoteTime; //Grace period after falling off a platform, where you can still jump
    [Range(0.01f, 0.5f)] public float jumpInputBufferTime; //Grace period after pressing jump where a jump will be automatically performed once the requirements (eg. being grounded) are met.

    private bool _isJumpCut;
    private bool _isJumpFalling;

    // Timers
    private float lastOnGroundTime;
    private float lastPressedJumpTime;

    [Space(15)]
    [Header("Checks")]
    [SerializeField] 
    private Transform _groundCheckPoint;
    [SerializeField] 
    private Vector2 _groundCheckSize = new Vector2(0.49f, 0.03f);

    [Space(10)]
    private Rigidbody2D RB;
    private FacingDirection facing = FacingDirection.Right;
    
    public LayerMask _groundLayer;

    private Vector2 moveInput;

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // Timers
        lastOnGroundTime -= Time.deltaTime;
        lastPressedJumpTime -= Time.deltaTime;

        moveInput.x = Utility.BoolToInt(PlayerControls.GetRight()) - Utility.BoolToInt(PlayerControls.GetLeft());

        if (moveInput.x != 0)
        {
            if (moveInput.x > 0)
            {
                CheckDirectionToFace(facing);
            }
        }

        if (PlayerControls.GetJumpPressed()) 
        {
            OnJumpInput();
        }

        if (PlayerControls.GetJumpReleased())
        {
            OnJumpUpInput();
        }

        // Collision Checking
        if (!isJumping)
        {
            if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer))
            {
                lastOnGroundTime = coyoteTime;
            }
        }

        // Jump Checks
        if (isJumping && RB.velocity.y < 0)
        {
            isJumping = false;
        }

        if (lastOnGroundTime > 0 && !isJumping)
        {
            _isJumpCut = false;

            if (!isJumping)
                _isJumpFalling = false;
        }

        // Jump
        if (CanJump() && lastPressedJumpTime > 0)
        {
            isJumping = true;
            _isJumpCut = false;
            _isJumpFalling = false;
            Jump();
        }

        // Gravity
        //Higher gravity if we've released the jump input or are falling
        /*
        if (RB.velocity.y < 0 && PlayerControls.GetDown())
        {
            //Much higher gravity if holding down
            RB.gravityScale = gravityScale * fastFallGravityMult;
            //Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
            RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -maxFastFallSpeed));
        }
        */
        if (_isJumpCut)
        {
            //Higher gravity if jump button released
            RB.gravityScale = gravityScale * jumpCutGravityMult;
            RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -maxFallSpeed));
        }
        else if ((isJumping || _isJumpFalling) && Mathf.Abs(RB.velocity.y) < jumpHangTimeThreshold)
        {
            RB.gravityScale = gravityScale * jumpHangGravityMult;
        }
        else
        {
            RB.gravityScale = gravityScale;
        }
    }

    private void FixedUpdate()
    {
        Run();
    }

    public void OnJumpInput()
    {
        lastPressedJumpTime = jumpInputBufferTime;
    }

    public void OnJumpUpInput()
    {
        if (CanJumpCut())
            _isJumpCut = true;
    }

    private void Jump()
    {
        //Ensures we can't call Jump multiple times from one press
        lastPressedJumpTime = 0;
        lastOnGroundTime = 0;

        //We increase the force applied if we are falling
        //This means we'll always feel like we jump the same amount 
        //(setting the player's Y velocity to 0 beforehand will likely work the same, but I find this more elegant :D)
        float force = jumpForce;
        if (RB.velocity.y < 0)
            force -= RB.velocity.y;

        RB.AddForce(Vector2.up * force, ForceMode2D.Impulse);
    }

    private bool CanJump()
    {
        return lastOnGroundTime > 0 && !isJumping;
    }

    private bool CanJumpCut()
    {
        return isJumping && RB.velocity.y > 0;
    }

    private void Run()
    {
        //Calculate the direction we want to move in and our desired velocity
        float targetSpeed = moveInput.x * runMaxSpeed;
        targetSpeed = Mathf.Lerp(RB.velocity.x, targetSpeed, 1);
        float accelRate;

        //Gets an acceleration value based on if we are accelerating (includes turning) 
        //or trying to decelerate (stop). As well as applying a multiplier if we're air borne.
        if (lastOnGroundTime > 0)
        {
            if (Mathf.Abs(targetSpeed) > 0.01f)
                accelRate = runAccelAmount;
            else
                accelRate = runDeccelAmount;
        }
        else
        {
            if (Mathf.Abs(targetSpeed) > 0.01f)
                accelRate = runAccelAmount * accelInAir;
            else
                accelRate = runDeccelAmount * deccelInAir;
        }

        //Calculate difference between current velocity and desired velocity
        float speedDif = targetSpeed - RB.velocity.x;
        //Calculate force along x-axis to apply to thr player

        float movement = speedDif * accelRate;

        //Convert this to a vector and apply to rigidbody
        RB.AddForce(movement * Vector2.right, ForceMode2D.Force);
    }

    private void Turn()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        if (facing == FacingDirection.Right) 
        {
            facing = FacingDirection.Left;
        }
        else
        {
            facing = FacingDirection.Right;
        }
    }

    public void CheckDirectionToFace(FacingDirection _facing)
    {
        if (_facing != facing)
            Turn();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
    }
}
