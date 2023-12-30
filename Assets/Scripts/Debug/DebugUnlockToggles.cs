using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugUnlockToggles : MonoBehaviour
{
    public bool DoubleJump;
    public bool Dash;

    private void Start()
    {
        PlayerUnlocks.isDoubleJumpUnlocked = DoubleJump;
        PlayerUnlocks.isDoubleJumpUnlocked = Dash;
    }

    private void Update()
    {
        if (DoubleJump != PlayerUnlocks.isDoubleJumpUnlocked)
        {
            PlayerUnlocks.isDoubleJumpUnlocked = DoubleJump;
        }

        if (Dash != PlayerUnlocks.isDashUnlocked)
        {
            PlayerUnlocks.isDashUnlocked = Dash;
        }
    }
}
