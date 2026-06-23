using UnityEngine;

public class EnemyChaseState : EnemyBaseState
{ 
    public EnemyChaseState(Enemy entity, EnemyStateFactory enemyStateFactory, StateMachine<Enemy> stateMachine) : base(entity, enemyStateFactory, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        rootMotion.DeactivateRootMotion();
        agent.speed = enemyParameters.chaseSpeed;
        entity.EnemyLocomotionBehavior.OnChaseStateEnter();

    }

    public override void Exit()
    {
        base.Exit();

        entity.EnemyLocomotionBehavior.OnChaseStateExit();
    }

    public override void Update()
    {
        base.Update();
        entity.EnemyLocomotionBehavior.OnChaseState();
    }
}
