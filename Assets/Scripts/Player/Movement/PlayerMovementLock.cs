using UnityEngine;

public class PlayerMovementLock : MonoBehaviour
{
    public static PlayerMovementLock instance;

    public bool CanMove { get; private set; } = true;
    private int  _currentPriority;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            CanMove = true;
        }
    }

    /// <summary>
    /// Lock movement if priority is higher than current lock and set priority
    /// </summary>
    /// <param name="priority"></param>
    public void LockMovement(int priority = 0)
    {
        if (priority >= _currentPriority)
        {
            _currentPriority = priority;
            CanMove = false;
        }
    }

    /// <summary>
    /// Unlock movement if priority is higher than current lock and reset priority
    /// </summary>
    /// <param name="priority"></param>
    public void UnlockMovement(int priority = 0)
    {
        if (priority >= _currentPriority)
        {
            _currentPriority = 0;
            CanMove = true;
        }
    }
}
