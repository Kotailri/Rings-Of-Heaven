using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerFacingDirection
{
    Up, Down, Left, Right, None
}

public class PlayerFacing : MonoBehaviour
{
    private PlayerFacingDirection facingDirection = PlayerFacingDirection.Right;
    private PlayerMovement pm;

    private void Awake()
    {
        pm = GetComponent<PlayerMovement>();
    }

    public PlayerFacingDirection GetFacingDirection()
    {
        return facingDirection;
    }

    private void Update()
    {
        if (PlayerControls.GetUp() && !PlayerControls.GetDown())
        {
            facingDirection = PlayerFacingDirection.Up;
            return;
        }

        if (PlayerControls.GetDown() && !PlayerControls.GetUp())
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
}
