using UnityEngine;

public class BasicEnemyPrayingState : EnemyBaseState
{
    public BasicEnemyPrayingState(Enemy entity, EnemyStateFactory enemyStateFactory, StateMachine<Enemy> stateMachine) : base(entity, enemyStateFactory, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        animationHandler.CrossFade("Praying", 0.1f);
    }

    public override void Exit()
    {
        base.Exit();

        basicEnemy.isPraying = false;

    }

    public override void Update()
    {
        base.Update();

        if (enemyVision.playerInSight)
        {
            entity.coordinator.PermissionToBeActiveAttacker(entity);

            if (!enemyVision.alreadySeenPlayer)
            {
                stateMachine.ChangeState(enemyStateFactory.PreparingtoAttack);
            }
            else
            {
                stateMachine.ChangeState(enemyStateFactory.ChaseState);
            }
        }
    }
}
