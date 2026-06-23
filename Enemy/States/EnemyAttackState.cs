using UnityEngine;

public class EnemyAttackState : EnemyBaseState
{
    bool hasAttacked;
    public EnemyAttackState(Enemy entity, EnemyStateFactory enemyStateFactory, StateMachine<Enemy> stateMachine) : base(entity, enemyStateFactory, stateMachine) { }

    public override void Enter()    
    {
        base.Enter();

        entity.EnemyCombatBehavior.OnAttackStateEnter();
        enemyBlackboard.applyMovementCorrection = true;
        entity.EnemyNavigation.StopMovement();
        entity.EnemyNavigation.ClearMovement();

        hasAttacked = false;

        entity.EnemyCombatSystem.GetRandomAttack();

        entity.EnemyCombatSystem.ExecuteAttack();       
    }

    public override void Exit()
    {
        base.Exit();

        enemyBlackboard.applyMovementCorrection = false;
        if (entity.EnemyHitBox != null)
        {
            entity.EnemyHitBox.DeactivateEnemyHitBox();
        }

        entity.EnemyNavigation.ResumeMovement();

        entity.EnemyRootMotion.DeactivateRootMotion();

        entity.EnemyCombatBehavior.OnAttackStateExit();

        if (hasAttacked)
        {
            enemyCombatBehavior.OnAttackComplete();
        }

        hasAttacked = false;
        enemyBlackboard.onlyTakeDamage = false;

        enemyBlackboard.isAttacking = false;
    }

    public override void Update()
    {
        base.Update();

        if (animationHandler.IsPlaying(entity.EnemyCombatSystem.CurrentAttackData.attackAnimationName) &&
            animationHandler.NormalizedTime() >= entity.EnemyCombatSystem.CurrentAttackData.attackExitTime)
        {
            hasAttacked = true;
        }

        entity.EnemyCombatBehavior.OnAttackState();
    }
}
