using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyIdle : MonoBehaviour
{
    protected Rigidbody2D _RB;

    [HideInInspector]
    public bool IsIdling;

    public abstract void StartIdleBehaviour();
    public abstract void StopIdleBehaviour();

    public void SetRigidBody(Rigidbody2D rb)
    {
        _RB = rb;
    }
}
