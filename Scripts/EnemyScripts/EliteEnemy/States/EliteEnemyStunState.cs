using UnityEngine;

public class EliteEnemyStunState : EnemyBaseState
{
    bool inStunLoop;
    float internTimer;
    public EliteEnemyStunState(Enemy entity, EnemyStateFactory enemyStateFactory, StateMachine<Enemy> stateMachine) : base(entity, enemyStateFactory, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stopLookingAtPlayer = true;
        inStunState = true;
        animationHandler.Play("BigOrc_StunIn");
        entity.EnemyBlackboard.onlyTakeDamage = true;
    }

    public override void Exit()
    {
        base.Exit();
        stopLookingAtPlayer = false;
        inStunLoop = false;
        inStunState = false;
        eliteEnemy.EnemyParameters.currentStaggerValue = 0;
        entity.EnemyBlackboard.onlyTakeDamage = false;
        internTimer = 0;

        eliteEnemy.hitCounter = 0;
    }

    public override void Update()
    {
        base.Update();

        internTimer += Time.deltaTime;

        if(internTimer >= 5 && inStunLoop)
        {
            inStunLoop = false;
            animationHandler.Play("BigOrc_StunOut");
        }

        if(animationHandler.IsPlaying("BigOrc_StunIn") && animationHandler.NormalizedTime() >= 1 && !inStunLoop)
        {
            inStunLoop = true;
            animationHandler.Play("BigOrc_StunLoop");
        }

        if(animationHandler.IsPlaying("BigOrc_StunOut") && animationHandler.NormalizedTime() >= 1)
        {
            stateMachine.ChangeState(enemyStateFactory.IdleState);
        }
    }
}
