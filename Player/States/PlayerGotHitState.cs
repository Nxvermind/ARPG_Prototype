using UnityEngine;

public class PlayerGotHitState : PlayerGroundState
{
    public PlayerGotHitState(Player _player, StateMachine<Player> _stateMachine) : base(_player, _stateMachine)
    {
    }


    public override void Enter()
    {
        base.Enter();
        entity.rootMotion.ActivateRootMotion();
        playerBlackboard.canMove = false;
        playerBlackboard.canAttack = false;
        playerBlackboard.canParry = false;
        playerBlackboard.canDodge = false;

        playerBlackboard.canRestorePosture = false;
        animationHandler.Play("Got_Hit");
    }

    public override void Exit()
    {
        base.Exit();
        entity.rootMotion.DeactivateRootMotion();
        playerBlackboard.canAttack = true;
        playerBlackboard.canMove = true;
        playerBlackboard.canParry = true;

        playerBlackboard.canRestorePosture = true;
        playerBlackboard.lastTimeToStartRegenPosture = Time.time;
    }

    public override void Update()
    {
        base.Update();

        if (animationHandler.IsPlaying("Got_Hit") && animationHandler.NormalizedTime() >= .6f)
        {
            playerBlackboard.canDodge = true;
        }

        if (animationHandler.IsPlaying("Got_Hit") && animationHandler.NormalizedTime() >= .85f)
        {
            stateMachine.ChangeState(playerStateFactory.IdleState);
        }
    }
}
