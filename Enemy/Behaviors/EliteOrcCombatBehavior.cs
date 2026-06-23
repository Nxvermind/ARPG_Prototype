using UnityEngine;

public class EliteOrcCombatBehavior : EnemyCombatBehavior
{
    private EliteEnemy enemy;

    public override void InitializeEnemyBehavior(Enemy enemy)
    {
        this.enemy = enemy as EliteEnemy;
    }

    public override void OnAttackState()
    {
        if (enemy.AnimationHandler.IsPlaying(enemy.EnemyCombatSystem.CurrentAttackData.attackAnimationName) &&
            enemy.AnimationHandler.NormalizedTime() >= .9f && !enemy.EnemyCombatSystem.canPlayNextAttack)
        {
            if (enemy.IsPlayerInRange(5) && enemy.EnemyVision.isPlayerInSight)
            {
                enemy.StateMachine.ChangeState(enemy.eliteEnemyStateFactory.RepositioningState);
                return;
            }

            enemy.StateMachine.ChangeState(enemy.EnemyStateFactory.AggresiveWalkState);
        }

        if(enemy.AnimationHandler.IsPlaying(enemy.EnemyCombatSystem.CurrentAttackData.attackAnimationName) && 
            enemy.AnimationHandler.NormalizedTime() >= .9f && !enemy.EnemyVision.isPlayerInSight)
        {
            enemy.StateMachine.ChangeState(enemy.EnemyStateFactory.TurnAroundState);
            return;
        }
    }

    public override void OnAttackComplete()
    {
        enemy.EnemyCombatSystem.ResetCombo();

        enemy.EnemyBlackboard.lastTimeSinceAttack = Time.time;
    }

    public override void OnAttackStateEnter()
    {
        enemy.EnemyBlackboard.isAttacking = true;
    }

    public override void OnAttackStateExit()
    {
        enemy.EnemyBlackboard.OnlyTakeDamageInactive();
    }
}
