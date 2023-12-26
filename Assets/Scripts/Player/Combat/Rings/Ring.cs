using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Ring : MonoBehaviour
{
    protected float range = 10.0f;
    public abstract float GetRingSpeed();
    public abstract int GetRingDamage();
    public abstract void SendRing(PlayerFacingDirection direction);
}

public interface IRingCatchable
{
    public bool IsCatchable();
}

public interface IRingReturn
{
    public void Return();
    public bool IsReturning();
}