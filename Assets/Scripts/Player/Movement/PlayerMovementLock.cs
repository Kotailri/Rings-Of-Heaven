using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementLock : MonoBehaviour
{
    public static PlayerMovementLock instance;
    private bool canMove = true;
    private int currentPriority;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            canMove = true;
        }
    }

    public void LockMovement(int priority = 0)
    {
        if (priority >= currentPriority)
        {
            currentPriority = priority;
            canMove = false;
        }
    }

    public void UnlockMovement(int priority = 0)
    {
        if (priority >= currentPriority)
        {
            currentPriority = 0;
            canMove = true;
        }
        
    }

    public bool CanMove()
    {
        return canMove;
    }
}

public abstract class PlayerMovementBehaviour : MonoBehaviour
{
    public bool CanMove()
    {
        if (PlayerMovementLock.instance == null)
            return false;

        return PlayerMovementLock.instance.CanMove();
    }
}
