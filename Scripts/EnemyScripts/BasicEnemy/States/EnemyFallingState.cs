using UnityEngine;

public class EnemyFallingState : EnemyBaseState
{
    public EnemyFallingState(Enemy entity, EnemyStateFactory enemyStateFactory, StateMachine<Enemy> stateMachine) : base(entity, enemyStateFactory, stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        entity.Agent.enabled = false;
        entity.rb.isKinematic = false;
        //Debug.Log("im in falling state");
    }

    public override void Exit()
    {
        base.Exit();

        entity.Agent.enabled = true;
        entity.rb.isKinematic = true;
    }

    public override void Update()
    {
        base.Update();

        if (entity.EnemyGroundDetection.isGrounded)
        {
            if(enemyParameters.currentStaggerValue >= enemyParameters.maxStaggerValue)
            {
                stateMachine.ChangeState(enemyStateFactory.StaggerState);
                enemyParameters.currentStaggerValue = enemyParameters.maxStaggerValue;
            }
            else
            {
                stateMachine.ChangeState(enemyStateFactory.GetUpState);
            }

        }
    }
}
