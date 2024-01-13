using UnityEngine;

[RequireComponent (typeof(Rigidbody2D))]
[RequireComponent (typeof(PlayerGrounded))]
[RequireComponent (typeof(PlayerAxisControl))]
[RequireComponent (typeof(PlayerFacing))]
public class PlayerMovement : PlayerMovementBehaviour
{
    [Header("Movement")]
    [Range(1, 100)]
    [SerializeField]   private float MovementSpeed; // Target speed we want the player to reach

    [Header("Friction")]
    [Range(0f, 10f)] public float Slipperyness; // How hard it is to change direction / stop
    [Range(1, 10)]     public float SlipSpeedMultiplier; // How much faster player can accelerate while slipping

    private Rigidbody2D         _RB;
    private PlayerGrounded      _playerGrounded;
    private PlayerAxisControl   _axisControl;
    private PlayerFacing        _playerFacing;
    private PlayerCameraHandler _playerCameraHandler;

    private Vector2 moveInput;

    private void Awake()
    {
        _RB                  = GetComponent<Rigidbody2D>();
        _playerGrounded      = GetComponent<PlayerGrounded>();
        _axisControl         = GetComponent<PlayerAxisControl>();
        _playerFacing        = GetComponent<PlayerFacing>();
        _playerCameraHandler = GetComponent<PlayerCameraHandler>();
    }

    private void Update()
    {
        if (!CanMove())
            return;

        // Get axis x input
        float controllerInputX = 0;
        if (Mathf.Abs(_axisControl.GetAxisInputX()) >= Config.ControllerDeadZone)
        {
            if (_axisControl.GetAxisInputX() < 0) { controllerInputX = -1; }
            if (_axisControl.GetAxisInputX() > 0) { controllerInputX = 1; }
        }

        // Get keyboard x input if controller not available
        float keyboardInputX = _axisControl.GetKeyboardRight() - _axisControl.GetKeyboardLeft();
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

        MovePlayerHorizontal();
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
    private void MovePlayerHorizontal()
    {
        float targetSpeed = moveInput.x * MovementSpeed * SlipSpeedMultiplier;
        float currVelocity = _RB.velocity.x;

        // Trying to Stop:
        // Constantly slow movespeed until stopped
        if (moveInput.x == 0.0f)
        {
            // Trying to stop
            if (Mathf.Abs(currVelocity) > (1.0f / Slipperyness) && Slipperyness != 0)
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

                _RB.velocity = new Vector2(factor, _RB.velocity.y);
            }
            else
            {
                _RB.velocity = new Vector2(0, _RB.velocity.y);
            }
            PlayerMovementEventManager.TriggerEvent(PlayerMovementEvent.OnPlayerStop, null);
        }

        // Trying to Move:
        // Constantly speed up until velocity is max speed * slip ispeed
        else
        {
            // Trying to move right
            if (moveInput.x > 0.0f)
            {
                if (currVelocity < targetSpeed && Slipperyness != 0)
                {
                    float factor = currVelocity + (1.0f / Slipperyness);
                    _RB.velocity = new Vector2(factor, _RB.velocity.y);
                }
                else
                {
                    _RB.velocity = new Vector2(targetSpeed, _RB.velocity.y);
                }
            }

            // Trying to move left
            else if (moveInput.x < 0.0f)
            {
                if (currVelocity > targetSpeed && Slipperyness != 0)
                {
                    float factor = currVelocity - (1.0f / Slipperyness);
                    _RB.velocity = new Vector2(factor, _RB.velocity.y);
                }
                else
                {
                    _RB.velocity = new Vector2(targetSpeed, _RB.velocity.y);
                }
            }
            PlayerMovementEventManager.TriggerEvent(PlayerMovementEvent.OnPlayerMove, null);
        }
    }

    /// <summary>
    /// Rotates the player about the Y axis to flip
    /// Sets the new facing direction
    /// </summary>
    private void Turn()
    {
        Vector3 rotator;

        if (_playerFacing.FacingDirection == OrthogonalDirection.Right)
        {
            rotator = new Vector3(transform.rotation.x, 180f, transform.rotation.z);
            _playerFacing.FacingDirection = OrthogonalDirection.Left;
        }
        else
        {
            rotator = new Vector3(transform.rotation.x, 0f, transform.rotation.z);
            _playerFacing.FacingDirection = OrthogonalDirection.Right;
        }

        transform.rotation = Quaternion.Euler(rotator);
        _playerCameraHandler.CallTurn();

        if (_playerGrounded.IsGrounded)
        {
            PlayerParticleManager.instance.PlayParticles(PlayerParticleName.TurnParticles);
            PlayerMovementEventManager.TriggerEvent(PlayerMovementEvent.OnPlayerTurnGrounded, null);
        }

        PlayerMovementEventManager.TriggerEvent(PlayerMovementEvent.OnPlayerTurn, null);

    }

    /// <summary>
    /// Check if Turn() is to be called.
    /// </summary>
    /// <param name="isFacingRight"></param>
    public void CheckDirectionToFace(bool isFacingRight)
    {
        if (isFacingRight)
        {
            if (_playerFacing.FacingDirection != OrthogonalDirection.Right)
                Turn();
        }
        else
        {
            if (_playerFacing.FacingDirection != OrthogonalDirection.Left)
                Turn();
        }
    }

}
