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

        agent.enabled = true;
        enemyBlackboard.availableForTeleportSkill = true;
        enemyBlackboard.canLookAtPlayer = false;

        if (entity.EnemyHitReciever.StrongAttackNode != null)
        {
            if (entity.EnemyHitReciever.StrongAttackNode.attackDirection == AttackDirection.Up)
            {
                animationHandler.CrossFade("Stand_Up", 0.15f);
            }
            else if (entity.EnemyHitReciever.StrongAttackNode.hitReactionType == HitReactionType.PushedBack)
            {
                animationHandler.CrossFade("Get_Up_Front", .1f);
            }
        }
        else
        {
            if (animationHandler.IsPlaying("StrongGotHit_Up") || animationHandler.IsPlaying("AirGot_Hit_Down"))
            {
                animationHandler.CrossFade("Stand_Up", 0.15f);
            }
            else if (animationHandler.IsPlaying("StrongGotHit_F") || animationHandler.IsPlaying("AirGot_Hit_Up"))
            {
                animationHandler.CrossFade("Get_Up_Front", .1f);
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        enemyBlackboard.canLookAtPlayer = true;

        basicEnemy.EnemyAttackParticipant.isPushedBack = false;
        rootMotion.DeactivateRootMotion();
    }

    public override void Update()
    {
        base.Update();
        //Debug.Log($"root motion is {rootMotion.useRootMotion}");

        if (animationHandler.IsPlaying("Get_Up_Front"))
        {
            rootMotion.ActivateRootMotion();
        }

        if (!enemyVision.isPlayerInSight)
        {   
            if ((animationHandler.IsPlaying("Stand_Up") ||
            animationHandler.IsPlaying("Getting_Up") ||
            animationHandler.IsPlaying("Get_Up_Front"))
            && animationHandler.NormalizedTime() >= .9f)
            {
                stateMachine.ChangeState(enemyStateFactory.TurnAroundState);

            }
            return;
        }

        if(animationHandler.IsPlaying("Get_Up_Front") && animationHandler.NormalizedTime() >= 1)
        {
            stateMachine.ChangeState(enemyStateFactory.IdleState);
        }

        if(animationHandler.IsPlaying("Stand_Up") && animationHandler.NormalizedTime() >= .75f)
        {
            stateMachine.ChangeState(enemyStateFactory.IdleState);
        }
    }

}
