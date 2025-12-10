using UnityEngine;

public class EnemyChaseState : EnemyBaseState
{
    float lastTime;

    Vector3 velocity = Vector3.zero;

    public EnemyChaseState(Enemy entity, EnemyStateFactory enemyStateFactory, StateMachine<Enemy> stateMachine) : base(entity, enemyStateFactory, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        entity.coordinator.PermissionToBeActiveAttacker(entity);

        agent.updatePosition = false;

        agent.isStopped = false;

        lastTime = Time.time;
        animationHandler.CrossFade("Chase", 0.12f);
        entity.Agent.speed = enemyParameters.chaseSpeed;   
    }

    public override void Exit()
    {
        base.Exit();
        agent.updatePosition = true;
    }

    public override void Update()
    {
        base.Update();

        if (!entity.EnemyCombatStatus.isAllowedToAttack)
        {
            stateMachine.ChangeState(enemyStateFactory.WaitingForAttackState);
        }

        entity.rb.position = Vector3.SmoothDamp(entity.rb.position, agent.nextPosition, ref velocity, 0.01f);

        if (basicEnemy)
        {
            if (entity.EnemyCombatStatus.waitingForAttackPermission && !entity.EnemyCombatStatus.isActiveAttacker)
            {
                if (entity.IsPlayerInRange(5))
                {
                    stateMachine.ChangeState(enemyStateFactory.WaitingForAttackState);
                }
            }

            if (enemyVision.playerInSight)
            {
                if (entity.Agent.enabled && Time.time >= lastTime + .1f && enemyVision.playerInSight)
                {
                    lastTime = Time.time;
                    agent.SetDestination(target.position);
                    enemyBlackboard.lastKnownPlayerPosition = target.position;
                }
            }
            else
            {
                agent.SetDestination(enemyBlackboard.lastKnownPlayerPosition);

                if (HasReachedDestination())
                {
                    stateMachine.ChangeState(enemyStateFactory.AlertState);
                }
            }

            if (entity.enemyType == EnemyType.Melee)
            {
                if (InRangeToAttack() && entity.EnemyCombatStatus.isActiveAttacker)
                {
                    stateMachine.ChangeState(enemyStateFactory.AttackState);
                }
            }
            else if (entity.enemyType == EnemyType.Range)
            {
                if (InRangeToAttack() && entity.EnemyCombatStatus.isActiveAttacker && entity.EnemyCombatStatus.readyToAttack)
                {
                    stateMachine.ChangeState(enemyStateFactory.AttackState);
                }
            }
        }

        if (eliteEnemy)
        {
            if (enemyVision.playerInSight)
            {
                if (entity.Agent.enabled && Time.time >= lastTime + .1f)
                {
                    lastTime = Time.time;
                    agent.SetDestination(target.position);
                    enemyBlackboard.lastKnownPlayerPosition = target.position;
                }
            }
            else
            {
                stateMachine.ChangeState(enemyStateFactory.TurnAroundState);
            }

            if (InRangeToAttack())
            {
                stateMachine.ChangeState(enemyStateFactory.AttackState);
            }

            if (enemyVision.playerDisappeared)
            {
                stateMachine.ChangeState(enemyStateFactory.TurnAroundState);
            }
        }
    }
}
