using UnityEngine;

public class EnemyStaggerState : EnemyBaseState
{
    public EnemyStaggerState(Enemy entity, EnemyStateFactory enemyStateFactory, StateMachine<Enemy> stateMachine) : base(entity, enemyStateFactory, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        if(enemyParameters.currentStaggerValue >= enemyParameters.maxStaggerValue)
        {
            enemyBlackboard.onlyTakeDamage = true;
            enemyBlackboard.canUpdateStaggerValue = false;
        }


        entity.Agent.enabled = true;

        entity.Agent.isStopped = true;
        entity.Agent.velocity = Vector3.zero;

        stopLookingAtPlayer = true;
        animationHandler.CrossFade("Stagger", 0.1f);

        if (basicEnemy)
        {
            basicEnemy.ThisEnemyIsReadyToBeExecuted(entity);
        }

        enemyBlackboard.positionToStartSearching = entity.transform.position;
    }

    public override void Exit()
    {
        base.Exit();

        if (enemyParameters.currentStaggerValue >= enemyParameters.maxStaggerValue)
        {
            enemyParameters.currentStaggerValue = 0;
        }

        stopLookingAtPlayer = false;
        entity.Agent.isStopped = false;
        enemyVision.playerDisappeared = false;

        entity.EnemyBlackboard.onlyTakeDamage = false;
        enemyBlackboard.canUpdateStaggerValue = true;

        if (basicEnemy && !entity.EnemyBlackboard.wasExecuted)
        {
            basicEnemy.executionImageGO.SetActive(false);
            basicEnemy.executionManager.RemoveEnemy(entity);
        }
    }

    public override void Update()
    {
        base.Update();

        if (animationHandler.IsPlaying("Stagger") && animationHandler.NormalizedTime() >= 1 && enemyVision.playerInSight)
        {
            stateMachine.ChangeState(enemyStateFactory.IdleState);
        }

        if(animationHandler.IsPlaying("Stagger") &&  animationHandler.NormalizedTime() >= 1 && !enemyVision.playerInSight)
        {
            stateMachine.ChangeState(enemyStateFactory.AlertState);
        }

        if (basicEnemy)
        {
            Vector3 directionToPlayer = target.position - entity.transform.position;

            float angle = Vector3.Angle(entity.transform.forward, directionToPlayer.normalized);

            if(angle < 60 && entity.IsPlayerInRange(3) && enemyParameters.currentStaggerValue >= enemyParameters.maxStaggerValue)
            {
                enemyBlackboard.canExecuteThisEnemy = true;
                basicEnemy.executionImageGO.SetActive(true);
            }
            else
            {
                enemyBlackboard.canExecuteThisEnemy = false;
                basicEnemy.executionImageGO.SetActive(false);
            }
        }
    }
}
