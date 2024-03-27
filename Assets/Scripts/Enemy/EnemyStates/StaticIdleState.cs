using UnityEngine;

public class StaticIdleState : MonoBehaviour, IEnemyIdleState
{
    private Rigidbody2D _RB;

    private void Awake()
    {
        _RB = GetComponent<Rigidbody2D>();
    }

    public void OnStateEnter()
    {
        _RB.velocity = Vector3.zero;
    }

    public void OnStateExit() { }

    public void OnStateUpdate() { }
    public void OnStateResumed() { }
    public void OnStatePaused() { }
}
