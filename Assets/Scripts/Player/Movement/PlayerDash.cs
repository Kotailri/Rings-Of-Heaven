using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDash : PlayerMovementBehaviour
{
    public float DashForce;
    public float DashTime;
    public float DashCooldown;

    private float currentDashCooldown = 0f;
    [HideInInspector]
    public bool airDashReady = true;

    [Space(10)]
    public ParticleSystem DashParticle;
    public ParticleSystem CooldownIndicationParticle;

    public GameObject afterImage;

    private Rigidbody2D RB;
    private PlayerMovement pm;
    private PlayerGrounded grounded;

    private bool isDashing = false;
    private Vector2 dashDirection = Vector2.zero;
    private float lockedY;

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        pm = GetComponent<PlayerMovement>();
        grounded = GetComponent<PlayerGrounded>();
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (PlayerUnlocks.isDashUnlocked
            && currentDashCooldown <= 0
            && CanMove()
            && isDashing == false
            && context.phase == InputActionPhase.Started)
        {
            // Check Dash Requirements
            if (!grounded.isGrounded && !airDashReady)
            {
                return;
            }

            if (!grounded.isGrounded && airDashReady)
            {
                airDashReady = false;
            }
            
            isDashing = true;

            // Set the dash direction
            
            if (pm.facing == PlayerRBFacingDirection.Left)
            {
                dashDirection = Vector2.left;
            }

            if (pm.facing == PlayerRBFacingDirection.Right)
            {
                dashDirection = Vector2.right;
            }

            // Reset velocity
            RB.velocity = Vector2.zero;

            // Dash Coroutine
            StartCoroutine(DashCoroutine());

            // Set Cooldown
            currentDashCooldown = DashCooldown;
        }
    }

    private IEnumerator DashCoroutine()
    {
        AudioManager.instance.PlaySound("dash");

        Quaternion rotation = Quaternion.identity;
        if (GetComponent<PlayerFacing>().GetFacingDirectionRB() == PlayerRBFacingDirection.Left )
        {
            rotation = Quaternion.Euler( 0, 180f, 0 );
        }

        pm.ReleaseInputs();
        Instantiate(afterImage, transform.position, rotation);
        DashParticle.Play();

        RB.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        isDashing = true;
        PlayerMovementLock.instance.LockMovement();
        
        lockedY = transform.position.y;

        yield return new WaitForSecondsRealtime(DashTime);

        DashParticle.Play();
        Instantiate(afterImage, transform.position, rotation);

        PlayerMovementLock.instance.UnlockMovement();
        isDashing = false;

        RB.velocity = Vector2.zero;
        RB.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void Update()
    {
        if (currentDashCooldown > 0) 
        {
            currentDashCooldown -= Time.deltaTime;
        }

        if (isDashing)
        {
            pm.ReleaseInputs();
            RB.velocity = Vector2.zero;
            RB.velocity = new Vector2(Mathf.Sign(dashDirection.x) * DashForce, 0);
            transform.position = new Vector2(transform.position.x, (float)lockedY);
        }

        if (grounded.isGrounded)
        {
            airDashReady = true;
        }
    }
}
