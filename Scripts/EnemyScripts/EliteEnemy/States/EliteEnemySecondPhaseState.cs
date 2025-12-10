using System.Collections;
using UnityEngine;

public class EliteEnemySecondPhaseState : EnemyBaseState
{
    public EliteEnemySecondPhaseState(Enemy entity, EnemyStateFactory enemyStateFactory, StateMachine<Enemy> stateMachine) : base(entity, enemyStateFactory, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        eliteEnemy.secondPhaseActive = true;

        entity.Agent.isStopped = true;
        entity.Agent.velocity = Vector3.zero;
        entity.Agent.ResetPath();

        enemyBlackboard.onlyTakeDamage = true;
        enemyBlackboard.canUpdateStaggerValue = false;

        entity.EnemyCombatSystem.attackSettings = entity.EnemyCombatSystem.aggresiveSettings;

        eliteEnemy.maxHitCounter = 3;

        animationHandler.CrossFade("SecondPhase_In", 0.1f);
    }

    public override void Exit()
    {
        base.Exit();
        entity.Agent.isStopped = false;
        enemyBlackboard.onlyTakeDamage = false;
        enemyBlackboard.canUpdateStaggerValue = true;
    }

    public override void Update()
    {
        base.Update();

        if(animationHandler.IsPlaying("SecondPhase_In") && animationHandler.NormalizedTime() >= 1)
        {
            entity.StartCoroutine(Delay());
        }

        if(animationHandler.IsPlaying("SecondPhase_Out") && animationHandler.NormalizedTime() >= 1)
        {
            stateMachine.ChangeState(enemyStateFactory.IdleState);
        }
    }

    IEnumerator Delay()
    {
        animationHandler.Play("SecondPhase_Loop");

        yield return new WaitForSecondsRealtime(1.5f);

        animationHandler.CrossFade("SecondPhase_Out", .1f);
    }
}
