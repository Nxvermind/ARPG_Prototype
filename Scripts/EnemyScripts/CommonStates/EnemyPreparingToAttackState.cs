using UnityEngine;

public class EnemyPreparingToAttackState : EnemyBaseState
{
    public EnemyPreparingToAttackState(Enemy entity, EnemyStateFactory enemyStateFactory, StateMachine<Enemy> stateMachine) : base(entity, enemyStateFactory, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        //agent.ResetPath();
        animationHandler.CrossFade("PreparingToAttack", 0.1f);
        enemyVision.alreadySeenPlayer = true;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (animationHandler.IsPlaying("PreparingToAttack") && animationHandler.NormalizedTime() >= 1f)
        {
            stateMachine.ChangeState(enemyStateFactory.ChaseState);
        }
    }
}
