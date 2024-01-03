using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDash : MonoBehaviour
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
        Quaternion rotation = Quaternion.identity;
        if (GetComponent<PlayerFacing>().GetFacingDirectionRB() == PlayerRBFacingDirection.Left )
        {
            rotation = Quaternion.Euler( 0, 180f, 0 );
        }

        DashParticle.Play();
        Instantiate(afterImage, transform.position, rotation);

        RB.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        isDashing = true;
        pm.ToggleMovement(false);
        lockedY = transform.position.y;

        yield return new WaitForSecondsRealtime(DashTime);

        DashParticle.Play();
        Instantiate(afterImage, transform.position, rotation);

        RB.velocity = Vector2.zero;
        pm.ToggleMovement(true);
        isDashing = false;
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
            RB.velocity = new Vector2(dashDirection.x * DashForce, 0);
            transform.position = new Vector2(transform.position.x, (float)lockedY);
        }

        if (grounded.isGrounded)
        {
            airDashReady = true;
        }
    }
}
