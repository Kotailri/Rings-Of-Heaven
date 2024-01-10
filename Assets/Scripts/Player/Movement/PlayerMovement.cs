using UnityEngine;

[RequireComponent (typeof(Rigidbody2D))]
[RequireComponent (typeof(PlayerGrounded))]
[RequireComponent (typeof(PlayerAxisControl))]
[RequireComponent (typeof(PlayerFacing))]
public class PlayerMovement : PlayerMovementBehaviour
{
    [Header("Movement")]
    [SerializeField] private float MovementSpeed; //Target speed we want the player to reach.

    [Header("Friction")]
    public float Slipperyness;
    public float SlipSpeedMultiplier;

    [Space(10f)]
    [Header("Extras")]
    public ParticleSystem     dustParticles;
    public CameraFollowObject followObject;

    private Rigidbody2D       RB;
    private PlayerGrounded    grounded;
    private PlayerAxisControl axisControl;
    private PlayerFacing      facing;

    private Vector2 moveInput;

    private void Awake()
    {
        RB          = GetComponent<Rigidbody2D>();
        grounded    = GetComponent<PlayerGrounded>();
        axisControl = GetComponent<PlayerAxisControl>();
        facing      = GetComponent<PlayerFacing>();
    }

    private void OnValidate()
    {
        if (Slipperyness <= 0)
        {
            Slipperyness = 0.1f;
        }

        if (SlipSpeedMultiplier <= 0)
        {
            SlipSpeedMultiplier = 1;
        }

        if (MovementSpeed <= 1)
        {
            MovementSpeed = 1;
        }
    }

    private void Update()
    {
        if (!CanMove())
            return;

        // Get axis x input
        float controllerInputX = 0;
        if (Mathf.Abs(axisControl.GetAxisInputX()) >= Config.ControllerDeadZone)
        {
            if (axisControl.GetAxisInputX() < 0) { controllerInputX = -1; }
            if (axisControl.GetAxisInputX() > 0) { controllerInputX = 1; }
        }

        // Get keyboard x input if controller not available
        float keyboardInputX = axisControl.GetKeyboardRight() - axisControl.GetKeyboardLeft();
        if (controllerInputX != 0)
        {
            moveInput.x = controllerInputX;
        }
        else
        {
            moveInput.x = keyboardInputX;
        }

        // Change the facing direction when new movement direction is different from current
        if (moveInput.x != 0)
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
        if (!CanMove())
            return;

        Run();
    }

    /// <summary>
    /// Releases all movement inputs for a single frame
    /// </summary>
    public void ReleaseInputs()
    {
        moveInput = Vector2.zero;
    }

    /// <summary>
    /// Move the player horizontally
    /// </summary>
    private void Run()
    {
        float targetSpeed = moveInput.x * MovementSpeed * SlipSpeedMultiplier;
        float currVelocity = RB.velocity.x;

        // Trying to Stop:
        // Constantly slow movespeed until stopped
        if (moveInput.x == 0.0f)
        {
            // Trying to stop
            if (Mathf.Abs(currVelocity) > (1.0f / Slipperyness))
            {
                float factor = 0f;
                if (currVelocity < 0)
                {
                    factor = currVelocity + (1.0f / Slipperyness);
                }

                else if (currVelocity > 0)
                {
                    factor = currVelocity - (1.0f / Slipperyness);
                }

                RB.velocity = new Vector2(factor, RB.velocity.y);
            }
            else
            {
                RB.velocity = new Vector2(0, RB.velocity.y);
            }
        }

        // Trying to Move:
        // Constantly speed up until velocity is max speed * slip ispeed
        else
        {
            // Trying to move right
            if (moveInput.x > 0.0f)
            {
                if (currVelocity < targetSpeed)
                {
                    float factor = currVelocity + (1.0f / Slipperyness);
                    RB.velocity = new Vector2(factor, RB.velocity.y);
                }
                else
                {
                    RB.velocity = new Vector2(targetSpeed, RB.velocity.y);
                }
            }

            // Trying to move left
            else if (moveInput.x < 0.0f)
            {
                if (currVelocity > targetSpeed)
                {
                    float factor = currVelocity - (1.0f / Slipperyness);
                    RB.velocity = new Vector2(factor, RB.velocity.y);
                }
                else
                {
                    RB.velocity = new Vector2(targetSpeed, RB.velocity.y);
                }
            }
        }
    }

    /// <summary>
    /// Rotates the player about the Y axis to flip
    /// Sets the new facing direction
    /// </summary>
    private void Turn()
    {
        Vector3 rotator;

        if (facing.FacingDirection == OrthogonalDirection.Right)
        {
            rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
            facing.FacingDirection = OrthogonalDirection.Left;
        }
        else
        {
            rotator = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
            facing.FacingDirection = OrthogonalDirection.Right;
        }

        transform.rotation = Quaternion.Euler(rotator);
        followObject.CallTurn();

        if (grounded.isGrounded)
        {
            dustParticles.Play();
        }

    }

    /// <summary>
    /// Check if Turn() is to be called.
    /// </summary>
    /// <param name="isFacingRight"></param>
    public void CheckDirectionToFace(bool isFacingRight)
    {
        if (isFacingRight)
        {
            if (facing.FacingDirection != OrthogonalDirection.Right)
                Turn();
        }
        else
        {
            if (facing.FacingDirection != OrthogonalDirection.Left)
                Turn();
        }
    }

}
