using UnityEngine;

public abstract class PlayerMovementBehaviour : MonoBehaviour
{
    public bool CanMove()
    {
        if (PlayerMovementLock.instance == null)
            return false;

        return PlayerMovementLock.instance.CanMove;
    }
}
