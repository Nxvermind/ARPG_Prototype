using UnityEngine;

public class EnemyStaggerState : EnemyBaseState
{
    public EnemyStaggerState(Enemy entity, EnemyStateFactory enemyStateFactory, StateMachine<Enemy> stateMachine) : base(entity, enemyStateFactory, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        if (!agent.enabled) agent.enabled = true;

        enemyBlackboard.isIncapacitated = true;


        enemyBlackboard.onlyTakeDamage = true;
        enemyBlackboard.canUpdateStaggerValue = false;

        entity.EnemyNavigation.StopMovement();
        entity.EnemyNavigation.ClearMovement();

        if(DirectionUtility.Dot(entity.transform, target) >= 0.7f)
        {
            entity.FacingSystem.FaceTarget(target);
        }


        enemyBlackboard.canLookAtPlayer = false;

        animationHandler.Play("Stagger");

        if (basicEnemy)
        {
            basicEnemy.ThisEnemyIsReadyToBeExecuted(entity);
        }
    }

    public override void Exit()
    {
        base.Exit();

        enemyBlackboard.isIncapacitated = false;

        enemyParameters.currentStaggerValue = 0;

        enemyBlackboard.canLookAtPlayer = true;
        entity.EnemyNavigation.ResumeMovement();
        enemyVision.playerDisappeared = false;

        entity.EnemyBlackboard.onlyTakeDamage = false;
        enemyBlackboard.canUpdateStaggerValue = true;

        if (basicEnemy && !entity.EnemyBlackboard.wasExecuted)
        {
            basicEnemy.executionImageGO.SetActive(false);
            basicEnemy.executionManager.RemoveEnemy(entity);
        }

        inStaggerState = false;
    }

    public override void Update()
    {
        base.Update();

        if (animationHandler.IsPlaying("Stagger") && animationHandler.NormalizedTime() >= 1 && enemyVision.isPlayerInSight)
        {
            stateMachine.ChangeState(enemyStateFactory.IdleState);
        }

        if(animationHandler.IsPlaying("Stagger") &&  animationHandler.NormalizedTime() >= 1 && !enemyVision.isPlayerInSight)
        {
            stateMachine.ChangeState(enemyStateFactory.TurnAroundState);
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
