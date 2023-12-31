
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerRBFacingDirection
{
    Right, Left
}

public class PlayerMovement : PlayerMovementBehaviour
{
    [Header("Movement")]
    public float runMaxSpeed; //Target speed we want the player to reach.
    public float runAccelAmount; //The actual force (multiplied with speedDiff) applied to the player.
    public float runDeccelAmount; //Actual force (multiplied with speedDiff) applied to the player .

    [Space(15)]
    public ParticleSystem dustParticles;

    private Rigidbody2D RB;
    private PlayerGrounded grounded;
    private PlayerJump pj;

    [Space(15)]
    [HideInInspector]
    public PlayerRBFacingDirection facing = PlayerRBFacingDirection.Right;
    private Vector2 moveInput;
    public CameraFollowObject followObject;

    [Space(15)]
    [Header("Ice Movement")]
    public bool OnIce;
    public float slipperyness;
    public float iceSpeedMultiplier;

    [HideInInspector]
    public Controls controls;
    private float axisInputX;
    private float keyboardLeft;
    private float keyboardRight;

    private void Awake()
    {
        controls = new Controls();



        switch (Config.controlConfig)
        {
            case ControlConfig.Arrows:
                controls.Gameplay.MoveX.performed += ctx => axisInputX = ctx.ReadValue<float>();

                controls.Gameplay.KeyboardLeft.performed += ctx => keyboardLeft = ctx.ReadValue<float>();
                controls.Gameplay.KeyboardLeft.canceled += ctx => keyboardLeft = 0;

                controls.Gameplay.KeyboardRight.performed += ctx => keyboardRight = ctx.ReadValue<float>();
                controls.Gameplay.KeyboardRight.canceled += ctx => keyboardRight = 0;


                GetComponent<PlayerInput>().SwitchCurrentActionMap("Gameplay");
                controls.Gameplay.Enable();
                controls.GameplayWASD.Disable();
                break;

            case ControlConfig.WASD:
                controls.GameplayWASD.MoveX.performed += ctx => axisInputX = ctx.ReadValue<float>();

                controls.GameplayWASD.KeyboardLeft.performed += ctx => keyboardLeft = ctx.ReadValue<float>();
                controls.GameplayWASD.KeyboardLeft.canceled += ctx => keyboardLeft = 0;

                controls.GameplayWASD.KeyboardRight.performed += ctx => keyboardRight = ctx.ReadValue<float>();
                controls.GameplayWASD.KeyboardRight.canceled += ctx => keyboardRight = 0;


                GetComponent<PlayerInput>().SwitchCurrentActionMap("GameplayWASD");
                controls.GameplayWASD.Enable();
                controls.Gameplay.Disable();
                break;
        }

        GetComponent<PlayerFacing>().SetupControls(controls);
        

        RB = GetComponent<Rigidbody2D>();
        pj = GetComponent<PlayerJump>();
        grounded = GetComponent<PlayerGrounded>();

        ReleaseInputs();
        RB.velocity = Vector2.zero;
    }

    public void ReleaseInputs()
    {
        moveInput = Vector2.zero;
    }

    private void Update()
    {
        if (!CanMove())
        {
            return;
        }

        // Get the x moveinput
        float controllerInputX = 0;
        if (Mathf.Abs(axisInputX) >= Config.ControllerDeadZone)
        {
            controllerInputX = axisInputX;
        }

        float keyboardInputX = keyboardRight - keyboardLeft;

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

    public float GetMoveInputX()
    {
        return moveInput.x;
    }

    private void FixedUpdate()
    {
        if (!CanMove())
        {
            return;
        }

        if (grounded.isIcy)
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
        followObject.CallTurn();

        if (grounded.isGrounded)
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
