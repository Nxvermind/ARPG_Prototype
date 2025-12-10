using System.Collections;
using UnityEngine;

public class EnemyWaitingForAttackState : EnemyBaseState
{
    private float originalTime;
    public EnemyWaitingForAttackState(Enemy entity, EnemyStateFactory enemyStateFactory, StateMachine<Enemy> stateMachine) : base(entity, enemyStateFactory, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        animationHandler.CrossFade("WaitingForAttack", 0.1f);

        originalTime = Time.time;
        
        entity.Agent.ResetPath();

        entity.Agent.updateRotation = false;

        agent.speed = 1.5f;
        entity.StartCoroutine(MovePositionWhileWaitingForAttackPermission());
    }

    public override void Exit()
    {
        base.Exit();

        entity.Agent.updateRotation = true;
        entity.EnemyCombatStatus.readyToAttack = false;
        entity.StopAllCoroutines();
    }

    public override void Update()
    {
        base.Update();

        if (!entity.EnemyCombatStatus.isAllowedToAttack) return;

        if (entity.EnemyCombatStatus.isActiveAttacker)
        {
            if (entity.enemyType == EnemyType.Range)
            {
                if (!entity.IsPlayerInRange(15))
                {
                    stateMachine.ChangeState(enemyStateFactory.ChaseState);
                    return;
                }

                if (entity.EnemyCombatStatus.readyToAttack)
                {
                    stateMachine.ChangeState(enemyStateFactory.AttackState);
                }
            }
            else if (entity.enemyType == EnemyType.Melee)
            {
                stateMachine.ChangeState(enemyStateFactory.ChaseState);
            }
        }
        else
        {
            if (entity.enemyType == EnemyType.Melee)
            {

                if (!entity.IsPlayerInRange(10))
                {
                    stateMachine.ChangeState(enemyStateFactory.AggresiveWalkState);
                }

                if (entity.IsPlayerInRange(3))
                {
                    Debug.Log("player is close");

                    if (Time.time >= originalTime + entity.EnemyCombatStatus.timeToGrantAttackPermission)
                    {
                        entity.coordinator.GrantMeleeAttackerPermissionToAttack(entity);
                    }
                }
                else
                {
                    originalTime = Time.time;
                }
            }
        }

    }

    IEnumerator MovePositionWhileWaitingForAttackPermission()
    {
        if(entity.enemyType == EnemyType.Melee)
        {
            while (true)
            {
                //if (enemySearchSystem.GetReachableRandomPosition(target.position, agent, 6, out Vector3 result))
                //{
                //    agent.SetDestination(result);
                //}

                if (enemySearchSystem.GetReachableRandomPositionBehind(entity.transform.position, agent, 6, out Vector3 result))
                {
                    agent.SetDestination(result);
                }

                yield return new WaitUntil(() => HasReachedDestination());
            }
        }

        if(entity.enemyType == EnemyType.Range)
        {
            while (true)
            {
                if (enemySearchSystem.GetReachableRandomPositionBehind(entity.transform.position, agent, 7, out Vector3 result))
                {
                    agent.SetDestination(result);
                }

                yield return new WaitUntil(() => HasReachedDestination());
            }
        }
    }
}
