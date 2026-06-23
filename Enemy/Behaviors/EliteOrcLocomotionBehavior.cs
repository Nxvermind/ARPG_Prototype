using System.Collections;
using UnityEngine;

public class EliteOrcLocomotionBehavior : EnemyLocomotionBehavior
{
    private EliteEnemy enemy;
    float lastTime;
    public override void InitializeLocomotionBehavior(Enemy enemy)
    {
        this.enemy = enemy as EliteEnemy;
    }

    public override void OnAggresiveWalkState()
    {
        if (enemy.EnemyVision.isPlayerInSight)
        {
            if (enemy.Agent.enabled && Time.time >= lastTime + .1f)
            {
                lastTime = Time.time;
                enemy.Agent.SetDestination(enemy.target.position);
            }

            if (enemy.EnemyAttackRules.InRangeToAttack(enemy.target, enemy.transform))
            {
                enemy.StateMachine.ChangeState(enemy.EnemyStateFactory.AttackState);
            }
        }
        else
        {
            enemy.StateMachine.ChangeState(enemy.EnemyStateFactory.IdleState);
        }

        if (enemy.EnemyVision.playerDisappeared)
        {
            enemy.StateMachine.ChangeState(enemy.EnemyStateFactory.TurnAroundState);
        }

        if (Time.time >= enemy.EnemyBlackboard.lastTimeSinceAttack + 2)
        {
            enemy.StateMachine.ChangeState(enemy.EnemyStateFactory.ChaseState);
        }

    }

    public override void OnAggresiveWalkStateEnter()
    {
        lastTime = Time.time;
        enemy.AnimationHandler.CrossFade("AggresiveWalk", 0.14f);
    }

    public override void OnChaseState()
    {
        if (enemy.EnemyVision.isPlayerInSight)
        {
            if (enemy.Agent.enabled && Time.time >= lastTime + .1f)
            {
                lastTime = Time.time;
                enemy.Agent.SetDestination(enemy.target.position);
            }

            if (enemy.EnemyAttackRules.InRangeToAttack(enemy.target, enemy.transform))
            {

                enemy.StateMachine.ChangeState(enemy.EnemyStateFactory.AttackState);
            }
        }
        else
        {
            enemy.StateMachine.ChangeState(enemy.EnemyStateFactory.IdleState);
        }

        if (enemy.EnemyVision.playerDisappeared)
        {
            enemy.StateMachine.ChangeState(enemy.EnemyStateFactory.TurnAroundState);
        }
    }

    public override void OnChaseStateEnter()
    {
        lastTime = Time.time;
        enemy.AnimationHandler.CrossFade("Chase", .1f);
    }

    public override void OnChaseStateExit()
    {
        
    }

    public override void OnIdleState()
    {
        if (enemy.ignorePlayer) return;
        if (!enemy.EnemyVision.wasPlayerDetected) return;

        if (enemy.EnemyVision.isPlayerInSight)
        {

            enemy.StateMachine.ChangeState(enemy.EnemyStateFactory.ChaseState);
        }
        else
        {
            enemy.StateMachine.ChangeState(enemy.EnemyStateFactory.TurnAroundState);
        }
    }

    public override void OnIdleStateEnter()
    {
        enemy.AnimationHandler.CrossFade("Idle", .1f);
    }
}
