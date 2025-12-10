using UnityEngine;

public class EnemyAlertState : EnemyBaseState
{
    public EnemyAlertState(Enemy entity, EnemyStateFactory enemyStateFactory, StateMachine<Enemy> stateMachine) : base(entity, enemyStateFactory, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        stopLookingAtPlayer = true;

        entity.Agent.ResetPath();
        entity.Agent.isStopped = true;
        entity.Agent.velocity = Vector3.zero;

        animationHandler.Play("Alert");
    }

    public override void Exit()
    {
        base.Exit();
        entity.Agent.isStopped = false;
    }

    public override void Update()
    {
        base.Update();

        if (enemyVision.playerInSight)
        {
            stateMachine.ChangeState(enemyStateFactory.ChaseState);
            return;
        }
        else
        {
            if(animationHandler.IsPlaying("Alert") && animationHandler.NormalizedTime() >= 0.75f)
            {
                if(stateMachine.PreviousState == enemyStateFactory.ChaseState)
                {
                    stateMachine.ChangeState(enemyStateFactory.LookingForPlayerState);
                }
                else if (stateMachine.PreviousState == enemyStateFactory.StaggerState)
                {
                    stateMachine.ChangeState(enemyStateFactory.TurnAroundState);
                }

            }
        }
    }
}
