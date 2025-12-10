using System.Collections;
using UnityEngine;

public class EnemyGetUpState : EnemyBaseState
{
    public EnemyGetUpState(Enemy entity, EnemyStateFactory enemyStateFactory, StateMachine<Enemy> stateMachine) : base(entity, enemyStateFactory, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stopLookingAtPlayer = true;

        entity.Agent.enabled = true;
        entity.rb.isKinematic = true;

        enemyBlackboard.rememberedGotHit = true;

        if (entity.LastAttackNode.upStrongAttack)
        {
            animationHandler.CrossFade("Stand_Up", 0.15f);
        }
        else if (entity.LastAttackNode.botStrongAttack || (!entity.LastAttackNode.upStrongAttack && !entity.LastAttackNode.botStrongAttack))
        {
            animationHandler.CrossFade("Getting_Up", 0.15f);
        }
    }

    public override void Exit()
    {
        base.Exit();
        stopLookingAtPlayer = false;

        enemyBlackboard.rememberedGotHit = false;
    }

    public override void Update()
    {
        base.Update();

        //if((animationHandler.IsPlaying("Stand_Up") || animationHandler.IsPlaying("Getting_Up")) && animationHandler.NormalizedTime() >= .8f)
        //{
        //    stateMachine.ChangeState(enemyStateFactory.IdleState);
        //}

        if ((animationHandler.IsPlaying("Stand_Up") || animationHandler.IsPlaying("Getting_Up")) && animationHandler.NormalizedTime() >= .8f && !enemyVision.playerInSight)
        {
            stateMachine.ChangeState(enemyStateFactory.TurnAroundState);
        }
    }

}
