using UnityEngine;

public class PlayerWalkState : PlayerGroundState
{
    public PlayerWalkState(Player _player, StateMachine<Player> _stateMachine) : base(_player, _stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        playerBlackboard.canAttack = true;

        entity.PlayerHorizontalMovement.MoveSpeed = entity.Parameters.walkSpeed;


        if (SwitchCameras.IsLockOnTargetCameraActive)
        {
            animationHandler.CrossFade("Walk_BlendTree", .2f);
        }
        else
        {
            animationHandler.CrossFade("Walk", 0.2f);
        }
    }
    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        if (inputHandler.LeftShift && playerBlackboard.canRun)
        {
            stateMachine.ChangeState(playerStateFactory.RunState);
            return;
        }

        if(xInput == 0 && zInput == 0)
        {
            stateMachine.ChangeState(playerStateFactory.IdleState);
        }
    }

    protected override bool ShouldUpdateInput => true;
}
