using UnityEngine;

public class EliteEnemyEvadeState : EnemyBaseState
{
    public EliteEnemyEvadeState(Enemy entity, EnemyStateFactory enemyStateFactory, StateMachine<Enemy> stateMachine) : base(entity, enemyStateFactory, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        animationHandler.Play("BigOrc_DodgeBackwards");
        entity.Agent.velocity = Vector3.zero;
        entity.Agent.ResetPath();
        entity.Agent.isStopped = true;
        entity.EnemyBlackboard.onlyTakeDamage = true;
        entity.rb.isKinematic = false;

        animationHandler.Anim.applyRootMotion = true;
    }

    public override void Exit()
    {
        base.Exit();
        entity.EnemyBlackboard.onlyTakeDamage = false;
        entity.rb.isKinematic = true;
        entity.Agent.isStopped = false;
    }

    public override void Update()
    {
        base.Update();

        if(animationHandler.IsPlaying("BigOrc_DodgeBackwards") && animationHandler.NormalizedTime() >= 1)
        {
            stateMachine.ChangeState(enemyStateFactory.IdleState);
        }
    }
}
