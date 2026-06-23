using UnityEngine;

public class MeleeSpecificBehavior : EnemySpecificBehavior
{
    private BasicEnemy enemy;

    public override void InitializeBehavior(Enemy enemy)
    {
        this.enemy = enemy as BasicEnemy;
    }

    public override void UpdateSpecificBehavior()
    {
        if (enemy.EnemyBlackboard.isParried)
        {
            enemy.StateMachine.ChangeState(enemy.EnemyStateFactory.ParriedState);
            return;
        }

        if (enemy.EnemyAttackParticipant.AttackRole == AttackRole.attacker && enemy.attackCoordinatorSystem.TotalEnemies.Count > 1)
        {
            if (enemy.StateMachine.CurrentState == enemy.EnemyStateFactory.AttackState)
            {
                enemy.EnemyAttackParticipant.IsAttackingPlayer = true;
                enemy.attackCoordinatorSystem.CanUpdateEngageTime = false;
                return;
            }
            else
            {
                enemy.EnemyAttackParticipant.IsAttackingPlayer = false;
            }

            if (enemy.IsPlayerInRange(3))
            {
                if (enemy.EnemyVision.isPlayerInSight && !enemy.EnemyBlackboard.isIncapacitated)
                {
                    enemy.EnemyAttackParticipant.isEngagingPlayer = true;
                }
                else
                {
                    enemy.EnemyAttackParticipant.isEngagingPlayer = false;
                }
            }
            else
            {
                enemy.EnemyAttackParticipant.isEngagingPlayer = false;
            }

            if (enemy.IsPlayerInRange(5))
            {
                enemy.EnemyAttackParticipant.UpdateTimeWithoutAttacking(Time.deltaTime);
            }


        }
    }
}
