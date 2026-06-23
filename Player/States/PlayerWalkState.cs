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
        playerBlackboard.canDodge = true;

        entity.PlayerHorizontalMovement.MoveSpeed = entity.Parameters.walkSpeed;

        PlayAnim();

        animationHandler.Anim.SetBool("LockOn", entity.lockOnTargetBlackboard.IsLockOnTargetActive);
    }
    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        PerformFirstAttack();
        animationHandler.Anim.SetBool("LockOn", entity.lockOnTargetBlackboard.IsLockOnTargetActive);

        if (inputHandler.LeftShift && playerBlackboard.canRun)
        {
            stateMachine.ChangeState(playerStateFactory.RunState);
            return;
        }

        if(xInput == 0 && zInput == 0)
        {
            stateMachine.ChangeState(playerStateFactory.IdleState);
        }

        if (entity.PlayerModel.CurrentStamina > 0)
        {
            playerBlackboard.canDodge = true;
        }
    }

    private void PlayAnim()
    {
        string anim = entity.lockOnTargetBlackboard.IsLockOnTargetActive ? "Walk_BlendTree" : "Walk";
        animationHandler.CrossFade(anim, .1f);
    }
}
