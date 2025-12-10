using UnityEngine;

public class EnemyGroundGotHitState : EnemyGotHitState
{
    public EnemyGroundGotHitState(Enemy entity, EnemyStateFactory enemyStateFactory, StateMachine<Enemy> stateMachine) : base(entity, enemyStateFactory, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        entity.Agent.velocity = Vector3.zero;
        entity.Agent.ResetPath();
        entity.Agent.isStopped = true;

        if (enemyParameters.currentStaggerValue >= enemyParameters.maxStaggerValue)
        {
            enemyBlackboard.onlyTakeDamage = true;
            enemyBlackboard.canUpdateStaggerValue = false;
        }
        else
        {
            enemyBlackboard.canUpdateStaggerValue = true;
        }

        enemyBlackboard.rememberedGotHit = true;

        Vector3 enemyVector = entity.transform.forward;
        Vector3 d = (target.position - entity.transform.position).normalized;

        float dot = Vector3.Dot(enemyVector, d);

        if (enemyBlackboard.gotHitByUltimateSkill)
        {
            if (dot > 0)
            {
                animationHandler.Play("Got_Hit_F");
            }
            else if (dot < 0)
            {
                animationHandler.Play("Got_Hit_B");
            }
        }

        if (eliteEnemy)
        {
            if (dot > 0)
            {
                if (dot > 0)
                {
                    animationHandler.Play("Got_Hit_F");
                }
                else if (dot < 0)
                {
                    animationHandler.Play("Got_Hit_B");
                    enemyBlackboard.onlyTakeDamage = true;
                }
            }

            return;
        }

        if (basicEnemy)
        {
            if(enemyBlackboard.gotHitByDashSkill)
            {
                enemyBlackboard.rememberedGotHit = true;
                if (dot > 0)
                {
                    animationHandler.Play("Got_Hit_F");
                }
                else if (dot < 0)
                {
                    animationHandler.Play("Got_Hit_B");
                }
                return;
            }

            if (entity.LastAttackNode != null && entity.LastAttackNode.upStrongAttack)
            {
                Levitate();
                animationHandler.Play("StrongGotHit_Up");
            }
            else if (entity.LastAttackNode != null && entity.LastAttackNode.botStrongAttack)
            {
                Levitate();
                animationHandler.Play("Got_Hit_Up");
            }
            else
            {
                if (dot > 0)
                {
                    animationHandler.Play("Got_Hit_F");
                }
                else if (dot < 0)
                {
                    animationHandler.Play("Got_Hit_B");
                    enemyBlackboard.onlyTakeDamage = true;
                    enemyBlackboard.canUpdateStaggerValue = true;
                }
            }
        }

    }

    public override void Exit()
    {
        base.Exit();

        entity.rb.isKinematic = true;
        entity.Agent.enabled = true;
        entity.Agent.isStopped = false;
        entity.EnemyBlackboard.onlyTakeDamage = false;
        enemyBlackboard.gotHitByUltimateSkill = false;

        if (enemyBlackboard.gotHitByDashSkill)
        {
            enemyBlackboard.gotHitByDashSkill = false;
        }

        if (enemyParameters.currentStaggerValue >= enemyParameters.maxStaggerValue)
        {
            enemyParameters.currentStaggerValue = enemyParameters.maxStaggerValue;
        }

    }

    public override void Update()
    {
        base.Update();


        if (animationHandler.IsPlaying("StrongGotHit_Up") && animationHandler.NormalizedTime() >= .9f && entity.EnemyGroundDetection.isGrounded)
        {
            stateMachine.ChangeState(enemyStateFactory.GetUpState);
        }
        else if(animationHandler.IsPlaying("StrongGotHit_Up") && animationHandler.NormalizedTime() >= .9f && !entity.EnemyGroundDetection.isGrounded)
        {
            stateMachine.ChangeState(basicEnemyStateFactory.FallingState);
        }

        if (animationHandler.IsPlaying("Got_Hit_Up") && animationHandler.NormalizedTime() >= .9f && entity.EnemyGroundDetection.isGrounded)
        {
            stateMachine.ChangeState(enemyStateFactory.GetUpState);
        }
        else if (animationHandler.IsPlaying("Got_Hit_Up") && animationHandler.NormalizedTime() >= .9f && !entity.EnemyGroundDetection.isGrounded)
        {
            stateMachine.ChangeState(basicEnemyStateFactory.FallingState);
        }

        if (enemyVision.playerInSight)
        {
            enemyBlackboard.rememberedGotHit = false;

            if ((animationHandler.IsPlaying("Got_Hit_B") || animationHandler.IsPlaying("Got_Hit_F")) && animationHandler.NormalizedTime() >= 0.9f)
            {
                stateMachine.ChangeState(enemyStateFactory.IdleState);
            }
        }
        else
        {
            if ((animationHandler.IsPlaying("Got_Hit_B") || animationHandler.IsPlaying("Got_Hit_F")) && animationHandler.NormalizedTime() >= 0.9f)
            {
                stateMachine.ChangeState(enemyStateFactory.TurnAroundState);
            }
        }

    }
}
