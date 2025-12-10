using UnityEngine;

public class EnemyBackToPatrolPointState : EnemyBaseState
{
    Vector3 velocity = Vector3.zero;

    public EnemyBackToPatrolPointState(Enemy entity, EnemyStateFactory enemyStateFactory, StateMachine<Enemy> stateMachine) : base(entity, enemyStateFactory, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        agent.updatePosition = false;

        agent.speed = enemyParameters.aggresiveWalkSpeed;

        BackToPatrolPoint();
    }

    public override void Exit()
    {
        base.Exit();
        agent.updatePosition = true;
    }

    public override void Update()
    {
        base.Update();

        entity.transform.position = Vector3.SmoothDamp(entity.transform.position, agent.nextPosition, ref velocity, 0);

        if (HasReachedDestination())
        {
            stateMachine.ChangeState(basicEnemy.basicEnemyStateFactory.PatrolState);
        }

        if (enemyVision.playerInSight)
        {
            stateMachine.ChangeState(enemyStateFactory.ChaseState);
        }

    }

    private void BackToPatrolPoint()
    {
        agent.SetDestination(basicEnemy.patrolPoints[0].position);
    }
}
