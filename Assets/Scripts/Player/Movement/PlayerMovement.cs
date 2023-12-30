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

    [Space(15)]
    public ParticleSystem dustParticles;

    private Rigidbody2D RB;
    [HideInInspector]
    public PlayerRBFacingDirection facing = PlayerRBFacingDirection.Right;
    private Vector2 moveInput;

    private PlayerJump pj;

    [Header("TEMP DEBUG")]
    public bool OnIce;
    public float slipperyness;
    public float iceSpeedMultiplier;

    private bool canMove = true;
    private PlayerRBFacingDirection lastDirectionInput = PlayerRBFacingDirection.Right;

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        pj = GetComponent<PlayerJump>();
    }

    public void ToggleMovement(bool _canMove)
    {
        canMove = _canMove;
        pj.ToggleMovement(_canMove);
    }

    private void Update()
    {
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
    }

    private void FixedUpdate()
    {
        if (OnIce)
            RunOnIce();
        else
            Run();
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
        Vector3 rotator;

        if (facing == PlayerRBFacingDirection.Right)
        {
            rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
            facing = PlayerRBFacingDirection.Left;
        }
        else
        {
            rotator = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
            facing = PlayerRBFacingDirection.Right;
        }

        transform.rotation = Quaternion.Euler(rotator);

        if (pj.isGrounded)
        {
            dustParticles.Play();
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

}
