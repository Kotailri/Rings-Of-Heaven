using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyMovement : MonoBehaviour
{
    protected Rigidbody2D _RB;

    [HideInInspector]
    public bool IsMoving;

    public abstract void StopMovement();
    public abstract void ResumeMovement();

    public void SetRigidBody(Rigidbody2D rb)
    {
        _RB = rb;
    }
}
