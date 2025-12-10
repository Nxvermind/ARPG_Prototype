using UnityEngine;

public class EnemyAggresiveWalkState : EnemyBaseState
{
    float internTimer;
    float lastTime;
    Vector3 velocity = Vector3.zero;

    public EnemyAggresiveWalkState(Enemy entity, EnemyStateFactory enemyStateFactory, StateMachine<Enemy> stateMachine) : base(entity, enemyStateFactory, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        agent.updatePosition = false;
        animationHandler.CrossFade("AggresiveWalk", 0.14f);

        entity.Agent.speed = enemyParameters.aggresiveWalkSpeed;


    }

    public override void Exit()
    {
        base.Exit();
        internTimer = 0;
        agent.updatePosition = true;
    }

    public override void Update()
    {
        base.Update();

        if (!entity.EnemyCombatStatus.isAllowedToAttack) return;

        entity.transform.position = Vector3.SmoothDamp(entity.transform.position, agent.nextPosition, ref velocity, 0.01f);

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

            internTimer += Time.deltaTime;

            if (entity.Agent.enabled && Time.time >= lastTime + .2f)
            {
                lastTime = Time.time;
                agent.SetDestination(target.position);
            }

            if (internTimer >= enemyParameters.aggresiveWalkTime)
            {
                stateMachine.ChangeState(enemyStateFactory.ChaseState);
            }

            return;
        }

        if (basicEnemy)
        {
            if (!entity.EnemyCombatStatus.isActiveAttacker)
            {
                if (entity.EnemyCombatStatus.waitingForAttackPermission)
                {
                    if (entity.IsPlayerInRange(5))
                    {
                        stateMachine.ChangeState(enemyStateFactory.WaitingForAttackState);
                    }
                }
            }
            else
            {
                stateMachine.ChangeState(enemyStateFactory.ChaseState);
            }

            if (enemyVision.playerInSight)
            {
                if (entity.Agent.enabled && Time.time >= lastTime + .1f)
                {
                    lastTime = Time.time;
                    agent.SetDestination(target.position);
                    enemyBlackboard.lastKnownPlayerPosition = target.position;
                }
            }

            if (entity.enemyType == EnemyType.Range)
            {
                if (InRangeToAttack() && entity.EnemyCombatStatus.isActiveAttacker && entity.EnemyCombatStatus.readyToAttack)
                {
                    stateMachine.ChangeState(enemyStateFactory.AttackState);
                }
            }
        }




    }
}
