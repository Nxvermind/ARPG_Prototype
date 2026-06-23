using System.Collections;
using UnityEngine;

public class EnemyIdleState : EnemyBaseState
{
    public EnemyIdleState(Enemy entity, EnemyStateFactory enemyStateFactory, StateMachine<Enemy> stateMachine) : base(entity, enemyStateFactory, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        enemyBlackboard.canLookAtPlayer = true;
        animationHandler.Anim.CrossFade("Idle", .15f);

        rootMotion.ActivateRootMotion();

        entity.EnemyLocomotionBehavior.OnIdleStateEnter();
    }

    public override void Exit()
    {
        base.Exit();
        rootMotion.DeactivateRootMotion();
    }

    public override void Update()
    {
        base.Update();

        entity.EnemyLocomotionBehavior.OnIdleState();
    }
}
