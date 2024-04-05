using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ring : MonoBehaviour
{
    public abstract float GetRingSpeed();
    public abstract int GetRingDamage();
    public abstract void SendRing(OrthogonalDirection direction);

    protected void OnTriggerStay2D(Collider2D collision)
    {
        if (Utility.IsOfTag(collision.gameObject, Tags.BrokeByRing))
        {
            Destroy(collision.gameObject);
        }
    }
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