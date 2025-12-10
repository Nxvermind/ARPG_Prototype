using System.Collections;
using UnityEngine;

public class EnemyParriedState : EnemyBaseState
{
    public EnemyParriedState(Enemy entity, EnemyStateFactory enemyStateFactory, StateMachine<Enemy> stateMachine) : base(entity, enemyStateFactory, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        agent.ResetPath();

        entity.EnemyParryStatus.isParried = false;

        enemyBlackboard.canUpdateStaggerValue = true;

        entity.rb.isKinematic = false;
        entity.Agent.enabled = false;

        animationHandler.Play("Parried");
    }

    public override void Exit()
    {
        base.Exit();

        enemyBlackboard.canUpdateStaggerValue = false;
        entity.EnemyBlackboard.onlyTakeDamage = false;

        entity.rb.isKinematic = true;
        entity.Agent.enabled = true;

    }

    public override void Update()
    {
        base.Update();

        if(animationHandler.NormalizedTime() >= .9f)
        {
            stateMachine.ChangeState(enemyStateFactory.IdleState);
        }
    }
}
