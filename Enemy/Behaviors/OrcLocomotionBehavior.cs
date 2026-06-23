using UnityEngine;

public class OrcLocomotionBehavior : EnemyLocomotionBehavior
{
    private BasicEnemy enemy;
    float lastTime;

    bool b;

    float updateTime;
    float originalUpdateTime;
    public override void InitializeLocomotionBehavior(Enemy enemy)
    {
        this.enemy = enemy as BasicEnemy;
    }

    public override void OnChaseState()
    {
        if (enemy.EnemyAttackParticipant.AttackRole == AttackRole.waiting)
        {
            if (enemy.IsPlayerInRange(10))
            {
                enemy.StateMachine.ChangeState(enemy.EnemyStateFactory.WaitingForAttackState);
                return;
            }
        }

        if (enemy.EnemyVision.isPlayerInSight)
        {
            if (enemy.Agent.enabled && Time.time >= lastTime + .1f)
            {
                lastTime = Time.time;
                enemy.EnemyNavigation.MoveTo(enemy.target.position);
                enemy.EnemyBlackboard.lastKnownPlayerPosition = enemy.target.position;
            }
            b = false;
        }
        else
        {
            if (!b)
            {
                lastTime = Time.time;
                b = true;
            }

            if(Time.time <= lastTime + .3f)
            {
                enemy.EnemyBlackboard.lastKnownPlayerPosition = enemy.target.position;

                updateTime -= Time.deltaTime;

                if(updateTime < 0)
                {
                    updateTime = originalUpdateTime;
                    enemy.EnemyNavigation.MoveTo(enemy.EnemyBlackboard.lastKnownPlayerPosition);
                }
            }


            Vector3 moveDir = (enemy.Agent.steeringTarget - enemy.transform.position).normalized;

            enemy.FacingSystem.RotateTowardsDirection(moveDir, 12);

            if (enemy.EnemyNavigation.HasReachedDestination())
            {
                enemy.StateMachine.ChangeState(enemy.EnemyStateFactory.AlertState);
                return;
            }
        }

        if (enemy.EnemyAttackRules.InRangeToAttack(enemy.target, enemy.transform) &&
             enemy.EnemyAttackParticipant.AttackRole == AttackRole.attacker)
        {
            enemy.StateMachine.ChangeState(enemy.EnemyStateFactory.AttackState);
            return;
        }


    }

    public override void OnChaseStateEnter()
    {
        lastTime = Time.time;
        b = false;
        updateTime = .15f;
        originalUpdateTime = updateTime;
        enemy.AnimationHandler.Anim.Play("Chase_Start");
    }

    public override void OnChaseStateExit()
    {
        enemy.EnemyNavigation.ClearMovement();
    }

    public override void OnIdleState()
    {
        if (enemy.ignorePlayer) return;

        if (enemy.EnemyVision.isPlayerInSight && !enemy.attackCoordinatorSystem.TotalEnemies.Contains(enemy.EnemyAttackParticipant))
        {
            enemy.attackCoordinatorSystem.Register(enemy.EnemyAttackParticipant);
        }

        if (enemy.EnemyAttackParticipant.AttackRole == AttackRole.attacker)
        {
            if (enemy.EnemyVision.alreadySeenPlayer)
            {
                enemy.StateMachine.ChangeState(enemy.EnemyStateFactory.ChaseState);
                return;

            }
            else
            {
                enemy.StateMachine.ChangeState(enemy.EnemyStateFactory.PreparingtoAttack);
                return;
            }

        }

        if (enemy.EnemyAttackParticipant.AttackRole == AttackRole.waiting)
        {
            enemy.StateMachine.ChangeState(enemy.EnemyStateFactory.WaitingForAttackState);
            return;
        }

        if (enemy.EnemyVision.wasPlayerDetected && !enemy.EnemyVision.isPlayerInSight)
        {
            enemy.StateMachine.ChangeState(enemy.EnemyStateFactory.TurnAroundState);
            return;
        }
    }

    public override void OnIdleStateEnter()
    {
        if (enemy.EnemyBlackboard.rememberedGotHit && !enemy.EnemyVision.isPlayerInSight)
        {
            enemy.StateMachine.ChangeState(enemy.EnemyStateFactory.TurnAroundState);
            return;
        }
    }
}
