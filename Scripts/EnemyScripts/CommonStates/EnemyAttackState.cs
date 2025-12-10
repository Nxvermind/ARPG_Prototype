using UnityEngine;

public class EnemyAttackState : EnemyBaseState
{
    bool hasAttacked;
    public EnemyAttackState(Enemy entity, EnemyStateFactory enemyStateFactory, StateMachine<Enemy> stateMachine) : base(entity, enemyStateFactory, stateMachine) { }

    public override void Enter()    
    {
        base.Enter();
        entity.Agent.isStopped = true;
        entity.Agent.velocity = Vector3.zero;
        entity.Agent.ResetPath();

        hasAttacked = false;

        enemyBlackboard.isAttacking = true;



        EnemyAttackData enemyAttackData = entity.EnemyCombatSystem.GetRandomAttack();

        entity.EnemyCombatSystem.ExecuteAttack(enemyAttackData);
    }

    public override void Exit()
    {
        base.Exit();
        entity.Agent.isStopped = false;
        entity.AnimationHandler.Anim.applyRootMotion = false;
        enemyBlackboard.isAttacking = false;

        if(entity.enemyType == EnemyType.Melee)
        {
            if (hasAttacked && !entity.EnemyParryStatus.isParried)
            {
                if (!entity.EnemyCombatStatus.oneTimeAttackPermission)
                {
                    entity.coordinator.ChangeActiveAttacker(entity);
                }
            }

            if (entity.EnemyCombatStatus.oneTimeAttackPermission)
            {
                entity.coordinator.ResetAttackPermission(entity);
            }
        }
        else if (entity.enemyType == EnemyType.Range)
        {
            if (hasAttacked)
            {
                Debug.Log("Change ranged active attacker is called");
                entity.coordinator.ChangeActiveAttacker(entity);

            }
        }

        if (eliteEnemy && hasAttacked)
        {
            eliteEnemy.hitCounter = 0;
            entity.EnemyCombatSystem.ResetCombo();
        }

        hasAttacked = false;
        enemyBlackboard.onlyTakeDamage = false;


    }

    public override void Update()
    {
        base.Update();

        if (!entity.EnemyCombatStatus.isAllowedToAttack)
        {
            stateMachine.ChangeState(enemyStateFactory.WaitingForAttackState);
        }

        if (animationHandler.IsPlaying(entity.EnemyCombatSystem.CurrentAttackData.attackAnimationName) && animationHandler.NormalizedTime() >= .5f)
        {
            hasAttacked = true;
        }

        if (animationHandler.IsPlaying(entity.EnemyCombatSystem.CurrentAttackData.attackAnimationName) && animationHandler.NormalizedTime() >= .9f && !entity.EnemyCombatSystem.canPlayNextAttack)
        {
            if (eliteEnemy)
            {
                stateMachine.ChangeState(enemyStateFactory.AggresiveWalkState);
            }
            else if (basicEnemy)
            {
                stateMachine.ChangeState(enemyStateFactory.WaitingForAttackState);
            }
        }
    }
}
