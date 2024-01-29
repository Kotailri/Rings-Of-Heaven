using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStaticIdle : EnemyIdle
{
    public override void StartIdleBehaviour()
    {
        IsIdling = true;
        _RB.velocity = new Vector2(0, _RB.velocity.y);
    }

    public override void StopIdleBehaviour()
    {
        IsIdling = false;
    }
}
