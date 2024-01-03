using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerFacingDirection
{
    Up, Down, Left, Right, None
}

public class PlayerFacing : MonoBehaviour
{
    private PlayerFacingDirection facingDirection = PlayerFacingDirection.Right;
    private PlayerMovement pm;

    private Controls controls;
    private float axisInputY;

    private float KeyboardUp = 0f;
    private float KeyboardDown = 0f;

    private void Awake()
    {
        controls = new Controls();
        controls.Gameplay.MoveY.performed += ctx => axisInputY = ctx.ReadValue<float>();

        controls.Gameplay.KeyboardUp.performed += ctx => KeyboardUp = ctx.ReadValue<float>();
        controls.Gameplay.KeyboardUp.canceled += ctx => KeyboardUp = 0;

        controls.Gameplay.KeyboardDown.performed += ctx => KeyboardDown = ctx.ReadValue<float>();
        controls.Gameplay.KeyboardDown.canceled += ctx => KeyboardDown = 0;

        controls.Gameplay.Enable();
        pm = GetComponent<PlayerMovement>();
    }

    public PlayerFacingDirection GetFacingDirection()
    {
        return facingDirection;
    }

    public PlayerRBFacingDirection GetFacingDirectionRB()
    {
        return pm.facing;
    }

    private void CheckKeyboard(float input)
    {
        if (input == 1)
        {
            facingDirection = PlayerFacingDirection.Up;
            return;
        }

        if (input == -1)
        {
            facingDirection = PlayerFacingDirection.Down;
            return;
        }

        if (pm.facing == PlayerRBFacingDirection.Left)
        {
            facingDirection = PlayerFacingDirection.Left;
            return;
        }

        if (pm.facing == PlayerRBFacingDirection.Right)
        {
            facingDirection = PlayerFacingDirection.Right;
            return;
        }
    }

    private void CheckController(float axisInput)
    {
        if (axisInput > 0)
        {
            facingDirection = PlayerFacingDirection.Up;
            return;
        }

        if (axisInput < 0)
        {
            facingDirection = PlayerFacingDirection.Down;
            return;
        }

        if (pm.facing == PlayerRBFacingDirection.Left)
        {
            facingDirection = PlayerFacingDirection.Left;
            return;
        }

        if (pm.facing == PlayerRBFacingDirection.Right)
        {
            facingDirection = PlayerFacingDirection.Right;
            return;
        }
    }

    private void Update()
    {
        if (Mathf.Abs(axisInputY) >= Config.ControllerDeadZone)
        {
            CheckController(axisInputY);
        }
        else
        {
            CheckKeyboard(KeyboardUp - KeyboardDown);
        }
    }
}
