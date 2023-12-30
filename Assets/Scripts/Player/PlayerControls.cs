using System;
using System.Collections;
using UnityEngine;

public static class PlayerControls
{
    public static bool GetAttackRightPressed()
    {
        return Input.GetKeyDown(KeyCode.F);
    }

    public static bool GetAttackLeftPressed()
    {
        return Input.GetKeyDown(KeyCode.D);
    }

    public static bool GetDashPressed()
    {
        return Input.GetKeyDown(KeyCode.LeftShift);
    }

    #region Movememt

    public static bool GetLeft()
    {
        return Input.GetKey(KeyCode.LeftArrow);
    }

    public static bool GetLeftPressed()
    {
        return Input.GetKeyDown(KeyCode.LeftArrow);
    }

    public static bool GetLeftReleased()
    {
        return Input.GetKeyUp(KeyCode.LeftArrow);
    }

    public static bool GetRight()
    {
        return Input.GetKey(KeyCode.RightArrow);
    }

    public static bool GetRightPressed()
    {
        return Input.GetKeyDown(KeyCode.RightArrow);
    }

    public static bool GetRightReleased()
    {
        return Input.GetKeyUp(KeyCode.RightArrow);
    }

    public static bool GetUp()
    {
        return Input.GetKey(KeyCode.UpArrow);
    }

    public static bool GetUpPressed()
    {
        return Input.GetKeyDown(KeyCode.UpArrow);
    }

    public static bool GetUpReleased()
    {
        return Input.GetKeyUp(KeyCode.UpArrow);
    }

    public static bool GetDown()
    {
        return Input.GetKey(KeyCode.DownArrow);
    }

    public static bool GetDownPressed()
    {
        return Input.GetKeyDown(KeyCode.DownArrow);
    }

    public static bool GetDownReleased()
    {
        return Input.GetKeyUp(KeyCode.DownArrow);
    }
    #endregion

    public static bool GetJump()
    {
        return Input.GetKey(KeyCode.Space);
    }

    public static bool GetJumpPressed()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    public static bool GetJumpReleased()
    {
        return Input.GetKeyUp(KeyCode.Space);
    }
}
