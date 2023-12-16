using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugUnlockToggles : MonoBehaviour
{
    public bool DoubleJump;

    private void Start()
    {
        PlayerUnlocks.isDoubleJumpUnlocked = DoubleJump;
    }

    private void Update()
    {
        if (DoubleJump != PlayerUnlocks.isDoubleJumpUnlocked)
        {
            PlayerUnlocks.isDoubleJumpUnlocked = DoubleJump;
        }
    }
}
