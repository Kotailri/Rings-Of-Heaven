using UnityEngine;

public enum OrthogonalDirection
{
    Up, Down, Left, Right, None
}

public class PlayerFacing : MonoBehaviour
{
    public OrthogonalDirection FacingDirection   { get; set; } = OrthogonalDirection.Right;  // Direction based on keyboard input
    public OrthogonalDirection PointingDirection { get; set; } = OrthogonalDirection.None;   // Direction based on player facing direction

    private PlayerAxisControl axisControl;

    private void Awake()
    {
        axisControl = GetComponent<PlayerAxisControl>();
    }

    /// <summary>
    /// Sets PointingDirection based on keyboard inputs.
    /// </summary>
    /// <param name="input"></param>
    private void CheckKeyboardPointingDirection(float input)
    {
        if (input == 1)
        {
            PointingDirection = OrthogonalDirection.Up;
            return;
        }

        if (input == -1)
        {
            PointingDirection = OrthogonalDirection.Down;
            return;
        }

        if (FacingDirection == OrthogonalDirection.Left)
        {
            PointingDirection = OrthogonalDirection.Left;
            return;
        }

        if (FacingDirection == OrthogonalDirection.Right)
        {
            PointingDirection = OrthogonalDirection.Right;
            return;
        }
    }

    /// <summary>
    /// Sets PointingDirection based on controller axis inputs.
    /// </summary>
    /// <param name="axisInput"></param>
    private void CheckControllerPointingDirection(float axisInput)
    {
        if (axisInput > 0)
        {
            PointingDirection = OrthogonalDirection.Up;
            return;
        }

        if (axisInput < 0)
        {
            PointingDirection = OrthogonalDirection.Down;
            return;
        }

        if (FacingDirection == OrthogonalDirection.Left)
        {
            PointingDirection = OrthogonalDirection.Left;
            return;
        }

        if (FacingDirection == OrthogonalDirection.Right)
        {
            PointingDirection = OrthogonalDirection.Right;
            return;
        }
    }

    private void Update()
    {
        if (Mathf.Abs(axisControl.GetAxisInputY()) >= Config.ControllerDeadZone)
        {
            CheckControllerPointingDirection(axisControl.GetAxisInputY());
        }
        else
        {
            CheckKeyboardPointingDirection(axisControl.GetKeyboardUp() - axisControl.GetKeyboardDown());
        }
    }
}
