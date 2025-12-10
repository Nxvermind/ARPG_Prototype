using UnityEngine;

public class EnemyTurnAroundState : EnemyBaseState
{
    public EnemyTurnAroundState(Enemy entity, EnemyStateFactory enemyStateFactory, StateMachine<Enemy> stateMachine) : base(entity, enemyStateFactory, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        entity.Agent.velocity = Vector3.zero;
        entity.Agent.ResetPath();

        stopLookingAtPlayer = true;
        enemyBlackboard.IsTurningAround = true;
        entity.rb.isKinematic = false;
        enemyBlackboard.onlyTakeDamage = true;
        enemyBlackboard.canUpdateStaggerValue = true;

        enemyBlackboard.rememberedGotHit = false;

        if (basicEnemy)
        {
            //animationHandler.CrossFade("TurnAround", 0.1f);
            SpecificTurnAround();
        }

        if (eliteEnemy)
        {
            SpecificTurnAround();
        }
    }

    public override void Exit()
    {
        base.Exit();
        enemyBlackboard.IsTurningAround = false;
        stopLookingAtPlayer = false;
        entity.rb.isKinematic = true;
        enemyBlackboard.onlyTakeDamage = false;
    }

    public override void Update()
    {
        base.Update();

        if (basicEnemy)
        {
            if (enemyVision.playerInSight)
            {
                entity.coordinator.PermissionToBeActiveAttacker(entity);

                stateMachine.ChangeState(enemyStateFactory.ChaseState);
            }
            else
            {
                if (animationHandler.IsPlaying("TurnAround") && animationHandler.NormalizedTime() >= .9f)
                {
                    if(stateMachine.PreviousState == enemyStateFactory.AlertState)
                    {
                        stateMachine.ChangeState(enemyStateFactory.LookingForPlayerState);
                    }
                    else
                    {
                        stateMachine.ChangeState(enemyStateFactory.IdleState);
                    }
                }
            }
        }
        
        if (eliteEnemy)
        {
            if ((animationHandler.IsPlaying("TurnRight") || animationHandler.IsPlaying("TurnLeft") || animationHandler.IsPlaying("TurnAround"))
                 && animationHandler.NormalizedTime() >= .5f)
            {
                if (enemyVision.playerInSight)
                {
                    stateMachine.ChangeState(enemyStateFactory.ChaseState);
                }
                else
                {
                    stateMachine.ChangeState(enemyStateFactory.IdleState);
                }
            }
        }
    }

    private void SpecificTurnAround()
    {

        Vector3 forward = entity.transform.forward;  // Mi dirección actual
        Vector3 targetDir = (target.position - entity.transform.position).normalized;  // Dirección al target

        float angle = Vector3.SignedAngle(forward, targetDir, Vector3.up);  // Ángulo firmado en plano horizontal

        if (angle > -150 && angle < -30)
        {
            animationHandler.CrossFade("TurnLeft", 0.1f);
        }
        else if (angle > 150 && angle < 30)
        {
            animationHandler.CrossFade("TurnRight", 0.1f);
        }
        else
        {
            animationHandler.CrossFade("TurnAround", 0.1f);
        }
    }
}
