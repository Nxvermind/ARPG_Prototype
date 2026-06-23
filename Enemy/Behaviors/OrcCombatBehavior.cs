using System.Collections;
using UnityEngine;

public class OrcCombatBehavior : EnemyCombatBehavior
{
    private BasicEnemy enemy;

    public override void InitializeEnemyBehavior(Enemy enemy)
    {
        this.enemy = enemy as BasicEnemy;
    }

    public override void OnAttackState()
    {
        if (enemy.EnemyAttackParticipant.AttackRole == AttackRole.waiting)
        {
            enemy.StateMachine.ChangeState(enemy.EnemyStateFactory.WaitingForAttackState);
            return;
        }

        if (enemy.AnimationHandler.IsPlaying(enemy.EnemyCombatSystem.CurrentAttackData.attackAnimationName) && enemy.AnimationHandler.NormalizedTime() >= .9f)
        {
            if (enemy.EnemyVision.isPlayerInSight)
            {
                enemy.EnemyBlackboard.canLookAtPlayer = true;
                enemy.StateMachine.ChangeState(enemy.EnemyStateFactory.IdleState);
            }
            else
            {
                enemy.StateMachine.ChangeState(enemy.EnemyStateFactory.TurnAroundState);
            }

            return;
        }

        if (!enemy.IsPlayerInRange(1))
        {
            enemy.EnemyRootMotion.ActivateRootMotion();
        }
        else
        {
            enemy.EnemyRootMotion.DeactivateRootMotion();
        }
    }

    public override void OnAttackComplete()
    {
        enemy.attackCoordinatorSystem.ParticipantAttackComplete(enemy.EnemyAttackParticipant);

        enemy.EnemyAttackParticipant.ZeroTimeWithoutAttacking();
    }

    public override void OnAttackStateEnter()
    {
        
    }
}
