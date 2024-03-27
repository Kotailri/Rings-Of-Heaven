public interface IEnemyBehaviourState
{
    public void OnStateEnter();
    public void OnStateUpdate();
    public void OnStateExit();
    public void OnStateResumed();
    public void OnStatePaused();
}

public interface IEnemyIdleState : IEnemyBehaviourState { }
public interface IEnemyAttackState : IEnemyBehaviourState { public UnityEngine.Vector2 GetAttackDetectionArea(); }
public interface IEnemyChaseState : IEnemyBehaviourState { }


public interface IEnemyController
{
    public void PauseController(float time);
}