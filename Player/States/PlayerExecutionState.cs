using System.Collections;
using UnityEngine;

public class PlayerExecutionState : PlayerGroundState
{
    public PlayerExecutionState(Player _player, StateMachine<Player> _stateMachine) : base(_player, _stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();

        playerBlackboard.canMove = false;
        playerBlackboard.isInvulnerable = true;
        alreadyExecutingEnemy = true;


        playerBlackboard.canAttack = false;
        playerBlackboard.onlyTakeDamage = true;

        animationHandler.Play("Execution");

        EventBus.StartExecution();
    }

    public override void Exit()
    {
        base.Exit();
        playerBlackboard.isInvulnerable = false;
        alreadyExecutingEnemy = false;
        playerBlackboard.onlyTakeDamage = false;
        playerBlackboard.canMove = true;

        EventBus.ExecutionEnded();
        entity.rootMotion.DeactivateRootMotion();
    }

    public override void Update()
    {
        base.Update();

        if (animationHandler.IsPlaying("Execution") && animationHandler.NormalizedTime() >= .9f)
        {
            stateMachine.ChangeState(playerStateFactory.IdleState);
        }

    }

}
