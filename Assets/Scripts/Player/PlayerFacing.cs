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
        if (PlayerControls.GetUp())
        {
            facingDirection = PlayerFacingDirection.Up;
            return;
        }

        if (PlayerControls.GetLeft() && !PlayerControls.GetRight())
        {
            facingDirection = PlayerFacingDirection.Left;
            return;
        }

        if (PlayerControls.GetRight() && !PlayerControls.GetLeft())
        {
            facingDirection = PlayerFacingDirection.Right;
            return;
        }

        if (facingDirection == PlayerFacingDirection.Up)
        {
            if (pm.facing == FacingDirection.Left) 
            {
                facingDirection = PlayerFacingDirection.Left;
            }
            else
            {
                facingDirection = PlayerFacingDirection.Right;
            }
        }
    }
}
