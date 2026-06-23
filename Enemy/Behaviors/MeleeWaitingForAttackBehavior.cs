using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MeleeWaitingForAttackBehavior : BasicEnemyWaitingForAttackBehavior
{
    private BasicEnemy enemy;

    private float updatedTime;

    float idleStartTime;
    bool isWaitingIdle;

    public override void InitializeBehavior(Enemy enemy)
    {
        this.enemy = enemy as BasicEnemy;
    }

    public override void OnWaitingStateEnter()
    {
        isWaitingIdle = false;
        enemy.EnemyAttackParticipant.isEngagingPlayer = false;

        enemy.EnemyAttackParticipant.ZeroTimeWithoutAttacking();

        enemy.AnimationHandler.CrossFade("WaitingForAttack_BlendTree", 0.1f);

        if (enemy.EnemyReposition.TryGetDesiredPosition(out Vector3 point))
        {
            enemy.EnemyNavigation.MoveTo(point);
            updatedTime = Time.time;
        }
    }

    public override void OnWaitingState()
    {
        if (enemy.EnemyAttackParticipant.AttackRole == AttackRole.attacker || !enemy.IsPlayerInRange(20))
        {
            enemy.StateMachine.ChangeState(enemy.EnemyStateFactory.ChaseState);
            return;
        }

        if (isWaitingIdle)
        {
            if (Time.time >= idleStartTime + 2f)
            {
                isWaitingIdle = false;

                enemy.EnemyReposition.ReleaseSlot();

                if (enemy.EnemyReposition.TryGetDesiredPosition(out Vector3 point))
                {
                    enemy.EnemyNavigation.MoveTo(point);
                    enemy.AnimationHandler.CrossFade("WaitingForAttack_BlendTree", 0.1f);
                }
            }

            return; 
        }

        if (enemy.EnemyNavigation.HasReachedDestination())
        {
            enemy.EnemyNavigation.ClearMovement();

            isWaitingIdle = true;
            idleStartTime = Time.time;

            enemy.AnimationHandler.CrossFade("Idle", .1f);

            return;
        }

        if (!enemy.IsPlayerInRange(6))
        {
            //prueba cambiar la posicion del enemigo si el player se aleja cierta distancia y añadele un cooldown
            if (Time.time >= updatedTime + .8f)
            {
                if (enemy.EnemyReposition.TryGetDesiredPosition(out Vector3 point))
                {
                    enemy.EnemyNavigation.MoveTo(point);
                }

                updatedTime = Time.time;
                Debug.Log("player too far, repositioning");
            }
        }

        if (enemy.Agent.desiredVelocity.sqrMagnitude > 0.01f)
        {
            Vector3 dir = enemy.Agent.desiredVelocity.normalized;

            float moveY = Vector3.Dot(enemy.transform.forward, dir);
            float moveX = Vector3.Dot(enemy.transform.right, dir);

            if (moveX >= .7f)
            {
                moveX = 1;
            }

            if (moveY >= .7f)
            {
                moveY = 1;
            }

            enemy.AnimationHandler.Anim.SetFloat("MoveX", moveX, 0.1f, Time.deltaTime);
            enemy.AnimationHandler.Anim.SetFloat("MoveY", moveY, 0.1f, Time.deltaTime);
        }
    }

    public override void OnWaitingStateExit()
    {
        isWaitingIdle = false;
        enemy.EnemyNavigation.ClearMovement();
        enemy.EnemyReposition.ReleaseSlot();
    }
}
