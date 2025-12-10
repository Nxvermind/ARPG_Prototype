using UnityEngine;

public class PlayerRunState : PlayerGroundState
{
    public PlayerRunState(Player _player, StateMachine<Player> _stateMachine) : base(_player, _stateMachine)
    {
    }

    public override void Enter()
    {
        base.Enter();
        playerBlackboard.isRunning = true;

        entity.PlayerHorizontalMovement.MoveSpeed = entity.Parameters.runSpeed;


        animationHandler.CrossFade("Run", 0.15f);
    }
    public override void Update()
    {
        base.Update();

        animationHandler.Anim.applyRootMotion = false;

        playerBlackboard.LastTimeToStartRegenStamina = Time.time;

        entity.PlayerModel.ConsumeStamina(5 * Time.deltaTime);
        playerParameters.currentStamina = entity.PlayerModel.CurrentStamina;

        if (!Input.GetKey(KeyCode.LeftShift) || playerParameters.currentStamina <= 0)
        {
            stateMachine.ChangeState(playerStateFactory.WalkState);
            return;
        }

        if (xInput == 0 && zInput == 0)
        {
            stateMachine.ChangeState(playerStateFactory.IdleState);
        }
    }
    public override void Exit()
    {
        base.Exit();
        playerBlackboard.isRunning = false;
    }
}
