using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDash : PlayerMovementBehaviour
{
    [SerializeField] private float DashForce;
    [SerializeField] private float DashTime;
    [SerializeField] private float DashCooldown;

    [SerializeField] private float CurrentDashCooldown = 0f;
    public bool AirDashReady { get; set; } = true;

    [Space(10)]
    public ParticleSystem DashParticle;
    public ParticleSystem CooldownIndicationParticle;

    public GameObject afterImage;

    private Rigidbody2D RB;
    private PlayerMovement playerMovement;
    private PlayerGrounded playerGrounded;
    private PlayerFacing playerFacing;

    private bool IsDashing = false;
    private Vector2 DashDirection = Vector2.zero;

    private void Awake()
    {
        RB = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
        playerGrounded = GetComponent<PlayerGrounded>();
        playerFacing = GetComponent<PlayerFacing>();
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (PlayerUnlocks.isDashUnlocked
            && CurrentDashCooldown <= 0
            && CanMove()
            && IsDashing == false
            && context.phase == InputActionPhase.Started)
        {
            // Check Dash Requirements
            if (!playerGrounded.isGrounded && !AirDashReady)
            {
                return;
            }

            if (!playerGrounded.isGrounded && AirDashReady)
            {
                AirDashReady = false;
            }
            
            IsDashing = true;

            // Set the dash direction
            
            if (playerFacing.PointingDirection == OrthogonalDirection.Left)
            {
                DashDirection = Vector2.left;
            }

            if (playerFacing.PointingDirection == OrthogonalDirection.Right)
            {
                DashDirection = Vector2.right;
            }

            // Reset velocity
            RB.velocity = Vector2.zero;

            // Dash Coroutine
            StartCoroutine(DashCoroutine());

            // Set Cooldown
            CurrentDashCooldown = DashCooldown;
        }
    }

    private IEnumerator DashCoroutine()
    {
        AudioManager.instance.PlaySound("dash");

        Quaternion rotation = Quaternion.identity;
        if (GetComponent<PlayerFacing>().FacingDirection == OrthogonalDirection.Left )
        {
            rotation = Quaternion.Euler( 0, 180f, 0 );
        }

        playerMovement.ReleaseInputs();
        Instantiate(afterImage, transform.position, rotation);
        DashParticle.Play();

        RB.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        IsDashing = true;
        PlayerMovementLock.instance.LockMovement();

        yield return new WaitForSecondsRealtime(DashTime);

        DashParticle.Play();
        Instantiate(afterImage, transform.position, rotation);

        PlayerMovementLock.instance.UnlockMovement();
        IsDashing = false;

        RB.velocity = Vector2.zero;
        RB.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void Update()
    {
        if (CurrentDashCooldown > 0) 
        {
            CurrentDashCooldown -= Time.deltaTime;
        }

        if (IsDashing)
        {
            playerMovement.ReleaseInputs();
            RB.velocity = Vector2.zero;
            RB.velocity = new Vector2(Mathf.Sign(DashDirection.x) * DashForce, 0);
        }

        if (playerGrounded.isGrounded)
        {
            AirDashReady = true;
        }
    }
}
